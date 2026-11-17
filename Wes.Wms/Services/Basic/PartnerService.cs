using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 往来单位 数据访问
	/// </summary>
	public class PartnerService : Repository<WmsPartnerEntity>, IPartnerService
	{
		public PartnerService(ISqlSugarClient db) : base(db) { }

		public List<WmsPartnerEntity> GetList(ParamData<WmsPartnerParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsPartnerEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.PartnerCode.Contains(kw));
				exp.Or(p => p.PartnerName.Contains(kw));
				exp.Or(p => p.BankName.Contains(kw));
				exp.Or(p => p.TaxNo.Contains(kw));
			}
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Status))
				exp.And(p => p.Status == param.Params.Status.Trim());
		if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.PartnerType))
			exp.And(p => p.PartnerType == param.Params.PartnerType.Trim());
			total = 0;
			var query = Context.Queryable<WmsPartnerEntity>().Where(exp.ToExpression()).OrderBy(p => p.PartnerId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsPartnerEntity? GetById(long id)
		{
			return Context.Queryable<WmsPartnerEntity>().Where(p => p.PartnerId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsPartnerEntity model)
		{
			if (model.PartnerId > 0)
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
			return Context.Updateable<WmsPartnerEntity>()
				.SetColumns(p => new WmsPartnerEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.PartnerId)).ExecuteCommand() > 0;
		}

		public bool ChangeStatus(WmsPartnerEntity model)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsPartnerEntity>()
				.SetColumns(p => new WmsPartnerEntity { Status = model.Status, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => p.PartnerId == model.PartnerId).ExecuteCommand() > 0;
		}
	}
}
