using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 往来单位联系人 数据访问
	/// </summary>
	public class PartnerContactService : Repository<WmsPartnerContactEntity>, IPartnerContactService
	{
		public PartnerContactService(ISqlSugarClient db) : base(db) { }

		public List<WmsPartnerContactEntity> GetList(ParamData<WmsPartnerContactParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsPartnerContactEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.ContactName.Contains(kw));
			}
			total = 0;
			var query = Context.Queryable<WmsPartnerContactEntity>().Includes(p => p.Partner).Where(exp.ToExpression()).OrderBy(p => p.ContactId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsPartnerContactEntity? GetById(long id)
		{
			return Context.Queryable<WmsPartnerContactEntity>().Includes(p => p.Partner).Where(p => p.ContactId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsPartnerContactEntity model)
		{
			if (model.ContactId > 0)
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
			return Context.Updateable<WmsPartnerContactEntity>()
				.SetColumns(p => new WmsPartnerContactEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.ContactId)).ExecuteCommand() > 0;
		}
	}
}
