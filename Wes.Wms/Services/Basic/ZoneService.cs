using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库区 数据访问
	/// </summary>
	public class ZoneService : Repository<WmsZoneEntity>, IZoneService
	{
		public ZoneService(ISqlSugarClient db) : base(db) { }

		public List<WmsZoneEntity> GetList(ParamData<WmsZoneParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsZoneEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.ZoneCode.Contains(kw));
				exp.Or(p => p.ZoneName.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsZoneEntity>().Includes(p => p.Warehouse).Where(exp.ToExpression()).OrderBy(p => p.ZoneId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsZoneEntity? GetById(long id)
		{
			return Context.Queryable<WmsZoneEntity>().Includes(p => p.Warehouse).Where(p => p.ZoneId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsZoneEntity model)
		{
			if (model.ZoneId > 0)
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
			return Context.Updateable<WmsZoneEntity>()
				.SetColumns(p => new WmsZoneEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.ZoneId)).ExecuteCommand() > 0;
		}

		public bool ChangeStatus(WmsZoneEntity model)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsZoneEntity>()
				.SetColumns(p => new WmsZoneEntity { ZoneStatus = model.ZoneStatus, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => p.ZoneId == model.ZoneId).ExecuteCommand() > 0;
		}
	}
}
