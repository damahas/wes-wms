using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 出库退货 数据访问
	/// </summary>
	public class ReturnOutService : Repository<WmsReturnOutEntity>, IReturnOutService
	{
		public ReturnOutService(ISqlSugarClient db) : base(db) { }

		public List<WmsReturnOutEntity> GetList(ParamData<WmsReturnOutParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsReturnOutEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.ReturnNo.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsReturnOutEntity>().Includes(p => p.Delivery).Includes(p => p.Customer).Includes(p => p.Warehouse).Includes(p => p.Zone).Where(exp.ToExpression()).OrderBy(p => p.ReturnId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsReturnOutEntity? GetById(long id)
		{
			return Context.Queryable<WmsReturnOutEntity>().Includes(p => p.Delivery).Includes(p => p.Customer).Includes(p => p.Warehouse).Includes(p => p.Zone).Where(p => p.ReturnId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsReturnOutEntity model)
		{
			if (model.ReturnId > 0)
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
			return Context.Updateable<WmsReturnOutEntity>()
				.SetColumns(p => new WmsReturnOutEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.ReturnId)).ExecuteCommand() > 0;
		}
	}
}
