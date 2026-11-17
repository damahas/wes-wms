using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 物料 数据访问
	/// </summary>
	public class MaterialService : Repository<WmsMaterialEntity>, IMaterialService
	{
		public MaterialService(ISqlSugarClient db) : base(db) { }

		public List<WmsMaterialEntity> GetList(ParamData<WmsMaterialParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsMaterialEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.MaterialCode.Contains(kw));
				exp.Or(p => p.MaterialName.Contains(kw));
				exp.Or(p => p.MaterialShortName.Contains(kw));
				exp.Or(p => p.Barcode.Contains(kw));
			}
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Status))
				exp.And(p => p.Status == param.Params.Status.Trim());
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.MaterialType))
				exp.And(p => p.MaterialType == param.Params.MaterialType.Trim());
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.CategoryId))
				exp.And(p => p.CategoryId == param.Params.CategoryId.Trim());
			total = 0;
			var query = Context.Queryable<WmsMaterialEntity>().Includes(p => p.DefaultWarehouse).Includes(p => p.DefaultZone).Includes(p => p.DefaultLocation).Includes(p => p.Suppliers).Where(exp.ToExpression()).OrderBy(p => p.MaterialId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsMaterialEntity? GetById(long id)
		{
			var entity = Context.Queryable<WmsMaterialEntity>().Includes(p => p.DefaultWarehouse).Includes(p => p.DefaultZone).Includes(p => p.DefaultLocation).Includes(p => p.Suppliers).Where(p => p.MaterialId == id && p.IsDel == 0).First();
			// 过滤掉已软删除的供应商关联记录
			if (entity != null && entity.Suppliers != null)
				entity.Suppliers = entity.Suppliers.Where(s => s.IsDel == 0).ToList();
			return entity;
		}

		/// <summary>
		/// 仅保存主表（插入或更新），不含子表级联。子表逻辑由 MaterialBiz 统一管理事务。
		/// </summary>
		public bool Save(WmsMaterialEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			if (model.MaterialId > 0)
			{
				model.UpdateBy = account;
				model.UpdateTime = now;
				return Context.Updateable(model).ExecuteCommand() > 0;
			}
			model.CreateBy = account;
			model.CreateTime = now;
			model.MaterialId = Context.Insertable(model).ExecuteReturnSnowflakeId();
			return model.MaterialId > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsMaterialEntity>()
				.SetColumns(p => new WmsMaterialEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.MaterialId)).ExecuteCommand() > 0;
		}

		public bool ChangeStatus(WmsMaterialEntity model)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsMaterialEntity>()
				.SetColumns(p => new WmsMaterialEntity { Status = model.Status, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => p.MaterialId == model.MaterialId).ExecuteCommand() > 0;
		}
	}
}
