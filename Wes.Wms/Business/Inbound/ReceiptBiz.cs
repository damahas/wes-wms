using System;
using System.Linq;
using SqlSugar;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 入库单 业务逻辑
	/// </summary>
	public class ReceiptBiz : IReceiptBiz
	{
		private readonly IReceiptService _receiptService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;
		private readonly IReceivingDetailService _receivingDetailService;
		private readonly IReceivingService _receivingService;
		private readonly IIqcOrderService _iqcOrderService;
		private readonly IStockService _stockService;
		private readonly IStockRecordService _stockRecordService;
		private readonly IReceiptDetailBarcodeService _barcodeService;
		private readonly ISqlSugarClient _client;

		public ReceiptBiz(IReceiptService receiptService, ISysCodeRuleBiz codeRuleBiz,
			IReceivingDetailService receivingDetailService, IReceivingService receivingService,
			IIqcOrderService iqcOrderService, IStockService stockService,
			IStockRecordService stockRecordService,
			IReceiptDetailBarcodeService barcodeService, ISqlSugarClient sqlSugarClient)
		{
			_receiptService = receiptService;
			_codeRuleBiz = codeRuleBiz;
			_receivingDetailService = receivingDetailService;
			_receivingService = receivingService;
			_iqcOrderService = iqcOrderService;
			_stockService = stockService;
			_stockRecordService = stockRecordService;
			_barcodeService = barcodeService;
			_client = sqlSugarClient;
		}

		public RowData<WmsReceiptEntity> GetList(ParamData<WmsReceiptParam> param)
		{
			int total = 0;
			var result = new RowData<WmsReceiptEntity>(_receiptService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsReceiptEntity> GetById(long id)
		{
			var entity = _receiptService.GetById(id);
			if (entity != null)
			{
				EnsureBarcodesForReceipt(id);   // 历史明细兜底补码

				// 回填条码统计，供前端展示「共 N 码 / 已上架 X」
				foreach (var d in entity.Details ?? new List<WmsReceiptDetailEntity>())
				{
					var barcodes = _barcodeService.GetListByDetailId(d.DetailId) ?? new List<WmsReceiptDetailBarcodeEntity>();
					d.BarcodeCount = barcodes.Count;
					d.ShelvedBarcodeCount = barcodes.Count(p => p.Status == "shelved");
				}
			}
			return new ResultData<WmsReceiptEntity>(entity);
		}

		public ReturnData Save(WmsReceiptEntity model)
		{
			// 已开始上架的单据不允许修改：编辑时会重建明细与条码，已入库的库存将失去来源记录
			if (model.ReceiptId > 0)
			{
				var exist = _receiptService.GetById(model.ReceiptId);
				if (exist == null) return new ReturnData(500, "入库单不存在！");
				if (exist.ReceiptStatus == "in_stock")
					return new ReturnData(500, "入库单已入库，不能再修改！");
				if (exist.ReceiptStatus == "shelfing")
					return new ReturnData(500, "入库单已有明细上架，不能再修改！");
			}

			// 入库日期兜底（数据库非空）：未传时取当前日期，避免抛出数据库层的原始错误
			if (model.ReceiptDate == null)
				model.ReceiptDate = DateTime.Now;

			// 新建时默认待入库（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
			if (string.IsNullOrWhiteSpace(model.ReceiptStatus))
				model.ReceiptStatus = "pending_in";

			// 校验明细
			if (model.Details == null || model.Details.Count == 0)
				return new ReturnData(500, "请至少添加一条入库明细！");

			foreach (var d in model.Details)
			{
				if (d.MaterialId <= 0)
					return new ReturnData(500, "请选择入库物料！");
				if (d.ReceiptQuantity <= 0)
					return new ReturnData(500, "入库数量必须大于0！");
			}

			// 采购入库：校验入库数量不超过收货单允许入库数
			if (model.ReceiptType == "purchase" && model.SourceId.HasValue && model.SourceId.Value > 0)
			{
				var receivingDetails = _receivingDetailService.GetListByReceivingId(model.SourceId.Value);
				if (receivingDetails == null || receivingDetails.Count == 0)
					return new ReturnData(500, "未找到关联的收货单明细！");

				foreach (var d in model.Details)
				{
					var receivingDetail = receivingDetails.FirstOrDefault(a => a.MaterialId == d.MaterialId);
					if (receivingDetail == null)
						return new ReturnData(500, $"收货单中不存在物料，无法入库！");
					var availableQty = (receivingDetail.PendingInboundQty ?? 0) - (receivingDetail.StockInQuantity ?? 0);
					if (d.ReceiptQuantity > availableQty)
						return new ReturnData(500, $"入库数量({d.ReceiptQuantity})超过收货单待入库数({availableQty})！");
				}
			}

			// 新增且未传编码时，按规则自动生成单据编码
			if (model.ReceiptId == 0 && string.IsNullOrWhiteSpace(model.ReceiptNo))
			{
				string err;
				model.ReceiptNo = _codeRuleBiz.GetCode("WMS_RECEIPT", out err);
				if (!string.IsNullOrWhiteSpace(err))
					return new ReturnData(500, err);
			}

			_client.Ado.BeginTran();
			try
			{
				// 保存主表
				if (!_receiptService.Save(model))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存入库单失败！");
				}

				// 编辑时：删除旧明细及其条码
				if (model.ReceiptId > 0)
				{
					_receiptService.DeleteDetailsByReceiptId(model.ReceiptId);
					_barcodeService.DeleteByReceiptId(model.ReceiptId);
				}

				// 插入新明细
				_receiptService.BatchSaveDetails(model.ReceiptId, model.Details);

				// 按物料包装规格拆码：箱装一箱一码，散装/最小单位一码绑全部数量
				GenerateBarcodes(model.ReceiptId, model.Details);

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"保存失败：{ex.Message}");
			}

			return new ReturnData();
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			// 已开始上架的单据不能删除（库存已增加、账务已记，删除会丢失来源凭证）
			foreach (var id in delIds)
			{
				var exist = _receiptService.GetById(id);
				if (exist != null && (exist.ReceiptStatus == "in_stock" || exist.ReceiptStatus == "shelfing"))
					return new ReturnData(500, $"入库单 {exist.ReceiptNo} 已开始上架，不能删除！");
			}

			_client.Ado.BeginTran();
			try
			{
				// 级联软删除明细及其条码
				foreach (var id in delIds)
				{
					_receiptService.DeleteDetailsByReceiptId(id);
					_barcodeService.DeleteByReceiptId(id);
				}

				// 软删除主表
				if (!_receiptService.Delete(delIds))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "删除失败！");
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"删除失败：{ex.Message}");
			}

			return new ReturnData();
		}

		/// <summary>从收货单创建入库单（跨模块：Receiving → Receipt）</summary>
		public WmsReceiptEntity CreateFromReceiving(string receiptNo, long receivingId, long? supplierId,
			List<WmsReceivingDetailEntity> exemptDetails)
		{
			var receipt = new WmsReceiptEntity
			{
				ReceiptId = 0,
				ReceiptNo = receiptNo,
				ReceiptType = "purchase",
				ReceiptStatus = "pending_in",
				SourceType = "receiving",
				SourceId = receivingId,
				WarehouseId = 0,
				SupplierId = supplierId,
				ReceiptDate = DateTime.Now,
			};

			if (!_receiptService.Save(receipt))
				throw new Exception("生成入库单失败！");

			// 转换收货明细 → 入库明细
			var details = exemptDetails.Select(d => new WmsReceiptDetailEntity
			{
				ReceiptId = receipt.ReceiptId,
				DetailId = 0,
				MaterialId = d.MaterialId,
				Spec = d.Spec,
				Unit = d.Unit,
				BatchNo = d.BatchNo,
				ReceiptQuantity = d.PlanQuantity,
				SupplierId = supplierId,
				LocationId = d.Material?.DefaultLocationId,
			}).ToList();
			_receiptService.BatchSaveDetails(receipt.ReceiptId, details);
			GenerateBarcodes(receipt.ReceiptId, details);

			return receipt;
		}

		/// <summary>
		/// 从检验单创建入库单（跨模块：Inspection → Receipt）
		/// 同一收货单已存在「待入库(pending_in)」的入库单时，直接把明细合并进去，不重复建单
		/// </summary>
		public WmsReceiptEntity CreateFromInspection(long receivingId, long? supplierId,
			List<WmsReceiptDetailEntity> receiptDetails)
		{
			// 同一收货单下已存在待入库的入库单 → 合并明细
			var pendingReceipt = _receiptService.GetByReceivingId(receivingId)?
				.Where(p => p.ReceiptStatus == "pending_in")
				.OrderBy(p => p.ReceiptId)
				.FirstOrDefault();

			if (pendingReceipt != null)
			{
				MergeDetails(pendingReceipt.ReceiptId, receiptDetails);
				return pendingReceipt;
			}

			// 不存在则新建（此时才消耗单据编码，避免合并时产生断号）
			string err;
			var receiptNo = _codeRuleBiz.GetCode("WMS_RECEIPT", out err);
			if (!string.IsNullOrWhiteSpace(err))
				throw new Exception(err);

			var receipt = new WmsReceiptEntity
			{
				ReceiptId = 0,
				ReceiptNo = receiptNo,
				ReceiptType = "purchase",
				ReceiptStatus = "pending_in",
				SourceType = "receiving",
				SourceId = receivingId,
				WarehouseId = 0,
				SupplierId = supplierId,
				ReceiptDate = DateTime.Now,
				TotalQuantity = receiptDetails?.Sum(d => d.ReceiptQuantity) ?? 0,
			};

			if (!_receiptService.Save(receipt))
				throw new Exception("生成入库单失败！");

			_receiptService.BatchSaveDetails(receipt.ReceiptId, receiptDetails);
			GenerateBarcodes(receipt.ReceiptId, receiptDetails);

			return receipt;
		}

		/// <summary>
		/// 合并入库明细：同物料 + 同批号 → 数量累加；其余 → 新增明细行
		/// </summary>
		private void MergeDetails(long receiptId, List<WmsReceiptDetailEntity> newDetails)
		{
			if (newDetails == null || newDetails.Count == 0) return;

			var exists = _receiptService.GetDetailsByReceiptId(receiptId) ?? new List<WmsReceiptDetailEntity>();
			var toInsert = new List<WmsReceiptDetailEntity>();
			var addedQty = 0;

			foreach (var d in newDetails)
			{
				var match = exists.FirstOrDefault(a => a.MaterialId == d.MaterialId
					&& (a.BatchNo == d.BatchNo || (string.IsNullOrEmpty(a.BatchNo) && string.IsNullOrEmpty(d.BatchNo))));

				if (match != null)
				{
					_receiptService.AddDetailQuantity(match.DetailId, d.ReceiptQuantity);
					// 数量增加 → 按新增数量补码（箱装补对应箱数的码）
					GenerateBarcodes(receiptId, new List<WmsReceiptDetailEntity> { match });
				}
				else
				{
					toInsert.Add(d);
					exists.Add(d); // 避免本次新增的明细被后续行重复匹配
				}

				addedQty += d.ReceiptQuantity;
			}

			if (toInsert.Count > 0)
			{
				_receiptService.BatchSaveDetails(receiptId, toInsert);
				GenerateBarcodes(receiptId, toInsert);
			}

			if (addedQty > 0)
				_receiptService.AddTotalQuantity(receiptId, addedQty);
		}

		/// <summary>
		/// 确认入库（兼容保留）：入库完成现在由「明细上架」驱动 —— 全部明细上架完成后自动置 in_stock。
		/// 这里只做兜底：明细确实已全部上架时允许补一次状态，否则提示先上架。
		/// </summary>
		public ReturnData ConfirmInbound(long receiptId)
		{
			var details = _receiptService.GetDetailsByReceiptId(receiptId);
			if (details == null || details.Count == 0) return new ReturnData(500, "入库单无明细");

			var unshelved = details.Count(p => (p.ShelfQuantity ?? 0) < p.ReceiptQuantity);
			if (unshelved > 0)
				return new ReturnData(500, $"还有 {unshelved} 条明细未上架完成，请完成上架后自动入库");

			_receiptService.UpdateStatus(receiptId, "in_stock");
			return new ReturnData();
		}

		/// <summary>
		/// 明细上架：写库存 → 明细累加已上架数量并流转状态 → 回写收货单已入库数量 → 重算主表状态。
		/// 全部明细上架完成后入库单自动置为 in_stock（不再有单独的确认入库动作）
		/// </summary>
		public ReturnData Shelve(long receiptId, long detailId, long locationId, int quantity, string onlyBarcode = null)
		{
			if (quantity <= 0) return new ReturnData(500, "上架数量必须大于0！");

			var receipt = _receiptService.GetById(receiptId);
			if (receipt == null) return new ReturnData(500, "入库单不存在！");
			if (receipt.ReceiptStatus == "in_stock") return new ReturnData(500, "入库单已完成入库，不能再上架！");

			var detail = _receiptService.GetDetailById(detailId);
			if (detail == null || detail.ReceiptId != receiptId) return new ReturnData(500, "入库明细不存在！");
			if (detail.DetailStatus == "shelved") return new ReturnData(500, "该明细已全部上架！");

			var shelvedQty = detail.ShelfQuantity ?? 0;
			var remainQty = detail.ReceiptQuantity - shelvedQty;
			if (quantity > remainQty) return new ReturnData(500, $"上架数量({quantity})超过待上架数量({remainQty})！");

			// 兜底补码：历史明细（条码规则上线前建的）可能还没有码
			GenerateBarcodes(receiptId, new List<WmsReceiptDetailEntity> { detail });

			// 本次上架取前 quantity 个未上架的条码（一箱一码）；PDA 扫码时只上指定的那一个
			var allBarcodes = _barcodeService.GetListByDetailId(detailId) ?? new List<WmsReceiptDetailBarcodeEntity>();
			var pending = (string.IsNullOrWhiteSpace(onlyBarcode)
					? allBarcodes.Where(p => p.Status != "shelved").OrderBy(p => p.SeqNo).Take(quantity)
					: allBarcodes.Where(p => p.Barcode == onlyBarcode && p.Status != "shelved").Take(1))
				.ToList();
			if (pending.Count == 0) return new ReturnData(500, "该明细已全部上架！");
			if (pending.Count < quantity)
				return new ReturnData(500, $"待上架条码只有 {pending.Count} 个，不足 {quantity} 个！");

			var location = _client.Queryable<WmsLocationEntity>()
				.Where(p => p.LocationId == locationId && p.IsDel == 0).First();
			if (location == null) return new ReturnData(500, "库位不存在！");

			var material = _client.Queryable<WmsMaterialEntity>()
				.Where(p => p.MaterialId == detail.MaterialId && p.IsDel == 0).First();
			var warehouse = _client.Queryable<WmsWarehouseEntity>()
				.Where(p => p.WarehouseId == receipt.WarehouseId && p.IsDel == 0).First();
			var zone = _client.Queryable<WmsZoneEntity>()
				.Where(p => p.ZoneId == location.ZoneId && p.IsDel == 0).First();

			// 本次上架数量（明细单位：箱装即箱数）
			var shelveQty = pending.Sum(p => p.Quantity ?? 0);
			var newShelvedQty = shelvedQty + shelveQty;
			var detailStatus = newShelvedQty >= detail.ReceiptQuantity ? "shelved" : "shelving";

			_client.Ado.BeginTran();
			try
			{
				// 1. 逐个条码写入库存：一箱一条库存，条码即该笔库存的码
				foreach (var b in pending)
				{
					var stock = new WmsStockEntity
					{
						Barcode = b.Barcode,
						MaterialId = detail.MaterialId,
						MaterialCode = material?.MaterialCode,
						MaterialName = material?.MaterialName,
						Spec = detail.Spec ?? material?.Spec,
						Unit = b.Unit ?? detail.Unit ?? material?.Unit,
						BatchNo = detail.BatchNo,
						WarehouseId = receipt.WarehouseId,
						WarehouseName = warehouse?.WarehouseName,
						ZoneId = location.ZoneId,
						ZoneCode = zone?.ZoneCode,
						ZoneName = zone?.ZoneName,
						LocationId = locationId,
						LocationCode = location.LocationCode,
						LocationName = location.LocationName,
						SupplierId = detail.SupplierId ?? receipt.SupplierId,
						ProductionDate = detail.MaterialionDate,
						ExpireDate = detail.ExpireDate,
						StockQuantity = b.Quantity ?? 0,
						BaseQuantity = b.BaseQuantity,
						PackSpecId = b.PackSpecId,
						ReceiptId = receiptId,
						ReceiptDetailId = detailId,
					};

					if (!_stockService.AddOrUpdateStock(stock))
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "写入库存失败！");
					}

					// 写库存流水：一码一条流水（before/after 按该条码库存计算）
					var beforeQty = ExistStockQty(b.Barcode);
					var changeQty = b.Quantity ?? 0;
					var saved = _stockRecordService.Save(new WmsStockRecordEntity
					{
						RecordNo = $"{receipt.ReceiptNo}-{b.SeqNo ?? 0:000}",
						MaterialId = detail.MaterialId,
						BatchNo = detail.BatchNo,
						WarehouseId = receipt.WarehouseId,
						ZoneId = location.ZoneId,
						LocationId = locationId,
						BizType = "receipt",
						SourceType = "receipt",
						SourceId = receiptId,
						SourceNo = receipt.ReceiptNo,
						Direction = "in",
						BeforeQuantity = beforeQty,
						ChangeQuantity = changeQty,
						AfterQuantity = beforeQty + changeQty,
						OperateTime = DateTime.Now,
						Remark = "上架入库",
					});
					if (!saved)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "写入库存流水失败！");
					}

					if (!_barcodeService.UpdateShelved(b.BarcodeId, locationId))
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "更新条码状态失败！");
					}
				}

				// 2. 明细：累加已上架数量 + 流转状态 + 回写上架库位
				if (!_receiptService.UpdateShelveInfo(detailId, (int)shelveQty, detailStatus, null, locationId))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "更新明细上架数量失败！");
				}

				// 3. 回写收货单已入库数量（累加）
				var receivingId = receipt.SourceId ?? 0;
				if (receivingId > 0)
				{
					var receivingDetails = _receivingDetailService.GetListByReceivingId(receivingId);
					var receivingDetail = receivingDetails?.FirstOrDefault(a => a.MaterialId == detail.MaterialId &&
						(a.BatchNo == detail.BatchNo || (string.IsNullOrEmpty(a.BatchNo) && string.IsNullOrEmpty(detail.BatchNo))));
					if (receivingDetail != null)
						_receivingDetailService.UpdateStockInQuantity(receivingDetail.DetailId, shelveQty);
				}

				// 4. 重算主表状态
				RefreshReceiptStatus(receiptId);

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"上架失败：{ex.Message}");
			}

			// 明细全部上架后，检查收货单是否可以完成
			if (detailStatus == "shelved")
			{
				var rid = receipt.SourceId ?? 0;
				if (rid > 0) CheckAndCompleteReceiving(rid);
			}

			return new ReturnData();
		}

		/// <summary>按条码上架（PDA：扫条码 → 扫库位 → 上架，一次一个码 = 一箱）</summary>
		public ReturnData ShelveByBarcode(string barcode, long locationId, int quantity)
		{
			if (string.IsNullOrWhiteSpace(barcode)) return new ReturnData(500, "条码不能为空！");
			var b = _barcodeService.GetByBarcode(barcode);
			if (b == null) return new ReturnData(500, "未找到该条码对应的入库明细！");
			if (b.Status == "shelved") return new ReturnData(500, "该条码已上架！");
			return Shelve(b.ReceiptId, b.DetailId, locationId, quantity > 0 ? quantity : 1, barcode);
		}

		/// <summary>按条码查询明细条码（PDA 扫码回显用）</summary>
		public WmsReceiptDetailBarcodeEntity GetDetailByBarcode(string barcode)
		{
			return _barcodeService.GetByBarcode(barcode);
		}

		/// <summary>明细条码列表（每码承载数量可不等）</summary>
		public List<WmsReceiptDetailBarcodeEntity> GetBarcodes(long detailId)
		{
			return _barcodeService.GetListByDetailId(detailId);
		}

		/// <summary>
		/// 拆分条码：把一个待上架的码拆成多个，拆分数量之和必须等于原数量（15 → [10, 5]）
		/// </summary>
		public ReturnData SplitBarcode(long detailId, long barcodeId, List<decimal> quantities)
		{
			if (quantities == null || quantities.Count < 2)
				return new ReturnData(500, "至少拆成 2 个条码！");
			if (quantities.Any(p => p <= 0))
				return new ReturnData(500, "拆分数量必须大于0！");

			var source = _barcodeService.GetById(barcodeId);
			if (source == null || source.DetailId != detailId)
				return new ReturnData(500, "条码不存在！");
			if (source.Status == "shelved")
				return new ReturnData(500, "已上架的条码不能拆分！");

			var sum = quantities.Sum();
			if (sum != (source.Quantity ?? 0))
				return new ReturnData(500, $"拆分数量合计({sum})必须等于原条码数量({source.Quantity})！");

			// 基本单位换算比不变
			var factor = (source.Quantity ?? 0) > 0 ? (source.BaseQuantity ?? 0) / (source.Quantity ?? 0) : 1;

			_client.Ado.BeginTran();
			try
			{
				var list = new List<WmsReceiptDetailBarcodeEntity>();
				foreach (var q in quantities)
				{
					string err;
					var code = _codeRuleBiz.GetCode("WMS_BARCODE", out err);
					if (!string.IsNullOrWhiteSpace(err))
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, err);
					}

					list.Add(new WmsReceiptDetailBarcodeEntity
					{
						ReceiptId = source.ReceiptId,
						DetailId = detailId,
						Barcode = code,
						SeqNo = source.SeqNo ?? 1,
						Quantity = q,
						BaseQuantity = q * factor,
						Unit = source.Unit,
						PackSpecId = source.PackSpecId,
						BatchNo = source.BatchNo,
						LocationId = source.LocationId,
						Status = "pending",
					});
				}

				_barcodeService.BatchSave(list);
				_barcodeService.DeleteById(barcodeId);

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"拆分失败：{ex.Message}");
			}

			return new ReturnData();
		}

		/// <summary>
		/// 合并条码：同一明细的多个待上架码合并为一个，数量相加（保留序号最小的码）
		/// </summary>
		public ReturnData MergeBarcodes(long detailId, List<long> barcodeIds)
		{
			if (barcodeIds == null || barcodeIds.Count < 2)
				return new ReturnData(500, "至少选择 2 个条码！");

			var all = _barcodeService.GetListByDetailId(detailId) ?? new List<WmsReceiptDetailBarcodeEntity>();
			var targets = all.Where(p => barcodeIds.Contains(p.BarcodeId)).ToList();
			if (targets.Count != barcodeIds.Count)
				return new ReturnData(500, "存在不属于该明细的条码！");
			if (targets.Any(p => p.Status == "shelved"))
				return new ReturnData(500, "已上架的条码不能合并！");

			var keep = targets.OrderBy(p => p.SeqNo ?? 0).First();
			var totalQty = targets.Sum(p => p.Quantity ?? 0);
			var totalBase = targets.Sum(p => p.BaseQuantity ?? 0);

			_client.Ado.BeginTran();
			try
			{
				if (!_barcodeService.UpdateQuantity(keep.BarcodeId, totalQty, totalBase))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "更新条码数量失败！");
				}

				foreach (var t in targets.Where(p => p.BarcodeId != keep.BarcodeId))
					_barcodeService.DeleteById(t.BarcodeId);

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"合并失败：{ex.Message}");
			}

			return new ReturnData();
		}

		/// <summary>
		/// 重算入库单状态：全部明细上架完成 → in_stock；部分上架 → shelfing；均未上架 → pending_in
		/// </summary>
		private void RefreshReceiptStatus(long receiptId)
		{
			var details = _receiptService.GetDetailsByReceiptId(receiptId) ?? new List<WmsReceiptDetailEntity>();
			if (details.Count == 0) return;

			string status;
			if (details.All(p => (p.ShelfQuantity ?? 0) >= p.ReceiptQuantity))
				status = "in_stock";
			else if (details.Any(p => (p.ShelfQuantity ?? 0) > 0))
				status = "shelfing";
			else
				status = "pending_in";

			_receiptService.UpdateStatus(receiptId, status);
		}

		/// <summary>某条码当前库存数量（写账务前取变动前数量）</summary>
		private decimal ExistStockQty(string barcode)
		{
			if (string.IsNullOrWhiteSpace(barcode)) return 0;
			var stock = _client.Queryable<WmsStockEntity>()
				.Where(p => p.IsDel == 0 && p.Barcode == barcode).First();
			return stock?.StockQuantity ?? 0;
		}

		/// <summary>
		/// 按物料包装规格为明细生成条码（一箱一码）：
		/// 1. 物料有包装规格，且明细单位对应「外层包装」（该层级还有下级，如 箱/托）→ 按数量拆码，每码 1 箱
		/// 2. 其余（最小单位、散装、无包装规格）→ 生成一个条码，承载全部数量
		/// 已有条码的明细不会重复生成；已上架部分只补剩余数量的码
		/// </summary>
		private void GenerateBarcodes(long receiptId, List<WmsReceiptDetailEntity> details)
		{
			if (details == null || details.Count == 0) return;

			var list = new List<WmsReceiptDetailBarcodeEntity>();
			foreach (var d in details)
			{
				if (d.DetailId == 0) continue;

				var exists = _barcodeService.GetListByDetailId(d.DetailId) ?? new List<WmsReceiptDetailBarcodeEntity>();
				var existingQty = exists.Sum(p => p.Quantity ?? 0);
				var remainQty = d.ReceiptQuantity - existingQty;   // 数量追加时只补差额
				if (remainQty <= 0) continue;

				var material = _client.Queryable<WmsMaterialEntity>()
					.Where(p => p.MaterialId == d.MaterialId && p.IsDel == 0).First();
				var packNodes = LoadPackNodes(material);
				var node = FindPackNode(packNodes, d.Unit);

				// 外层包装（箱/托）→ 默认一箱一码；最小单位/散装 → 一码绑全量
				var splitByPack = node != null && packNodes.Any(p => p.ParentId == node.SpecId);

				// 每码承载数量：箱装 1 箱/码，散装/最小单位一个码绑全量
				var perQty = splitByPack ? 1m : remainQty;

				var need = (int)Math.Ceiling(remainQty / perQty);
				var seqStart = exists.Count;
				var baseFactor = splitByPack ? CalcBaseFactor(packNodes, node) : 1m;

				for (var i = 0; i < need; i++)
				{
					// 最后一个码承载余数：15 按 10 拆 → [10, 5] 两个码
					var qty = i == need - 1 ? remainQty - perQty * (need - 1) : perQty;

					string err;
					var code = _codeRuleBiz.GetCode("WMS_BARCODE", out err);
					if (!string.IsNullOrWhiteSpace(err)) throw new Exception(err);

					list.Add(new WmsReceiptDetailBarcodeEntity
					{
						ReceiptId = receiptId,
						DetailId = d.DetailId,
						Barcode = code,
						SeqNo = seqStart + i + 1,
						Quantity = qty,
						BaseQuantity = qty * baseFactor,
						Unit = d.Unit,
						PackSpecId = node?.SpecId,
						BatchNo = d.BatchNo,
						LocationId = d.LocationId,
						Status = "pending",
					});
				}
			}

			if (list.Count > 0)
				_barcodeService.BatchSave(list);
		}

		/// <summary>
		/// 兜底补码：条码规则上线前的历史明细，打开入库单时按当前规则补生成
		/// </summary>
		private void EnsureBarcodesForReceipt(long receiptId)
		{
			var details = _receiptService.GetDetailsByReceiptId(receiptId);
			if (details == null || details.Count == 0) return;
			GenerateBarcodes(receiptId, details);
		}

		/// <summary>加载物料的包装规格层级树</summary>
		private List<WmsPackSpecEntity> LoadPackNodes(WmsMaterialEntity material)
		{
			if (material?.PackSpecId == null) return new List<WmsPackSpecEntity>();
			return _client.Queryable<WmsPackSpecEntity>()
				.Where(p => p.RootId == material.PackSpecId.Value && p.IsDel == 0)
				.ToList();
		}

		private WmsPackSpecEntity FindPackNode(List<WmsPackSpecEntity> nodes, string unit)
		{
			if (nodes == null || nodes.Count == 0 || string.IsNullOrWhiteSpace(unit)) return null;
			return nodes.FirstOrDefault(p => p.Uom == unit);
		}

		/// <summary>从当前层级折算到最小单位：沿子链相乘 qty_per_parent（箱 → 个）</summary>
		private decimal CalcBaseFactor(List<WmsPackSpecEntity> nodes, WmsPackSpecEntity node)
		{
			decimal factor = 1;
			var cur = node;
			while (cur != null)
			{
				var child = nodes.FirstOrDefault(p => p.ParentId == cur.SpecId);
				if (child == null) break;
				factor *= child.QtyPerParent ?? 1;
				cur = child;
			}
			return factor;
		}

		/// <summary>
		/// 检查收货单完成条件：
		/// 1. 所有抽检/全检物料的检验单均已完成
		/// 2. 所有物料的 StockInQuantity >= PendingInboundQty（待入库数量全部确认）
		/// 满足 → 收货单状态 = completed
		/// </summary>
		private void CheckAndCompleteReceiving(long receivingId)
		{
			var receiving = _receivingService.GetById(receivingId);
			if (receiving == null || receiving.ReceivingStatus == "completed") return;

			var details = _receivingDetailService.GetIncompleteDetails(receivingId);
			if (details == null || details.Count == 0)
			{
				_receivingService.UpdateStatus(receivingId, "completed");
				return;
			}

			// 检查每条明细：StockInQuantity >= PendingInboundQty（已入库数 >= 待入库数）
			foreach (var d in details)
			{
				var stockIn = d.StockInQuantity ?? 0;
				var pending = d.PendingInboundQty ?? 0;
				if (stockIn < pending)
					return; // 还有未确认入库的，不完成
			}

			// 检查该收货单下所有检验单是否已完成
			var unfinishedOrders = _iqcOrderService.GetUncompletedByReceivingId(receivingId);
			if (unfinishedOrders != null && unfinishedOrders.Count > 0)
				return; // 还有未完成检验的

			_receivingService.UpdateStatus(receivingId, "completed");
		}
	}
}
