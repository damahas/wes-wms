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
	/// 移库单 业务逻辑
	/// </summary>
	public class MoveBiz : IMoveBiz
	{
		private readonly IMoveService _moveService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;
		private readonly IStockService _stockService;
		private readonly IStockRecordService _stockRecordService;
		private readonly ISqlSugarClient _client;

		public MoveBiz(IMoveService moveService, ISysCodeRuleBiz codeRuleBiz,
			IStockService stockService, IStockRecordService stockRecordService,
			ISqlSugarClient sqlSugarClient)
		{
			_moveService = moveService;
			_codeRuleBiz = codeRuleBiz;
			_stockService = stockService;
			_stockRecordService = stockRecordService;
			_client = sqlSugarClient;
		}

		public RowData<WmsMoveEntity> GetList(ParamData<WmsMoveParam> param)
		{
			int total = 0;
			var result = new RowData<WmsMoveEntity>(_moveService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsMoveEntity> GetById(long id)
		{
			return new ResultData<WmsMoveEntity>(_moveService.GetById(id));
		}

		public ReturnData Save(WmsMoveEntity model)
		{
			// 已执行的移库单不允许修改（库存已变动、账务已记）
			if (model.MoveId > 0)
			{
				var exist = _moveService.GetById(model.MoveId);
				if (exist == null) return new ReturnData(500, "移库单不存在！");
				if (exist.MoveStatus == "completed")
					return new ReturnData(500, "该移库单已执行，不能修改！");
			}

			if (model.WarehouseId <= 0)
				return new ReturnData(500, "请选择仓库！");

			// 校验明细
			if (model.Details == null || model.Details.Count == 0)
				return new ReturnData(500, "请至少添加一条移库明细！");

			// 移库单只管库内：跨仓走调拨单。
			// 否则执行时目标库存行会记成源仓，出现"库位在B仓、库存却记在A仓"的错账。
			foreach (var d in model.Details)
			{
				if (d.MaterialId <= 0)
					return new ReturnData(500, "请选择移库物料！");
				if (d.MoveQuantity <= 0)
					return new ReturnData(500, "移库数量必须大于0！");
				if (d.FromLocationId <= 0 || d.ToLocationId <= 0)
					return new ReturnData(500, "每条明细都要选择移出库位和移入库位！");
				if (d.FromLocationId == d.ToLocationId)
					return new ReturnData(500, "移出库位与移入库位不能相同！");

				var fromWh = GetWarehouseIdOfLocation(d.FromLocationId);
				if (fromWh == null) return new ReturnData(500, "移出库位不存在！");
				var toWh = GetWarehouseIdOfLocation(d.ToLocationId);
				if (toWh == null) return new ReturnData(500, "移入库位不存在！");
				if (fromWh.Value != toWh.Value)
					return new ReturnData(500, "移出库位与移入库位不在同一仓库，跨仓调拨请使用调拨单！");
				if (fromWh.Value != model.WarehouseId)
					return new ReturnData(500, "所选库位不属于该移库单的仓库！");

				// 数量必须等于库存行的可用量：一条库存一个条码，搬半条的话标签就对不上实物了。
				// 后端自己按库存行算，不采信前端传来的数量。
				var srcStock = FindStock(d.MaterialId, d.BatchNo ?? "", d.FromLocationId);
				if (srcStock == null)
					return new ReturnData(500, "移出库位没有该物料的库存！");

				var available = (srcStock.StockQuantity ?? 0)
					- (srcStock.FreezeQuantity ?? 0) - (srcStock.LockQuantity ?? 0);
				if (d.MoveQuantity != Math.Floor(available))
					return new ReturnData(500,
						$"物料【{d.MaterialName ?? srcStock.MaterialName}】的数量必须跟随库存行（可用 {available}），一条库存一个条码，不能搬半条！");
			}

			// 新增且未传编码时，按规则自动生成单据编码
			if (model.MoveId == 0 && string.IsNullOrWhiteSpace(model.MoveNo))
			{
				string err;
				model.MoveNo = _codeRuleBiz.GetCode("WMS_MOVE", out err);
				if (!string.IsNullOrWhiteSpace(err))
					return new ReturnData(500, err);
			}

			// 补全冗余编码：库区/库位编码随 id 写入，列表与执行时不必再回查
			foreach (var d in model.Details)
			{
				var material = _client.Queryable<WmsMaterialEntity>()
					.Where(p => p.MaterialId == d.MaterialId && p.IsDel == 0).First();
				d.MaterialCode = material?.MaterialCode;
				d.MaterialName = material?.MaterialName;

				var fromLoc = _client.Queryable<WmsLocationEntity>()
					.Where(p => p.LocationId == d.FromLocationId && p.IsDel == 0).First();
				if (fromLoc == null) return new ReturnData(500, "移出库位不存在！");
				d.FromLocationCode = fromLoc.LocationCode;
				if (d.FromZoneId <= 0) d.FromZoneId = fromLoc.ZoneId;
				var fromZone = d.FromZoneId > 0
					? _client.Queryable<WmsZoneEntity>().Where(p => p.ZoneId == d.FromZoneId && p.IsDel == 0).First()
					: null;
				d.FromZoneCode = fromZone?.ZoneCode;

				var toLoc = _client.Queryable<WmsLocationEntity>()
					.Where(p => p.LocationId == d.ToLocationId && p.IsDel == 0).First();
				if (toLoc == null) return new ReturnData(500, "移入库位不存在！");
				d.ToLocationCode = toLoc.LocationCode;
				if (d.ToZoneId <= 0) d.ToZoneId = toLoc.ZoneId;
				var toZone = d.ToZoneId > 0
					? _client.Queryable<WmsZoneEntity>().Where(p => p.ZoneId == d.ToZoneId && p.IsDel == 0).First()
					: null;
				d.ToZoneCode = toZone?.ZoneCode;

				if (string.IsNullOrWhiteSpace(d.MoveStatus)) d.MoveStatus = "pending";
			}

			// 总数量 = 明细数量合计
			model.TotalQuantity = (int)model.Details.Sum(p => p.MoveQuantity);
			// 新建时默认待移库（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
			if (string.IsNullOrWhiteSpace(model.MoveStatus)) model.MoveStatus = "pending";

			_client.Ado.BeginTran();
			try
			{
				// 保存主表
				if (!_moveService.Save(model))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存移库单失败！");
				}

				// 编辑时：删除旧明细
				if (model.MoveId > 0)
					_moveService.DeleteDetailsByMoveId(model.MoveId);

				// 插入新明细
				_moveService.BatchSaveDetails(model.MoveId, model.Details);

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
		/// 执行移库：源库位扣减、目标库位增加，同一事务内写两条流水（移出 out / 移入 in）。
		/// 移库不改变全仓库存总量，但库位维度变了，所以按库位各记一条。
		/// </summary>
		public ReturnData Execute(long moveId)
		{
			var move = _moveService.GetById(moveId);
			if (move == null) return new ReturnData(500, "移库单不存在！");
			if (move.MoveStatus == "completed") return new ReturnData(500, "该移库单已执行，不能重复执行！");
			if (move.Details == null || move.Details.Count == 0)
				return new ReturnData(500, "移库单没有明细，不能执行！");

			foreach (var d in move.Details)
			{
				if (d.FromLocationId <= 0 || d.ToLocationId <= 0)
					return new ReturnData(500, "移库明细缺少源库位或目标库位！");
				if (d.FromLocationId == d.ToLocationId)
					return new ReturnData(500, "源库位与目标库位不能相同！");
				if (d.MoveQuantity <= 0)
					return new ReturnData(500, "移库数量必须大于0！");

				// 执行前再挡一次：历史单据可能是在加校验之前建的，
				// 跨仓的单据一旦执行就会记出错账，宁可拦下让人走调拨单
				var fromWh = GetWarehouseIdOfLocation(d.FromLocationId);
				var toWh = GetWarehouseIdOfLocation(d.ToLocationId);
				if (fromWh == null || toWh == null)
					return new ReturnData(500, "移出库位或移入库位不存在！");
				if (fromWh.Value != toWh.Value)
					return new ReturnData(500, "源库位与目标库位不在同一仓库，跨仓调拨请使用调拨单！");
			}

			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			_client.Ado.BeginTran();
			try
			{
				var seq = 0;

				foreach (var d in move.Details)
				{
					var qty = d.MoveQuantity;
					var batchNo = d.BatchNo ?? "";

					// ---------- 源库位：扣减 ----------
					var fromStock = FindStock(d.MaterialId, batchNo, d.FromLocationId);
					if (fromStock == null)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "源库位没有该物料的库存！");
					}

					var fromBefore = fromStock.StockQuantity ?? 0;
					if (fromBefore < qty)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, $"源库位库存不足（现有 {fromBefore}，需移出 {qty}）！");
					}

					var fromAvailable = fromBefore - (fromStock.FreezeQuantity ?? 0) - (fromStock.LockQuantity ?? 0);
					if (fromAvailable < qty)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, $"源库位可用库存不足（可用 {fromAvailable}，需移出 {qty}）！");
					}

					_client.Updateable<WmsStockEntity>()
						.SetColumns(p => p.StockQuantity == (p.StockQuantity ?? 0) - qty)
						.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) - qty)
						.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
						.Where(p => p.StockId == fromStock.StockId)
						.ExecuteCommand();

					var fromAfter = fromBefore - qty;
					seq++;
					if (!_stockRecordService.Save(new WmsStockRecordEntity
					{
						RecordNo = $"{move.MoveNo}-{seq:000}",
						MaterialId = d.MaterialId,
						BatchNo = d.BatchNo,
						WarehouseId = move.WarehouseId,
						ZoneId = d.FromZoneId,
						LocationId = d.FromLocationId,
						BizType = "move",
						SourceType = "move",
						SourceId = moveId,
						SourceNo = move.MoveNo,
						Direction = "out",
						BeforeQuantity = fromBefore,
						ChangeQuantity = qty,
						AfterQuantity = fromAfter,
						OperateTime = now,
						Remark = "移出",
					}))
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "写入移出流水失败！");
					}

					// ---------- 目标库位：增加 ----------
					// 有条码的库存是独立的一坨，不能并进目标库位上同物料同批次的散货里，得按条码找
					var toStock = string.IsNullOrWhiteSpace(fromStock.Barcode)
						? FindStock(d.MaterialId, batchNo, d.ToLocationId)
						: FindStockByBarcode(fromStock.Barcode);
					var toBefore = toStock?.StockQuantity ?? 0;

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
						var location = _client.Queryable<WmsLocationEntity>()
							.Where(p => p.LocationId == d.ToLocationId && p.IsDel == 0).First();
						if (location == null)
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "目标库位不存在！");
						}

						var material = _client.Queryable<WmsMaterialEntity>()
							.Where(p => p.MaterialId == d.MaterialId && p.IsDel == 0).First();
						var warehouse = _client.Queryable<WmsWarehouseEntity>()
							.Where(p => p.WarehouseId == move.WarehouseId && p.IsDel == 0).First();
						var zone = d.ToZoneId > 0
							? _client.Queryable<WmsZoneEntity>().Where(p => p.ZoneId == d.ToZoneId && p.IsDel == 0).First()
							: null;

						var newStock = new WmsStockEntity
						{
							MaterialId = d.MaterialId,
							MaterialCode = material?.MaterialCode,
							MaterialName = material?.MaterialName,
							Spec = material?.Spec,
							Unit = material?.Unit,
							BatchNo = batchNo,
							WarehouseId = move.WarehouseId,
							WarehouseName = warehouse?.WarehouseName,
							ZoneId = d.ToZoneId > 0 ? d.ToZoneId : null,
							ZoneCode = zone?.ZoneCode,
							ZoneName = zone?.ZoneName,
							LocationId = d.ToLocationId,
							LocationCode = location.LocationCode,
							LocationName = location.LocationName,
							// 条码跟着库存走：容器还是那个容器，只是换了地方
							Barcode = fromStock.Barcode,
							StockQuantity = qty,
						};
						if (!_stockService.AddOrUpdateStock(newStock))
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "目标库位写入库存失败！");
						}
					}

					var toAfter = toBefore + qty;
					seq++;
					if (!_stockRecordService.Save(new WmsStockRecordEntity
					{
						RecordNo = $"{move.MoveNo}-{seq:000}",
						MaterialId = d.MaterialId,
						BatchNo = d.BatchNo,
						WarehouseId = move.WarehouseId,
						ZoneId = d.ToZoneId > 0 ? d.ToZoneId : null,
						LocationId = d.ToLocationId,
						BizType = "move",
						SourceType = "move",
						SourceId = moveId,
						SourceNo = move.MoveNo,
						Direction = "in",
						BeforeQuantity = toBefore,
						ChangeQuantity = qty,
						AfterQuantity = toAfter,
						OperateTime = now,
						Remark = "移入",
					}))
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "写入移入流水失败！");
					}

					// 明细置为已完成
					_client.Updateable<WmsMoveDetailEntity>()
						.SetColumns(p => new WmsMoveDetailEntity
						{
							MoveStatus = "completed",
							MoveTime = now,
							UpdateBy = account,
							UpdateTime = now,
						})
						.Where(p => p.DetailId == d.DetailId)
						.ExecuteCommand();
				}

				// 主表置为已完成（此状态防止重复执行）
				var totalQty = (int)move.Details.Sum(p => p.MoveQuantity);
				var updated = _client.Updateable<WmsMoveEntity>()
					.SetColumns(p => new WmsMoveEntity
					{
						MoveStatus = "completed",
						TotalQuantity = totalQty,
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.MoveId == moveId)
					.ExecuteCommand() > 0;
				if (!updated)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "更新移库单状态失败！");
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"执行失败：{ex.Message}");
			}

			return new ReturnData();
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			// 已执行的移库单不能删除（库存已变动，删除会丢失凭证）
			foreach (var id in delIds)
			{
				var exist = _moveService.GetById(id);
				if (exist != null && exist.MoveStatus == "completed")
					return new ReturnData(500, $"移库单 {exist.MoveNo} 已执行，不能删除！");
			}

			_client.Ado.BeginTran();
			try
			{
				// 级联软删除明细
				foreach (var id in delIds)
					_moveService.DeleteDetailsByMoveId(id);

				// 软删除主表
				if (!_moveService.Delete(delIds))
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

		/// <summary>按条码定位库存行（有条码的库存是一个独立单元，不参与合并）</summary>
		private WmsStockEntity FindStockByBarcode(string barcode)
		{
			if (string.IsNullOrWhiteSpace(barcode)) return null;
			return _client.Queryable<WmsStockEntity>()
				.Where(p => p.Barcode == barcode && p.IsDel == 0).First();
		}

		/// <summary>定位库存行：同物料 + 批次 + 库位（无处理单元）</summary>
		private WmsStockEntity FindStock(long materialId, string batchNo, long locationId)
		{
			var batch = batchNo ?? "";
			return _client.Queryable<WmsStockEntity>()
				.Where(p => p.IsDel == 0 && p.MaterialId == materialId
					&& p.LocationId == locationId && p.HuId == null)
				.Where(p => p.BatchNo == batch || (p.BatchNo == null && batch == ""))
				.First();
		}
	}
}
