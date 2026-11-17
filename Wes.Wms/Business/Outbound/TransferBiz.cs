using System;
using System.Collections.Generic;
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
	/// 调拨单 业务逻辑
	///
	/// 只管跨仓：库内搬货请用移库单。拆开是因为跨仓多出三样东西——在途账、两个阶段、两个责任人，
	/// 混在移库单里会让状态机与字段互相打架（最典型的是移库明细的 to_location_id 非空，
	/// 逼着开单人替对方仓库决定货放哪个货位）。
	///
	/// 流程：待调拨 --移出--> 在途 --移入--> 已完成
	/// 移出由调出仓的人操作，移入由调入仓的人操作，中间那段叫在途。
	/// 移入按实收数量入账，少收的差额继续挂在途上——系统不说谎，
	/// 在途数量永远等于"已发出未收到"。
	/// </summary>
	public class TransferBiz : ITransferBiz
	{
		private readonly ITransferService _transferService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;
		private readonly IStockService _stockService;
		private readonly IStockRecordService _stockRecordService;
		private readonly ISqlSugarClient _client;

		public TransferBiz(ITransferService transferService, ISysCodeRuleBiz codeRuleBiz,
			IStockService stockService, IStockRecordService stockRecordService,
			ISqlSugarClient sqlSugarClient)
		{
			_transferService = transferService;
			_codeRuleBiz = codeRuleBiz;
			_stockService = stockService;
			_stockRecordService = stockRecordService;
			_client = sqlSugarClient;
		}

		public RowData<WmsTransferEntity> GetList(ParamData<WmsTransferParam> param)
		{
			int total = 0;
			var result = new RowData<WmsTransferEntity>(_transferService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsTransferEntity> GetById(long id)
		{
			return new ResultData<WmsTransferEntity>(_transferService.GetById(id));
		}

		public ReturnData Save(WmsTransferEntity model)
		{
			// 已开始作业的单据不允许改：库存与在途账都已变动
			if (model.TransferId > 0)
			{
				var exist = _transferService.GetById(model.TransferId);
				if (exist == null) return new ReturnData(500, "调拨单不存在！");
				if (exist.TransferStatus == "completed")
					return new ReturnData(500, "该调拨单已完成，不能修改！");
				if (exist.TransferStatus == "in_transit")
					return new ReturnData(500, "该调拨单已移出，不能修改！");
				if (exist.TransferStatus == "cancelled")
					return new ReturnData(500, "该调拨单已取消！");
			}

			// 调拨单的立身之本是跨仓，同仓搬货请走移库单
			if (model.FromWarehouseId <= 0) return new ReturnData(500, "请选择调出仓库！");
			if (model.ToWarehouseId <= 0) return new ReturnData(500, "请选择调入仓库！");
			if (model.FromWarehouseId == model.ToWarehouseId)
				return new ReturnData(500, "调出仓库与调入仓库相同，库内搬货请使用移库单！");

			if (model.Details == null || model.Details.Count == 0)
				return new ReturnData(500, "请至少添加一条调拨明细！");

			foreach (var d in model.Details)
			{
				if (d.MaterialId <= 0) return new ReturnData(500, "请选择调拨物料！");
				if (d.TransferQuantity <= 0) return new ReturnData(500, "调拨数量必须大于0！");

				// 源库位必填：从哪个货位拿货，开单的人是清楚的
				if (!d.FromLocationId.HasValue || d.FromLocationId.Value <= 0)
					return new ReturnData(500, "请选择调出库位！");

				var fromWh = GetWarehouseIdOfLocation(d.FromLocationId.Value);
				if (fromWh == null) return new ReturnData(500, "调出库位不存在！");
				if (fromWh.Value != model.FromWarehouseId)
					return new ReturnData(500, "调出库位不属于调出仓库！");
			}

			// 新增且未传编码时，按规则自动生成单据编码
			if (model.TransferId == 0 && string.IsNullOrWhiteSpace(model.TransferNo))
			{
				string err;
				model.TransferNo = _codeRuleBiz.GetCode("WMS_TRANSFER", out err);
				if (!string.IsNullOrWhiteSpace(err))
					return new ReturnData(500, err);
			}

			// 补全物料冗余信息与调出库位编码
			foreach (var d in model.Details)
			{
				var material = _client.Queryable<WmsMaterialEntity>()
					.Where(p => p.MaterialId == d.MaterialId && p.IsDel == 0).First();
				if (material == null) return new ReturnData(500, "选择的物料不存在！");

				d.MaterialCode = material.MaterialCode;
				d.MaterialName = material.MaterialName;
				d.Spec = material.Spec;
				d.Unit = material.Unit;

				var fromLoc = _client.Queryable<WmsLocationEntity>()
					.Where(p => p.LocationId == d.FromLocationId.Value && p.IsDel == 0).First();
				if (fromLoc == null) return new ReturnData(500, "调出库位不存在！");
				d.FromLocationCode = fromLoc.LocationCode;
				if (!d.FromZoneId.HasValue || d.FromZoneId.Value <= 0)
					d.FromZoneId = fromLoc.ZoneId;

				// 数量必须等于库存行的可用量：一条库存一个条码，搬半条的话标签就对不上实物了。
				// 后端自己按库存行算，不采信前端传来的数量。
				var srcStock = FindStock(model.FromWarehouseId, d.MaterialId, d.BatchNo, d.FromLocationId.Value);
				if (srcStock == null)
					return new ReturnData(500, $"物料【{material.MaterialName}】在调出库位没有库存！");

				var available = (srcStock.StockQuantity ?? 0)
					- (srcStock.FreezeQuantity ?? 0) - (srcStock.LockQuantity ?? 0);
				if (d.TransferQuantity != available)
					return new ReturnData(500,
						$"物料【{material.MaterialName}】的数量必须跟随库存行（可用 {available}），一条库存一个条码，不能搬半条！");

				// 条码记到明细上，移入时带到调入仓：容器还是那个容器，只是换了仓库
				d.Barcode = srcStock.Barcode;

				// 开单阶段不锁死目标库位，收货时由调入仓现场定
				if (d.OutQuantity == null) d.OutQuantity = 0;
				if (d.InQuantity == null) d.InQuantity = 0;
				if (d.InTransitQuantity == null) d.InTransitQuantity = 0;
				if (string.IsNullOrWhiteSpace(d.TransferStatus)) d.TransferStatus = "pending";
			}

			model.TotalQuantity = model.Details.Sum(p => p.TransferQuantity);
			if (model.OutQuantity == null) model.OutQuantity = 0;
			if (model.InQuantity == null) model.InQuantity = 0;
			if (model.InTransitQuantity == null) model.InTransitQuantity = 0;
			// 新建时默认待调拨（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
			if (string.IsNullOrWhiteSpace(model.TransferStatus)) model.TransferStatus = "pending";

			_client.Ado.BeginTran();
			try
			{
				if (!_transferService.Save(model))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存调拨单失败！");
				}

				// 编辑时先软删旧明细再重建，避免残留删除行
				if (model.TransferId > 0)
					_transferService.DeleteDetailsByTransferId(model.TransferId);

				if (!_transferService.BatchSaveDetails(model.TransferId, model.Details))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存调拨明细失败！");
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"保存失败：{ex.Message}");
			}

			return new ReturnData();
		}

		/// <summary>
		/// 移出：调出仓扣减库存、记在途账，由调出仓的人操作。
		/// 一次性移出所有待调拨明细，先整体校验再落账，避免只扣了一半。
		/// </summary>
		public ReturnData TransferOut(long transferId)
		{
			var t = _transferService.GetById(transferId);
			if (t == null) return new ReturnData(500, "调拨单不存在！");
			if (t.TransferStatus == "completed") return new ReturnData(500, "该调拨单已完成！");
			if (t.TransferStatus == "cancelled") return new ReturnData(500, "该调拨单已取消！");
			if (t.TransferStatus == "in_transit") return new ReturnData(500, "该调拨单已移出，等待调入仓收货！");
			if (t.Details == null || t.Details.Count == 0)
				return new ReturnData(500, "调拨单没有明细！");

			var pending = t.Details
				.Where(p => string.IsNullOrWhiteSpace(p.TransferStatus) || p.TransferStatus == "pending")
				.ToList();
			if (pending.Count == 0) return new ReturnData(500, "没有待移出的明细！");

			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			// 先在事务外校验一遍，任何一条不满足就整体不动
			foreach (var d in pending)
			{
				var batch = d.BatchNo ?? "";
				var stock = FindStock(t.FromWarehouseId, d.MaterialId, batch, d.FromLocationId.Value);
				if (stock == null)
					return new ReturnData(500, $"调出库位没有物料【{d.MaterialName}】的库存！");

				var available = (stock.StockQuantity ?? 0)
					- (stock.FreezeQuantity ?? 0) - (stock.LockQuantity ?? 0);
				if (available < d.TransferQuantity)
					return new ReturnData(500,
						$"物料【{d.MaterialName}】调出库位可用库存不足（可用 {available}，需调出 {d.TransferQuantity}）！");
			}

			_client.Ado.BeginTran();
			try
			{
				var seq = 0;
				foreach (var d in pending)
				{
					var batch = d.BatchNo ?? "";
					var qty = d.TransferQuantity;
					var stock = FindStock(t.FromWarehouseId, d.MaterialId, batch, d.FromLocationId.Value);
					if (stock == null)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, $"物料【{d.MaterialName}】的库存已不存在，请刷新重试！");
					}

					var before = stock.StockQuantity ?? 0;
					var after = before - qty;

					_client.Updateable<WmsStockEntity>()
						.SetColumns(p => p.StockQuantity == (p.StockQuantity ?? 0) - qty)
						.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) - qty)
						.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
						.Where(p => p.StockId == stock.StockId)
						.ExecuteCommand();

					seq++;
					if (!_stockRecordService.Save(new WmsStockRecordEntity
					{
						RecordNo = $"{t.TransferNo}-O{seq:00}",
						MaterialId = d.MaterialId,
						BatchNo = d.BatchNo,
						WarehouseId = t.FromWarehouseId,
						ZoneId = d.FromZoneId,
						LocationId = d.FromLocationId,
						BizType = "transfer",
						SourceType = "transfer",
						SourceId = transferId,
						SourceNo = t.TransferNo,
						Direction = "out",
						BeforeQuantity = before,
						ChangeQuantity = qty,
						AfterQuantity = after,
						OperateTime = now,
						Remark = "调拨移出",
					}))
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "写入移出流水失败！");
					}

					// 明细进入在途：已出库 +qty，在途 +qty
					// 条码在此记到明细上：移入是另一批人、另一天的事，到那时源库存行可能已经动过了
					_client.Updateable<WmsTransferDetailEntity>()
						.SetColumns(p => new WmsTransferDetailEntity
						{
							OutQuantity = qty,
							InTransitQuantity = qty,
							TransferStatus = "in_transit",
							Barcode = stock.Barcode,
							OutTime = now,
							UpdateBy = account,
							UpdateTime = now,
						})
						.Where(p => p.DetailId == d.DetailId)
						.ExecuteCommand();
				}

				// 主表汇总：已出库与在途都来自明细，避免两处各算各的
				var all = _transferService.GetDetailsByTransferId(transferId);
				var outQty = all.Sum(p => p.OutQuantity ?? 0);
				var transitQty = all.Sum(p => p.InTransitQuantity ?? 0);

				var ok = _client.Updateable<WmsTransferEntity>()
					.SetColumns(p => new WmsTransferEntity
					{
						OutQuantity = outQty,
						InTransitQuantity = transitQty,
						TransferStatus = "in_transit",
						OutBy = account,
						OutTime = now,
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.TransferId == transferId)
					.ExecuteCommand() > 0;
				if (!ok)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "更新调拨单状态失败！");
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"移出失败：{ex.Message}");
			}

			return new ReturnData(200, $"移出成功，{pending.Count} 条明细已在途，等待调入仓收货");
		}

		/// <summary>
		/// 移入：调入仓按实收数量入账、消在途，由调入仓的人操作。
		/// 允许少收——差额继续留在在途上，不凭空消失也不凭空入库。
		/// </summary>
		public ReturnData TransferIn(TransferInDto dto)
		{
			if (dto == null || dto.TransferId <= 0)
				return new ReturnData(500, "参数有误！");
			if (dto.Items == null || dto.Items.Count == 0)
				return new ReturnData(500, "请填写收货明细！");

			var t = _transferService.GetById(dto.TransferId);
			if (t == null) return new ReturnData(500, "调拨单不存在！");
			if (t.TransferStatus == "pending") return new ReturnData(500, "该调拨单还未移出，请先执行移出！");
			if (t.TransferStatus == "completed") return new ReturnData(500, "该调拨单已完成！");
			if (t.TransferStatus == "cancelled") return new ReturnData(500, "该调拨单已取消！");
			if (t.Details == null || t.Details.Count == 0)
				return new ReturnData(500, "调拨单没有明细！");

			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			// 校验：明细存在、未超收、收货库位属于调入仓
			var plans = new List<KeyValuePair<WmsTransferDetailEntity, TransferInItemDto>>();
			foreach (var item in dto.Items)
			{
				if (item.Quantity <= 0) continue;

				var d = t.Details.FirstOrDefault(p => p.DetailId == item.DetailId);
				if (d == null) return new ReturnData(500, $"明细 {item.DetailId} 不存在！");

				var transit = d.InTransitQuantity ?? 0;
				if (transit <= 0)
					return new ReturnData(500, $"物料【{d.MaterialName}】没有在途数量，无需收货！");
				if (item.Quantity > transit)
					return new ReturnData(500,
						$"物料【{d.MaterialName}】实收 {item.Quantity} 超过在途 {transit}，不能超收！");

				if (!item.ToLocationId.HasValue || item.ToLocationId.Value <= 0)
					return new ReturnData(500, $"请选择物料【{d.MaterialName}】的收货库位！");

				// 库位必须属于调入仓，否则又会出现"库位在B仓、库存记在A仓"的错账
				var wh = GetWarehouseIdOfLocation(item.ToLocationId.Value);
				if (wh == null) return new ReturnData(500, "收货库位不存在！");
				if (wh.Value != t.ToWarehouseId)
					return new ReturnData(500, $"物料【{d.MaterialName}】的收货库位不属于调入仓库！");

				plans.Add(new KeyValuePair<WmsTransferDetailEntity, TransferInItemDto>(d, item));
			}

			if (plans.Count == 0)
				return new ReturnData(500, "没有需要收货的明细！");

			_client.Ado.BeginTran();
			try
			{
				var seq = 0;
				foreach (var plan in plans)
				{
					var d = plan.Key;
					var item = plan.Value;
					var qty = item.Quantity;
					var batch = d.BatchNo ?? "";
					var toLocationId = item.ToLocationId.Value;

					// 有条码的按条码找（不并入调入仓的散货），没条码才按 物料+批次+库位
					var toStock = string.IsNullOrWhiteSpace(d.Barcode)
						? FindStock(t.ToWarehouseId, d.MaterialId, batch, toLocationId)
						: FindStockByBarcode(d.Barcode);
					var before = toStock?.StockQuantity ?? 0;
					var after = before + qty;

					if (toStock != null)
					{
						_client.Updateable<WmsStockEntity>()
							.SetColumns(p => p.StockQuantity == (p.StockQuantity ?? 0) + qty)
							.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) + qty)
							.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
							.Where(p => p.StockId == toStock.StockId)
							.ExecuteCommand();
					}
					else
					{
						// 该库位首次存放这个物料：新建库存行，仓库要写调入仓
						var location = _client.Queryable<WmsLocationEntity>()
							.Where(p => p.LocationId == toLocationId && p.IsDel == 0).First();
						if (location == null)
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "收货库位不存在！");
						}
						var zone = _client.Queryable<WmsZoneEntity>()
							.Where(p => p.ZoneId == location.ZoneId && p.IsDel == 0).First();
						var warehouse = _client.Queryable<WmsWarehouseEntity>()
							.Where(p => p.WarehouseId == t.ToWarehouseId && p.IsDel == 0).First();

						var newStock = new WmsStockEntity
						{
							MaterialId = d.MaterialId,
							MaterialCode = d.MaterialCode,
							MaterialName = d.MaterialName,
							Spec = d.Spec,
							Unit = d.Unit,
							BatchNo = batch,
							WarehouseId = t.ToWarehouseId,
							WarehouseName = warehouse?.WarehouseName,
							ZoneId = zone?.ZoneId,
							ZoneCode = zone?.ZoneCode,
							ZoneName = zone?.ZoneName,
							LocationId = toLocationId,
							LocationCode = location.LocationCode,
							LocationName = location.LocationName,
							// 条码跟着库存走：容器还是那个容器，只是换了仓库
							Barcode = d.Barcode,
							StockQuantity = qty,
						};
						if (!_stockService.AddOrUpdateStock(newStock))
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "目标库位写入库存失败！");
						}
					}

					seq++;
					if (!_stockRecordService.Save(new WmsStockRecordEntity
					{
						RecordNo = $"{t.TransferNo}-I{seq:00}",
						MaterialId = d.MaterialId,
						BatchNo = d.BatchNo,
						WarehouseId = t.ToWarehouseId,
						ZoneId = d.ToZoneId,
						LocationId = toLocationId,
						BizType = "transfer",
						SourceType = "transfer",
						SourceId = t.TransferId,
						SourceNo = t.TransferNo,
						Direction = "in",
						BeforeQuantity = before,
						ChangeQuantity = qty,
						AfterQuantity = after,
						OperateTime = now,
						Remark = "调拨移入",
					}))
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "写入移入流水失败！");
					}

					// 明细：已入库 +qty，在途 -qty；收满才算完成，少收的继续挂在途
					var newIn = (d.InQuantity ?? 0) + qty;
					var newTransit = (d.InTransitQuantity ?? 0) - qty;
					if (newTransit < 0) newTransit = 0;
					var detailStatus = newIn >= d.TransferQuantity ? "completed" : "in_transit";

					_client.Updateable<WmsTransferDetailEntity>()
						.SetColumns(p => new WmsTransferDetailEntity
						{
							InQuantity = newIn,
							InTransitQuantity = newTransit,
							TransferStatus = detailStatus,
							ToZoneId = null,
							ToLocationId = toLocationId,
							InTime = now,
							UpdateBy = account,
							UpdateTime = now,
						})
						.Where(p => p.DetailId == d.DetailId)
						.ExecuteCommand();

					// 回填收货库位编码，列表上能直接看到货放在哪
					var loc = _client.Queryable<WmsLocationEntity>()
						.Where(p => p.LocationId == toLocationId && p.IsDel == 0).First();
					if (loc != null)
					{
						_client.Updateable<WmsTransferDetailEntity>()
							.SetColumns(p => new WmsTransferDetailEntity
							{
								ToLocationCode = loc.LocationCode,
								ToZoneId = loc.ZoneId,
							})
							.Where(p => p.DetailId == d.DetailId)
							.ExecuteCommand();
					}
				}

				// 主表：全部明细收齐才算完成，否则仍在途
				var all = _transferService.GetDetailsByTransferId(t.TransferId);
				var inQty = all.Sum(p => p.InQuantity ?? 0);
				var transitQty = all.Sum(p => p.InTransitQuantity ?? 0);
				var finished = all.Count > 0 && all.All(p => p.TransferStatus == "completed");

				var ok = _client.Updateable<WmsTransferEntity>()
					.SetColumns(p => new WmsTransferEntity
					{
						InQuantity = inQty,
						InTransitQuantity = transitQty,
						TransferStatus = finished ? "completed" : "in_transit",
						InBy = account,
						InTime = now,
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.TransferId == t.TransferId)
					.ExecuteCommand() > 0;
				if (!ok)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "更新调拨单状态失败！");
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"移入失败：{ex.Message}");
			}

			var rest = _transferService.GetDetailsByTransferId(t.TransferId)
				.Sum(p => p.InTransitQuantity ?? 0);

			return new ReturnData(200, rest > 0
				? $"收货完成，仍有 {rest} 在途未入库（少收部分已记为在途）"
				: "收货完成，调拨单已完成");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			// 已作业的单据不能删：库存与在途账已变动，删了就丢了凭证
			foreach (var id in delIds)
			{
				var exist = _transferService.GetById(id);
				if (exist == null) continue;
				if (exist.TransferStatus == "in_transit")
					return new ReturnData(500, $"调拨单 {exist.TransferNo} 已移出（在途），不能删除！");
				if (exist.TransferStatus == "completed")
					return new ReturnData(500, $"调拨单 {exist.TransferNo} 已完成，不能删除！");
			}

			_client.Ado.BeginTran();
			try
			{
				foreach (var id in delIds)
					_transferService.DeleteDetailsByTransferId(id);

				if (!_transferService.Delete(delIds))
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

		/// <summary>库位属于哪个仓库（库位 → 库区 → 仓库）</summary>
		private long? GetWarehouseIdOfLocation(long locationId)
		{
			var loc = _client.Queryable<WmsLocationEntity>()
				.Where(p => p.LocationId == locationId && p.IsDel == 0).First();
			if (loc == null) return null;
			var zone = _client.Queryable<WmsZoneEntity>()
				.Where(p => p.ZoneId == loc.ZoneId && p.IsDel == 0).First();
			return zone?.WarehouseId;
		}

		/// <summary>定位库存行：仓库 + 物料 + 批次 + 库位（跨仓必须带仓库，否则会串仓）</summary>
		/// <summary>按条码定位库存行（有条码的库存是一个独立单元，不参与合并）</summary>
		private WmsStockEntity FindStockByBarcode(string barcode)
		{
			if (string.IsNullOrWhiteSpace(barcode)) return null;
			return _client.Queryable<WmsStockEntity>()
				.Where(p => p.Barcode == barcode && p.IsDel == 0).First();
		}

		private WmsStockEntity FindStock(long warehouseId, long materialId, string batchNo, long locationId)
		{
			var batch = batchNo ?? "";
			return _client.Queryable<WmsStockEntity>()
				.Where(p => p.IsDel == 0 && p.WarehouseId == warehouseId
					&& p.MaterialId == materialId && p.LocationId == locationId && p.HuId == null)
				.Where(p => p.BatchNo == batch || (p.BatchNo == null && batch == ""))
				.First();
		}
	}
}
