using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库位锁定 数据访问
	/// </summary>
	public class LocationLockService : Repository<WmsLocationLockEntity>, ILocationLockService
	{
		public LocationLockService(ISqlSugarClient db) : base(db) { }

		public List<WmsLocationLockEntity> GetList(ParamData<WmsLocationLockParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsLocationLockEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsLocationLockEntity>().Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Where(exp.ToExpression()).OrderBy(p => p.LockId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsLocationLockEntity? GetById(long id)
		{
			return Context.Queryable<WmsLocationLockEntity>().Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Where(p => p.LockId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsLocationLockEntity model)
		{
			if (model.LockId > 0)
			{
				model.UpdateBy = GlobalContext.CurrentUser?.Account;
				model.UpdateTime = DateTime.Now;
				return Update(model);
			}
			model.CreateBy = GlobalContext.CurrentUser?.Account;
			model.CreateTime = DateTime.Now;
			return InsertReturnSnowflakeId(model) > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsLocationLockEntity>()
				.SetColumns(p => new WmsLocationLockEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.LockId)).ExecuteCommand() > 0;
		}
	}
}
