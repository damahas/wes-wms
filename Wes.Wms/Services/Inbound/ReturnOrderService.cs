using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 退货单 数据访问
	/// </summary>
	public class ReturnOrderService : Repository<WmsReturnOrderEntity>, IReturnOrderService
	{
		public ReturnOrderService(ISqlSugarClient db) : base(db) { }

		public List<WmsReturnOrderEntity> GetList(ParamData<WmsReturnOrderParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsReturnOrderEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.ReturnOrderNo.Contains(kw));
			}
			if (param.Params != null && param.Params.SupplierId.HasValue)
				exp.And(p => p.SupplierId == param.Params.SupplierId.Value);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.ReturnType))
				exp.And(p => p.ReturnType == param.Params.ReturnType);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.ReturnStatus))
				exp.And(p => p.ReturnStatus == param.Params.ReturnStatus);
			total = 0;
			var query = Context.Queryable<WmsReturnOrderEntity>().Includes(p => p.Supplier).Where(exp.ToExpression()).OrderBy(p => p.ReturnOrderId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsReturnOrderEntity? GetById(long id)
		{
			return Context.Queryable<WmsReturnOrderEntity>().Includes(p => p.Supplier).Where(p => p.ReturnOrderId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsReturnOrderEntity model)
		{
			if (model.ReturnOrderId > 0)
			{
				model.UpdateBy = GlobalContext.CurrentUser?.Account;
				model.UpdateTime = DateTime.Now;
				return Context.Updateable(model).ExecuteCommand() > 0;
			}
			model.IsDel = 0;
			model.CreateBy = GlobalContext.CurrentUser?.Account;
			model.CreateTime = DateTime.Now;
			return Context.Insertable(model).ExecuteReturnSnowflakeId() > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsReturnOrderEntity>()
				.SetColumns(p => new WmsReturnOrderEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.ReturnOrderId)).ExecuteCommand() > 0;
		}
	}
}
