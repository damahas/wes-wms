using System;
using SqlSugar;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存冻结 业务逻辑
	/// 冻结只影响可用量（available_quantity），不改变库存总量（stock_quantity），
	/// 因此不写库存流水流水、也不影响"流水累加 = 库存数量"的对账关系；
	/// 冻结单本身就是这笔变动的凭证。
	/// </summary>
	public class FreezeBiz : IFreezeBiz
	{
		private readonly IFreezeService _freezeService;
		private readonly ISqlSugarClient _client;

		public FreezeBiz(IFreezeService freezeService, ISqlSugarClient sqlSugarClient)
		{
			_freezeService = freezeService;
			_client = sqlSugarClient;
		}

		public RowData<WmsFreezeEntity> GetList(ParamData<WmsFreezeParam> param)
		{
			int total = 0;
			var result = new RowData<WmsFreezeEntity>(_freezeService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsFreezeEntity> GetById(long id)
		{
			return new ResultData<WmsFreezeEntity>(_freezeService.GetById(id));
		}

		public ReturnData Save(WmsFreezeEntity model)
		{
			// 已冻结的单据不允许修改
			if (model.FreezeId > 0)
			{
				var exist = _freezeService.GetById(model.FreezeId);
				if (exist == null) return new ReturnData(500, "冻结单不存在！");
				if (exist.FreezeStatus == "frozen")
					return new ReturnData(500, "该冻结单已执行冻结，不能修改！");
			}

			if (model.FreezeQuantity <= 0)
				return new ReturnData(500, "冻结数量必须大于0！");

			// 校验库存行与单据的物料/库位一致（同物料同批次可能多条码行，必须精确对上）
			if (model.StockId.HasValue && model.StockId.Value > 0)
			{
				var stockRow = _client.Queryable<WmsStockEntity>()
					.Where(p => p.StockId == model.StockId.Value && p.IsDel == 0).First();
				if (stockRow == null)
					return new ReturnData(500, "库存行不存在！");
				if (stockRow.MaterialId != model.MaterialId || stockRow.LocationId != model.LocationId)
					return new ReturnData(500, "库存行与冻结单的物料/库位不匹配！");
			}

			// 新建时默认待冻结：只有执行冻结后才置为 frozen（SqlSugar 会显式写入所有列，数据库默认值不生效）
			if (string.IsNullOrWhiteSpace(model.FreezeStatus))
				model.FreezeStatus = "pending";

			return _freezeService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		/// <summary>执行冻结：库存行 freeze_quantity 增加、available_quantity 减少</summary>
		public ReturnData Execute(long freezeId)
		{
			var freeze = _freezeService.GetById(freezeId);
			if (freeze == null) return new ReturnData(500, "冻结单不存在！");
			if (freeze.FreezeStatus == "frozen") return new ReturnData(500, "该冻结单已冻结，不能重复执行！");
			if (freeze.FreezeQuantity <= 0) return new ReturnData(500, "冻结数量必须大于0！");
			if (!freeze.LocationId.HasValue || freeze.LocationId.Value <= 0)
				return new ReturnData(500, "请指定冻结库位！");

			var qty = (decimal)freeze.FreezeQuantity;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			_client.Ado.BeginTran();
			try
			{
				var (stock, err) = ResolveStock(freeze.StockId, freeze.MaterialId, freeze.BatchNo, freeze.LocationId.Value);
				if (stock == null)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, err);
				}

				var available = (stock.StockQuantity ?? 0) - (stock.FreezeQuantity ?? 0) - (stock.LockQuantity ?? 0);
				if (available < qty)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, $"可用库存不足（可用 {available}，需冻结 {qty}）！");
				}

				var affected = _client.Updateable<WmsStockEntity>()
					.SetColumns(p => p.FreezeQuantity == (p.FreezeQuantity ?? 0) + qty)
					.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) - qty)
					.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
					.Where(p => p.StockId == stock.StockId)
					.ExecuteCommand();
				if (affected <= 0)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "更新库存冻结数量失败！");
				}

				_client.Updateable<WmsFreezeEntity>()
					.SetColumns(p => new WmsFreezeEntity
					{
						FreezeStatus = "frozen",
						FreezeTime = now,
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.FreezeId == freezeId)
					.ExecuteCommand();

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"冻结失败：{ex.Message}");
			}

			return new ReturnData();
		}

		/// <summary>解冻：库存行 freeze_quantity 减少、available_quantity 回补</summary>
		public ReturnData Thaw(long freezeId, string thawReason)
		{
			var freeze = _freezeService.GetById(freezeId);
			if (freeze == null) return new ReturnData(500, "冻结单不存在！");
			if (freeze.FreezeStatus != "frozen") return new ReturnData(500, "该冻结单未冻结，无需解冻！");
			if (!freeze.LocationId.HasValue || freeze.LocationId.Value <= 0)
				return new ReturnData(500, "冻结单缺少库位，无法解冻！");

			var qty = (decimal)freeze.FreezeQuantity;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			_client.Ado.BeginTran();
			try
			{
				var (stock, err) = ResolveStock(freeze.StockId, freeze.MaterialId, freeze.BatchNo, freeze.LocationId.Value);
				if (stock == null)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, err);
				}

				if ((stock.FreezeQuantity ?? 0) < qty)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, $"库存冻结数量不足（当前冻结 {stock.FreezeQuantity ?? 0}，需解冻 {qty}）！");
				}

				_client.Updateable<WmsStockEntity>()
					.SetColumns(p => p.FreezeQuantity == (p.FreezeQuantity ?? 0) - qty)
					.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) + qty)
					.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
					.Where(p => p.StockId == stock.StockId)
					.ExecuteCommand();

				_client.Updateable<WmsFreezeEntity>()
					.SetColumns(p => new WmsFreezeEntity
					{
						FreezeStatus = "thawed",
						ThawTime = now,
						ThawReason = thawReason,
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.FreezeId == freezeId)
					.ExecuteCommand();

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"解冻失败：{ex.Message}");
			}

			return new ReturnData();
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			// 已冻结的单据必须先解冻才能删除，避免库存冻结数量成孤儿
			foreach (var id in delIds)
			{
				var exist = _freezeService.GetById(id);
				if (exist != null && exist.FreezeStatus == "frozen")
					return new ReturnData(500, "冻结中的单据不能删除，请先解冻！");
			}

			return _freezeService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}

		/// <summary>定位库存行：优先按单据上的库存行id精确取（同物料同批次可能多条码行）；老数据无 stock_id 退回按物料+批次+库位找</summary>
		private (WmsStockEntity Stock, string Error) ResolveStock(long? stockId, long materialId, string batchNo, long locationId)
		{
			if (stockId.HasValue && stockId.Value > 0)
			{
				var stock = _client.Queryable<WmsStockEntity>()
					.Where(p => p.StockId == stockId.Value && p.IsDel == 0).First();
				if (stock == null)
					return (null, "单据指向的库存行不存在（可能已被其他单据处理）！");
				if (stock.MaterialId != materialId || stock.LocationId != locationId)
					return (null, "单据与库存行不匹配（物料或库位对不上）！");
				return (stock, null);
			}

			var batch = batchNo ?? "";
			var fallback = _client.Queryable<WmsStockEntity>()
				.Where(p => p.IsDel == 0 && p.MaterialId == materialId
					&& p.LocationId == locationId && p.HuId == null)
				.Where(p => p.BatchNo == batch || (p.BatchNo == null && batch == ""))
				.First();
			return (fallback, fallback == null ? "该库位下没有此物料的库存！" : null);
		}
	}
}
