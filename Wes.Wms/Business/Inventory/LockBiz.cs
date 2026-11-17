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
	/// 库存锁定 业务逻辑
	/// 锁定只占用可用量（available_quantity），不改变库存总量（stock_quantity），
	/// 因此不写库存流水流水；锁定单本身即凭证。
	/// 出库单发货时应优先消耗锁定数量（lock_quantity），并把已用量记到 used_quantity。
	/// </summary>
	public class LockBiz : ILockBiz
	{
		private readonly ILockService _lockService;
		private readonly ISqlSugarClient _client;

		public LockBiz(ILockService lockService, ISqlSugarClient sqlSugarClient)
		{
			_lockService = lockService;
			_client = sqlSugarClient;
		}

		public RowData<WmsLockEntity> GetList(ParamData<WmsLockParam> param)
		{
			int total = 0;
			var result = new RowData<WmsLockEntity>(_lockService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsLockEntity> GetById(long id)
		{
			return new ResultData<WmsLockEntity>(_lockService.GetById(id));
		}

		public ReturnData Save(WmsLockEntity model)
		{
			if (model.LockId > 0)
			{
				var exist = _lockService.GetById(model.LockId);
				if (exist == null) return new ReturnData(500, "锁定单不存在！");
				if (exist.LockStatus == "locked")
					return new ReturnData(500, "该锁定单已执行锁定，不能修改！");
			}

			if (model.LockQuantity <= 0)
				return new ReturnData(500, "锁定数量必须大于0！");

			// 校验库存行与单据的物料/库位一致（同物料同批次可能多条码行，必须精确对上）
			if (model.StockId.HasValue && model.StockId.Value > 0)
			{
				var stockRow = _client.Queryable<WmsStockEntity>()
					.Where(p => p.StockId == model.StockId.Value && p.IsDel == 0).First();
				if (stockRow == null)
					return new ReturnData(500, "库存行不存在！");
				if (stockRow.MaterialId != model.MaterialId || stockRow.LocationId != model.LocationId)
					return new ReturnData(500, "库存行与锁定单的物料/库位不匹配！");
			}

			// 新建时默认待锁定：只有执行锁定后才置为 locked（SqlSugar 会显式写入所有列，数据库默认值不生效）
			if (string.IsNullOrWhiteSpace(model.LockStatus))
				model.LockStatus = "pending";

			return _lockService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		/// <summary>执行锁定：库存行 lock_quantity 增加、available_quantity 减少</summary>
		public ReturnData Execute(long lockId)
		{
			var entity = _lockService.GetById(lockId);
			if (entity == null) return new ReturnData(500, "锁定单不存在！");
			if (entity.LockStatus == "locked") return new ReturnData(500, "该锁定单已锁定，不能重复执行！");
			if (entity.LockQuantity <= 0) return new ReturnData(500, "锁定数量必须大于0！");
			if (!entity.LocationId.HasValue || entity.LocationId.Value <= 0)
				return new ReturnData(500, "请指定锁定库位！");

			var qty = (decimal)entity.LockQuantity;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			_client.Ado.BeginTran();
			try
			{
				var (stock, err) = ResolveStock(entity.StockId, entity.MaterialId, entity.BatchNo, entity.LocationId.Value);
				if (stock == null)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, err);
				}

				var available = (stock.StockQuantity ?? 0) - (stock.FreezeQuantity ?? 0) - (stock.LockQuantity ?? 0);
				if (available < qty)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, $"可用库存不足（可用 {available}，需锁定 {qty}）！");
				}

				_client.Updateable<WmsStockEntity>()
					.SetColumns(p => p.LockQuantity == (p.LockQuantity ?? 0) + qty)
					.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) - qty)
					.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
					.Where(p => p.StockId == stock.StockId)
					.ExecuteCommand();

				_client.Updateable<WmsLockEntity>()
					.SetColumns(p => new WmsLockEntity
					{
						LockStatus = "locked",
						LockTime = now,
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.LockId == lockId)
					.ExecuteCommand();

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"锁定失败：{ex.Message}");
			}

			return new ReturnData();
		}

		/// <summary>释放锁定：未使用部分回补可用量</summary>
		public ReturnData Release(long lockId)
		{
			var entity = _lockService.GetById(lockId);
			if (entity == null) return new ReturnData(500, "锁定单不存在！");
			if (entity.LockStatus == "released") return new ReturnData(500, "该锁定单已释放！");
			if (!entity.LocationId.HasValue || entity.LocationId.Value <= 0)
				return new ReturnData(500, "锁定单缺少库位，无法释放！");

			// 未使用部分才回补（已被出库消耗的部分不应退回可用量）
			var remain = entity.LockQuantity - (entity.UsedQuantity ?? 0);
			if (remain < 0) remain = 0;

			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			if (remain == 0)
			{
				// 全部已被出库消耗：没有量可退，直接置为已完成
				_client.Updateable<WmsLockEntity>()
					.SetColumns(p => new WmsLockEntity { LockStatus = "completed", UpdateBy = account, UpdateTime = now })
					.Where(p => p.LockId == lockId)
					.ExecuteCommand();
				return new ReturnData();
			}

			var qty = (decimal)remain;

			_client.Ado.BeginTran();
			try
			{
				var (stock, err) = ResolveStock(entity.StockId, entity.MaterialId, entity.BatchNo, entity.LocationId.Value);
				if (stock == null)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, err);
				}

				if ((stock.LockQuantity ?? 0) < qty)
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, $"库存锁定数量不足（当前锁定 {stock.LockQuantity ?? 0}，需释放 {qty}）！");
				}

				_client.Updateable<WmsStockEntity>()
					.SetColumns(p => p.LockQuantity == (p.LockQuantity ?? 0) - qty)
					.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) + qty)
					.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
					.Where(p => p.StockId == stock.StockId)
					.ExecuteCommand();

				_client.Updateable<WmsLockEntity>()
					.SetColumns(p => new WmsLockEntity
					{
						LockStatus = "released",
						UpdateBy = account,
						UpdateTime = now,
					})
					.Where(p => p.LockId == lockId)
					.ExecuteCommand();

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"释放失败：{ex.Message}");
			}

			return new ReturnData();
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			foreach (var id in delIds)
			{
				var exist = _lockService.GetById(id);
				// locked 与 partial 都还占着库存，必须先释放
				if (exist != null && (exist.LockStatus == "locked" || exist.LockStatus == "partial"))
					return new ReturnData(500, "锁定中的单据不能删除，请先释放！");
			}

			return _lockService.Delete(delIds)
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
