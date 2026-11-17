using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库位 数据访问
	/// </summary>
	public class LocationService : Repository<WmsLocationEntity>, ILocationService
	{
		public LocationService(ISqlSugarClient db) : base(db) { }

		public List<WmsLocationEntity> GetList(ParamData<WmsLocationParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsLocationEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.LocationCode.Contains(kw));
				exp.Or(p => p.LocationName.Contains(kw));
				exp.Or(p => p.ShelfCode.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			if (param.Params != null && param.Params.ZoneId.HasValue)
				exp.And(p => p.ZoneId == param.Params.ZoneId.Value);
			total = 0;
			var query = Context.Queryable<WmsLocationEntity>().Includes(p => p.Warehouse).Includes(p => p.Zone).Where(exp.ToExpression()).OrderBy(p => p.LocationId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsLocationEntity? GetById(long id)
		{
			return Context.Queryable<WmsLocationEntity>().Includes(p => p.Warehouse).Includes(p => p.Zone).Where(p => p.LocationId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsLocationEntity model)
		{
			if (model.LocationId > 0)
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
			return Context.Updateable<WmsLocationEntity>()
				.SetColumns(p => new WmsLocationEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.LocationId)).ExecuteCommand() > 0;
		}

		public bool ChangeStatus(WmsLocationEntity model)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsLocationEntity>()
				.SetColumns(p => new WmsLocationEntity { LocationStatus = model.LocationStatus, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => p.LocationId == model.LocationId).ExecuteCommand() > 0;
		}
	}
}
