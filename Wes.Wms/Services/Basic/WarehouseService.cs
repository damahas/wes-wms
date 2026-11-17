using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 仓库 数据访问
	/// </summary>
	public class WarehouseService : Repository<WmsWarehouseEntity>, IWarehouseService
	{
		public WarehouseService(ISqlSugarClient db) : base(db) { }

		public List<WmsWarehouseEntity> GetList(ParamData<WmsWarehouseParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsWarehouseEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.WarehouseCode.Contains(kw));
				exp.Or(p => p.WarehouseName.Contains(kw));
			}
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Status))
				exp.And(p => p.Status == param.Params.Status.Trim());
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsWarehouseEntity>().Where(exp.ToExpression()).OrderBy(p => p.WarehouseId, OrderByType.Desc)
				.Includes(p => p.ManagerUser);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsWarehouseEntity? GetById(long id)
		{
			return Context.Queryable<WmsWarehouseEntity>().Where(p => p.WarehouseId == id && p.IsDel == 0)
				.Includes(p => p.ManagerUser).First();
		}

		public bool Save(WmsWarehouseEntity model)
		{
			if (model.WarehouseId > 0)
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
			return Context.Updateable<WmsWarehouseEntity>()
				.SetColumns(p => new WmsWarehouseEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.WarehouseId)).ExecuteCommand() > 0;
		}

		public bool ChangeStatus(WmsWarehouseEntity model)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsWarehouseEntity>()
				.SetColumns(p => new WmsWarehouseEntity { Status = model.Status, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => p.WarehouseId == model.WarehouseId).ExecuteCommand() > 0;
		}
	}
}
