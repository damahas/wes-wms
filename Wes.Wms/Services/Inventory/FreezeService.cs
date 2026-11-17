using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存冻结 数据访问
	/// </summary>
	public class FreezeService : Repository<WmsFreezeEntity>, IFreezeService
	{
		public FreezeService(ISqlSugarClient db) : base(db) { }

		public List<WmsFreezeEntity> GetList(ParamData<WmsFreezeParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsFreezeEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.FreezeNo.Contains(kw));
				exp.Or(p => p.BatchNo.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsFreezeEntity>().Includes(p => p.Material).Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Where(exp.ToExpression()).OrderBy(p => p.FreezeId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsFreezeEntity? GetById(long id)
		{
			return Context.Queryable<WmsFreezeEntity>().Includes(p => p.Material).Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Where(p => p.FreezeId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsFreezeEntity model)
		{
			if (model.FreezeId > 0)
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
			return Context.Updateable<WmsFreezeEntity>()
				.SetColumns(p => new WmsFreezeEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.FreezeId)).ExecuteCommand() > 0;
		}
	}
}
