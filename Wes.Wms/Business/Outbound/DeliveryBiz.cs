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
	/// 出库单 业务逻辑
	/// </summary>
	public class DeliveryBiz : IDeliveryBiz
	{
		private readonly IDeliveryService _deliveryService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;
		private readonly IStockRecordService _stockRecordService;
		private readonly ISqlSugarClient _client;
		private readonly IStockAllocaterBiz _allocaterBiz;
		/// <summary>回写上游来源单据用（注入数据层而非 Biz，避免与出库申请单 Biz 形成循环依赖）</summary>
		private readonly IIssueOrderService _issueService;

		public DeliveryBiz(IDeliveryService deliveryService, ISysCodeRuleBiz codeRuleBiz,
			IStockRecordService stockRecordService, ISqlSugarClient sqlSugarClient,
			IStockAllocaterBiz allocaterBiz, IIssueOrderService issueService)
		{
			_deliveryService = deliveryService;
			_codeRuleBiz = codeRuleBiz;
			_stockRecordService = stockRecordService;
			_client = sqlSugarClient;
			_allocaterBiz = allocaterBiz;
			_issueService = issueService;
		}

		public RowData<WmsDeliveryEntity> GetList(ParamData<WmsDeliveryParam> param)
		{
			int total = 0;
			var result = new RowData<WmsDeliveryEntity>(_deliveryService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsDeliveryEntity> GetById(long id)
		{
			return new ResultData<WmsDeliveryEntity>(_deliveryService.GetById(id));
		}

		public ReturnData Save(WmsDeliveryEntity model)
		{
			// 校验明细
			if (model.Details == null || model.Details.Count == 0)
				return new ReturnData(500, "请至少添加一条出库明细！");

			foreach (var d in model.Details)
			{
				if (d.MaterialId <= 0)
					return new ReturnData(500, "请选择出库物料！");
				if (d.DeliveryQuantity <= 0)
					return new ReturnData(500, "出库数量必须大于0！");
			}

			// 新增且未传编码时，按规则自动生成单据编码
			if (model.DeliveryId == 0 && string.IsNullOrWhiteSpace(model.DeliveryNo))
			{
				string err;
				model.DeliveryNo = _codeRuleBiz.GetCode("WMS_DELIVERY", out err);
				if (!string.IsNullOrWhiteSpace(err))
					return new ReturnData(500, err);
			}

			// 新建时默认待出库（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
			if (string.IsNullOrWhiteSpace(model.DeliveryStatus))
				model.DeliveryStatus = "pending";

			_client.Ado.BeginTran();
			try
			{
				// 保存主表
				if (!_deliveryService.Save(model))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存出库单失败！");
				}

				// 编辑时：删除旧明细
				if (model.DeliveryId > 0)
					_deliveryService.DeleteDetailsByDeliveryId(model.DeliveryId);

				// 插入新明细
				_deliveryService.BatchSaveDetails(model.DeliveryId, model.Details);

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
		/// 执行出库（发货）：扣减库存可用量 + 写库存流水流水（direction=out）。
		/// 明细指定了库位就从该库位扣；未指定则在本仓库内按库存行创建时间顺序（先进先出）依次扣。
		/// </summary>
		public ReturnData Execute(long deliveryId)
		{
			var delivery = _deliveryService.GetById(deliveryId);
			if (delivery == null) return new ReturnData(500, "出库单不存在！");
			if (delivery.DeliveryStatus == "shipped" || delivery.DeliveryStatus == "delivered")
				return new ReturnData(500, "该出库单已出库，不能重复执行！");
			if (delivery.Details == null || delivery.Details.Count == 0)
				return new ReturnData(500, "出库单没有明细，不能执行！");

			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			_client.Ado.BeginTran();
			try
			{
				var seq = 0;

				foreach (var d in delivery.Details)
				{
					if (d.MaterialId <= 0)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "出库明细缺少物料！");
					}
					if (d.DeliveryQuantity <= 0)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "出库数量必须大于0！");
					}

					var remain = (decimal)d.DeliveryQuantity;
					var batchNo = d.BatchNo ?? "";

					// 挑哪一批货统一走分配器：FEFO（先过期先出）→ FIFO（先进先出），
					// 并按出库单的仓库过滤，避免扣到别的仓库头上
					var allocItems = _allocaterBiz.Allocate(
						delivery.WarehouseId,
						d.MaterialId,
						d.DeliveryQuantity,
						d.LocationId.HasValue && d.LocationId.Value > 0 ? d.LocationId : null,
						string.IsNullOrWhiteSpace(batchNo) ? null : batchNo);

					if (allocItems.Count == 0)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500,
							$"物料 id【{d.MaterialId}】没有可用库存（库存不足或已被冻结/锁定），无法出库！");
					}

					foreach (var alloc in allocItems)
					{
						if (remain <= 0) break;

						// 分配器已按 FEFO/FIFO 排好序并算出每行可扣数量，这里只负责落地
						var row = alloc.Stock;
						var take = alloc.Quantity > remain ? (int)remain : alloc.Quantity;
						if (take <= 0) continue;

						var stockQty = row.StockQuantity ?? 0;
						var before = stockQty;
						var after = before - take;

						// 出库消耗预留时，同步释放该行的锁定数量（先扣锁定再扣普通可用）
						var lockQty = row.LockQuantity ?? 0;
						var consumeLock = lockQty < take ? lockQty : take;

						_client.Updateable<WmsStockEntity>()
							.SetColumns(p => p.StockQuantity == (p.StockQuantity ?? 0) - take)
							.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) - take)
							.SetColumns(p => p.LockQuantity == (p.LockQuantity ?? 0) - consumeLock)
							.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
							.Where(p => p.StockId == row.StockId)
							.ExecuteCommand();

						// 核销锁定单：出库真实消耗了锁定量，必须回写锁定单的已用量，
						// 否则释放时会把已出库的量重复退还可用量（先锁先核销）
						if (consumeLock > 0)
						{
							var lockOrders = _client.Queryable<WmsLockEntity>()
								.Where(p => p.IsDel == 0 && p.StockId == row.StockId
									&& (p.LockStatus == "locked" || p.LockStatus == "partial"))
								.OrderBy(p => p.LockTime, OrderByType.Asc).ToList();
							if (lockOrders.Count == 0)
							{
								// 兼容没有库存行的老锁定单
								lockOrders = _client.Queryable<WmsLockEntity>()
									.Where(p => p.IsDel == 0 && p.StockId == null && p.MaterialId == d.MaterialId
										&& p.LocationId == row.LocationId
										&& (p.LockStatus == "locked" || p.LockStatus == "partial"))
									.OrderBy(p => p.LockTime, OrderByType.Asc).ToList();
							}
							var needWriteOff = consumeLock;
							foreach (var lk in lockOrders)
							{
								if (needWriteOff <= 0) break;
								var used = lk.UsedQuantity ?? 0;
								var lockRemain = lk.LockQuantity - used;
								if (lockRemain <= 0) continue;
								var writeOff = lockRemain < needWriteOff ? lockRemain : needWriteOff;
								var newUsed = used + writeOff;
								var newStatus = newUsed >= lk.LockQuantity ? "completed" : "partial";
								_client.Updateable<WmsLockEntity>()
									.SetColumns(p => p.UsedQuantity == newUsed)
									.SetColumns(p => p.LockStatus == newStatus)
									.SetColumns(p => new WmsLockEntity { UpdateBy = account, UpdateTime = now })
									.Where(p => p.LockId == lk.LockId)
									.ExecuteCommand();
								needWriteOff -= writeOff;
							}
						}

						seq++;
						if (!_stockRecordService.Save(new WmsStockRecordEntity
						{
							RecordNo = $"{delivery.DeliveryNo}-{seq:000}",
							MaterialId = d.MaterialId,
							BatchNo = row.BatchNo,
							WarehouseId = row.WarehouseId,
							ZoneId = row.ZoneId,
							LocationId = row.LocationId,
							BizType = "delivery",
							SourceType = "delivery",
							SourceId = deliveryId,
							SourceNo = delivery.DeliveryNo,
							Direction = "out",
							BeforeQuantity = before,
							ChangeQuantity = take,
							AfterQuantity = after,
							UnitPrice = d.UnitPrice,
							Amount = d.UnitPrice.HasValue ? take * d.UnitPrice.Value : null,
							OperateTime = now,
							Remark = "出库发货",
						}))
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "写入出库流水失败！");
						}

						remain -= take;
					}

					if (remain > 0)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500,
							$"可用库存不足（还需出库 {remain}，可用量已被冻结或锁定）！");
					}
				}

				var updated = _client.Updateable<WmsDeliveryEntity>()
					.SetColumns(p => new WmsDeliveryEntity
					{
						DeliveryStatus = "shipped",
						ShipTime = now,
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.DeliveryId == deliveryId)
					.ExecuteCommand() > 0;
				if (!updated)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "更新出库单状态失败！");
				}

				// 回写上游来源单据的进度与状态
				var writeBack = WriteBackSource(delivery, account, now);
				if (writeBack.Code != 200)
				{
					_client.Ado.RollbackTran();
					return writeBack;
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

		/// <summary>
		/// 出库完成后回写上游来源单据的进度与状态。
		/// 一张来源单据允许对应多个出库单（领料的部分发料场景），所以这里是**累加**而非覆盖。
		/// </summary>
		private ReturnData WriteBackSource(WmsDeliveryEntity delivery, string account, DateTime now)
		{
			// 手工出库单没有来源单据，无需回写
			if (string.IsNullOrWhiteSpace(delivery.SourceType) || !delivery.SourceId.HasValue || delivery.SourceId.Value <= 0)
				return new ReturnData();

			switch (delivery.SourceType)
			{
				case "issue":
					return WriteBackIssue(delivery, account, now);
				default:
					return new ReturnData();
			}
		}

		/// <summary>领料单：累加明细已发数量，重算欠料并推导发料状态</summary>
		private ReturnData WriteBackIssue(WmsDeliveryEntity delivery, string account, DateTime now)
		{
			var issueId = delivery.SourceId.Value;
			var details = _issueService.GetDetailsByIssueId(issueId);
			if (details == null || details.Count == 0)
				return new ReturnData(500, "来源领料单没有明细，无法回写！");

			foreach (var d in delivery.Details)
			{
				var left = d.DeliveryQuantity;
				if (left <= 0) continue;

				// 同一物料可能拆成多行明细，按未发完的顺序依次累加
				var targets = details
					.Where(p => p.MaterialId == d.MaterialId && (p.IssuedQuantity ?? 0) < (p.RequireQuantity ?? 0))
					.OrderBy(p => p.DetailId)
					.ToList();

				foreach (var t in targets)
				{
					if (left <= 0) break;
					var lack = (t.RequireQuantity ?? 0) - (t.IssuedQuantity ?? 0);
					var add = lack < left ? lack : left;

					t.IssuedQuantity = (t.IssuedQuantity ?? 0) + add;
					t.ShortageQuantity = (t.RequireQuantity ?? 0) - t.IssuedQuantity;
					t.UpdateBy = account;
					t.UpdateTime = now;

					if (!(_client.Updateable(t).ExecuteCommand() > 0))
						return new ReturnData(500, "回写领料单明细失败！");

					left -= add;
				}
			}

			// 明细对象已在上面累加，直接据其汇总
			var require = details.Sum(p => p.RequireQuantity ?? 0);
			var issued = details.Sum(p => p.IssuedQuantity ?? 0);
			var shortage = require - issued;
			// 一点没发 pending，发了部分 partial，全部发完 issued
			var status = issued <= 0 ? "pending" : (shortage > 0 ? "partial" : "issued");

			var ok = _client.Updateable<WmsIssueOrderEntity>()
				.SetColumns(p => new WmsIssueOrderEntity
				{
					TotalQuantity = require,
					IssuedQuantity = issued,
					ShortageQuantity = shortage,
					IssueStatus = status,
					UpdateBy = account,
					UpdateTime = now,
				})
				.Where(p => p.IssueId == issueId)
				.ExecuteCommand() > 0;

			return ok ? new ReturnData() : new ReturnData(500, "更新领料单状态失败！");
		}



		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			// 已出库的单据不能删除（库存已扣减，删除会丢失凭证）
			foreach (var id in delIds)
			{
				var exist = _deliveryService.GetById(id);
				if (exist != null && (exist.DeliveryStatus == "shipped" || exist.DeliveryStatus == "delivered"))
					return new ReturnData(500, $"出库单 {exist.DeliveryNo} 已出库，不能删除！");
			}

			_client.Ado.BeginTran();
			try
			{
				// 级联软删除明细
				foreach (var id in delIds)
					_deliveryService.DeleteDetailsByDeliveryId(id);

				// 软删除主表
				if (!_deliveryService.Delete(delIds))
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
	}
}
