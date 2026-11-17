using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 往来单位地址 数据访问
	/// </summary>
	public class PartnerAddressService : Repository<WmsPartnerAddressEntity>, IPartnerAddressService
	{
		public PartnerAddressService(ISqlSugarClient db) : base(db) { }

		public List<WmsPartnerAddressEntity> GetList(ParamData<WmsPartnerAddressParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsPartnerAddressEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.ContactName.Contains(kw));
			}
			total = 0;
			var query = Context.Queryable<WmsPartnerAddressEntity>().Includes(p => p.Partner).Where(exp.ToExpression()).OrderBy(p => p.AddressId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsPartnerAddressEntity? GetById(long id)
		{
			return Context.Queryable<WmsPartnerAddressEntity>().Includes(p => p.Partner).Where(p => p.AddressId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsPartnerAddressEntity model)
		{
			if (model.AddressId > 0)
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
			return Context.Updateable<WmsPartnerAddressEntity>()
				.SetColumns(p => new WmsPartnerAddressEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.AddressId)).ExecuteCommand() > 0;
		}
	}
}
