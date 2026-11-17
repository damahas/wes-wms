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
	/// 库存调整 业务逻辑
	/// 库存不能直接改，只能通过调整单执行：事务内同时更新 wms_stock + 写 wms_stock_record
	/// </summary>
	public class AdjustBiz : IAdjustBiz
	{
		private readonly IAdjustService _adjustService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;
		private readonly IStockService _stockService;
		private readonly IStockRecordService _stockRecordService;
		private readonly ISqlSugarClient _client;

		public AdjustBiz(IAdjustService adjustService, ISysCodeRuleBiz codeRuleBiz,
			IStockService stockService, IStockRecordService stockRecordService,
			ISqlSugarClient sqlSugarClient)
		{
			_adjustService = adjustService;
			_codeRuleBiz = codeRuleBiz;
			_stockService = stockService;
			_stockRecordService = stockRecordService;
			_client = sqlSugarClient;
		}

		public RowData<WmsAdjustEntity> GetList(ParamData<WmsAdjustParam> param)
		{
			int total = 0;
			var result = new RowData<WmsAdjustEntity>(_adjustService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsAdjustEntity> GetById(long id)
		{
			return new ResultData<WmsAdjustEntity>(_adjustService.GetById(id));
		}

		public ReturnData Save(WmsAdjustEntity model)
		{
			// 校验明细
			if (model.Details == null || model.Details.Count == 0)
				return new ReturnData(500, "请至少添加一条调整明细！");

			foreach (var d in model.Details)
			{
				if (d.MaterialId <= 0)
					return new ReturnData(500, "请选择调整物料！");
				if (d.AdjustQuantity == 0)
					return new ReturnData(500, "调整数量不能为0！");
			}

			// 已执行的单据不允许再改
			if (model.AdjustId > 0)
			{
				var exist = _adjustService.GetById(model.AdjustId);
				if (exist == null) return new ReturnData(500, "调整单不存在！");
				if (exist.AdjustStatus == "completed")
					return new ReturnData(500, "该调整单已执行，不能修改！");
			}

			// 新增且未传编码时，按规则自动生成单据编码
			if (model.AdjustId == 0 && string.IsNullOrWhiteSpace(model.AdjustNo))
			{
				string err;
				model.AdjustNo = _codeRuleBiz.GetCode("WMS_ADJUST", out err);
				if (!string.IsNullOrWhiteSpace(err))
					return new ReturnData(500, err);
			}

			if (string.IsNullOrWhiteSpace(model.AdjustStatus))
				model.AdjustStatus = "pending";

			_client.Ado.BeginTran();
			try
			{
				// 保存主表
				if (!_adjustService.Save(model))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存库存调整单失败！");
				}

				// 编辑时：删除旧明细
				if (model.AdjustId > 0)
					_adjustService.DeleteDetailsByAdjustId(model.AdjustId);

				// 插入新明细
				_adjustService.BatchSaveDetails(model.AdjustId, model.Details);

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
		/// 执行调整单：逐条明细更新实时库存并写库存流水，单据置为已完成。
		/// 调整前/后数量按库存当前实际值回填，避免手工填写与实际库存不符。
		/// </summary>
		public ReturnData Execute(long adjustId)
		{
			var adjust = _adjustService.GetById(adjustId);
			if (adjust == null) return new ReturnData(500, "调整单不存在！");
			if (adjust.AdjustStatus == "completed") return new ReturnData(500, "该调整单已执行，不能重复执行！");
			if (adjust.WarehouseId <= 0) return new ReturnData(500, "请先选择仓库！");

			var details = adjust.Details ?? new List<WmsAdjustDetailEntity>();
			if (details.Count == 0) return new ReturnData(500, "调整单没有明细，不能执行！");

			foreach (var d in details)
			{
				if (d.MaterialId <= 0) return new ReturnData(500, "调整明细缺少物料！");
				if (d.AdjustQuantity == 0) return new ReturnData(500, "调整数量不能为0！");
				if (!d.LocationId.HasValue || d.LocationId.Value <= 0)
					return new ReturnData(500, "调整明细缺少库位！");
			}

			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			_client.Ado.BeginTran();
			try
			{
				var seq = 0;

				foreach (var d in details)
				{
					var location = _client.Queryable<WmsLocationEntity>()
						.Where(p => p.LocationId == d.LocationId.Value && p.IsDel == 0).First();
					if (location == null)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "调整明细中的库位不存在！");
					}

					var material = _client.Queryable<WmsMaterialEntity>()
						.Where(p => p.MaterialId == d.MaterialId && p.IsDel == 0).First();
					if (material == null)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "调整明细中的物料不存在！");
					}

					var zoneId = d.ZoneId ?? location.ZoneId;
					var batchNo = d.BatchNo ?? "";

					// 定位库存行：明细带 stock_id 就精确到行（同物料同批次可能有多条码行，不能猜）；
					// 不带（盘盈新增）则要求该库位无同物料同批次库存，避免误并入已有行
					WmsStockEntity stock = null;
					if (d.StockId.HasValue && d.StockId.Value > 0)
					{
						stock = _client.Queryable<WmsStockEntity>()
							.Where(p => p.StockId == d.StockId.Value && p.IsDel == 0).First();
						if (stock == null)
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "调整明细指向的库存行不存在（可能已被其他单据处理），请重新核单！");
						}
						if (stock.MaterialId != d.MaterialId || stock.LocationId != d.LocationId)
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "调整明细与库存行不匹配（物料或库位对不上），请重新核单！");
						}
					}
					else if (d.AdjustQuantity > 0)
					{
						var dup = _client.Queryable<WmsStockEntity>()
							.Where(p => p.IsDel == 0 && p.MaterialId == d.MaterialId
								&& p.LocationId == d.LocationId.Value && p.HuId == null)
							.Where(p => p.BatchNo == batchNo || (p.BatchNo == null && batchNo == ""))
							.First();
						if (dup != null)
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500,
								$"该库位已存在物料【{material.MaterialCode}】批次【{(string.IsNullOrEmpty(batchNo) ? "无" : batchNo)}】的库存，请在库存行下拉中选择该行调整，不要重复新增！");
						}
					}
					else
					{
						// 兼容：不带 stock_id 的负调整（旧单据），退回按物料+批次+库位找无处理单元的行
						stock = _client.Queryable<WmsStockEntity>()
							.Where(p => p.IsDel == 0 && p.MaterialId == d.MaterialId
								&& p.LocationId == d.LocationId.Value && p.HuId == null)
							.Where(p => p.BatchNo == batchNo || (p.BatchNo == null && batchNo == ""))
							.First();
						if (stock == null)
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "该库位没有此物料的库存行，负数调整必须指定库存行！");
						}
					}

					var beforeQty = stock?.StockQuantity ?? 0;
					var afterQty = beforeQty + d.AdjustQuantity;
					if (afterQty < 0)
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500,
							$"物料调整后库存为负（调整前 {beforeQty}，调整 {d.AdjustQuantity}），请核对数量！");
					}

					if (stock != null)
					{
						var ok = _client.Updateable<WmsStockEntity>()
							.SetColumns(p => p.StockQuantity == afterQty)
							.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) + d.AdjustQuantity)
							.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
							.Where(p => p.StockId == stock.StockId)
							.ExecuteCommand() > 0;
						if (!ok)
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "更新实时库存失败！");
						}
					}
					else
					{
						// 库存行不存在：盘盈新增一条库存（无条码，按物料+批次+库位归集）
						var warehouse = _client.Queryable<WmsWarehouseEntity>()
							.Where(p => p.WarehouseId == adjust.WarehouseId && p.IsDel == 0).First();
						var zone = zoneId > 0
							? _client.Queryable<WmsZoneEntity>().Where(p => p.ZoneId == zoneId && p.IsDel == 0).First()
							: null;

						var newStock = new WmsStockEntity
						{
							MaterialId = d.MaterialId,
							MaterialCode = material.MaterialCode,
							MaterialName = material.MaterialName,
							Spec = material.Spec,
							Unit = material.Unit,
							BatchNo = d.BatchNo,
							WarehouseId = adjust.WarehouseId,
							WarehouseName = warehouse?.WarehouseName,
							ZoneId = zoneId,
							ZoneCode = zone?.ZoneCode,
							ZoneName = zone?.ZoneName,
							LocationId = d.LocationId.Value,
							LocationCode = location.LocationCode,
							LocationName = location.LocationName,
							StockQuantity = d.AdjustQuantity,
						};
						if (!_stockService.AddOrUpdateStock(newStock))
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "新增库存失败！");
						}
					}

					// 回填明细的真实调整前/后数量
					var beforeInt = (int)beforeQty;
					var afterInt = (int)afterQty;
					_client.Updateable<WmsAdjustDetailEntity>()
						.SetColumns(p => p.BeforeQuantity == beforeInt)
						.SetColumns(p => p.AfterQuantity == afterInt)
						.SetColumns(p => new WmsAdjustDetailEntity { UpdateBy = account, UpdateTime = now })
						.Where(p => p.DetailId == d.DetailId)
						.ExecuteCommand();

					// 写库存流水流水
					seq++;
					var record = new WmsStockRecordEntity
					{
						RecordNo = $"{adjust.AdjustNo}-{seq:000}",
						MaterialId = d.MaterialId,
						BatchNo = d.BatchNo,
						WarehouseId = adjust.WarehouseId,
						ZoneId = zoneId,
						LocationId = d.LocationId.Value,
						BizType = "adjust",
						SourceType = "adjust",
						SourceId = adjust.AdjustId,
						SourceNo = adjust.AdjustNo,
						Direction = d.AdjustQuantity > 0 ? "in" : "out",
						BeforeQuantity = beforeQty,
						ChangeQuantity = Math.Abs(d.AdjustQuantity),
						AfterQuantity = afterQty,
						OperateTime = now,
						Remark = string.IsNullOrWhiteSpace(adjust.AdjustReason) ? "库存调整" : adjust.AdjustReason,
					};
					if (!_stockRecordService.Save(record))
					{
						_client.Ado.RollbackTran();
						return new ReturnData(500, "写入库存流水失败！");
					}
				}

				// 主表：置为已完成（此状态防止重复执行）
				var totalQty = details.Sum(p => p.AdjustQuantity);
				var updated = _client.Updateable<WmsAdjustEntity>()
					.SetColumns(p => new WmsAdjustEntity
					{
						AdjustStatus = "completed",
						TotalQuantity = totalQty,
						ApproveTime = now,
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.AdjustId == adjustId)
					.ExecuteCommand() > 0;
				if (!updated)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "更新调整单状态失败！");
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

			// 已执行的调整单不能删除（库存已变动，删除会丢失凭证）
			foreach (var id in delIds)
			{
				var exist = _adjustService.GetById(id);
				if (exist != null && exist.AdjustStatus == "completed")
					return new ReturnData(500, $"调整单 {exist.AdjustNo} 已执行，不能删除！");
			}

			_client.Ado.BeginTran();
			try
			{
				// 级联软删除明细
				foreach (var id in delIds)
					_adjustService.DeleteDetailsByAdjustId(id);

				// 软删除主表
				if (!_adjustService.Delete(delIds))
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
