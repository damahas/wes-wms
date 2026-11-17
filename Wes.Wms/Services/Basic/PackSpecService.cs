using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 包装规格 数据访问
	/// </summary>
	public class PackSpecService : Repository<WmsPackSpecEntity>, IPackSpecService
	{
		public PackSpecService(ISqlSugarClient db) : base(db) { }

		/// <summary>
		/// 列表：仅返回最顶层的包装规格（parent_id IS NULL）。
		/// 子层级在编辑弹窗内通过 GetTree 整棵加载管理。
		/// </summary>
		public List<WmsPackSpecEntity> GetList(ParamData<WmsPackSpecParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsPackSpecEntity>();
			exp.And(p => p.IsDel == 0);
			exp.And(p => p.ParentId == null);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.And(p => p.LevelName.Contains(kw));
			}
			total = 0;
			var query = Context.Queryable<WmsPackSpecEntity>()
				.Where(exp.ToExpression())
				.OrderBy(p => p.SortOrder, OrderByType.Asc)
				.OrderBy(p => p.SpecId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		/// <summary>
		/// 获取一整套包装规格的层级树（按 root_id 归组的所有层级）。
		/// </summary>
		public List<WmsPackSpecEntity> GetTree(long rootId)
		{
			return Context.Queryable<WmsPackSpecEntity>()
				.Where(p => p.RootId == rootId && p.IsDel == 0)
				.OrderBy(p => p.SortOrder, OrderByType.Asc)
				.OrderBy(p => p.SpecId, OrderByType.Asc)
				.ToList();
		}

		public WmsPackSpecEntity? GetById(long id)
		{
			return Context.Queryable<WmsPackSpecEntity>().Where(p => p.SpecId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsPackSpecEntity model)
		{
			if (model.SpecId > 0)
			{
				model.UpdateBy = GlobalContext.CurrentUser?.Account;
				model.UpdateTime = DateTime.Now;
				return Update(model);
			}
			model.CreateBy = GlobalContext.CurrentUser?.Account;
			model.CreateTime = DateTime.Now;
			model.SpecId = InsertReturnSnowflakeId(model);
			return model.SpecId > 0;
		}

		/// <summary>纯 CRUD：按 rootId 软删除整棵树</summary>
		public bool SoftDeleteByRootId(long rootId)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsPackSpecEntity>()
				.SetColumns(p => new WmsPackSpecEntity { IsDel = 1, UpdateTime = now, UpdateBy = updateBy })
				.Where(p => p.RootId == rootId && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		/// <summary>纯 CRUD：按 specId 软删除自身及直接子节点</summary>
		public bool SoftDeleteSelfAndChildren(long specId)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsPackSpecEntity>()
				.SetColumns(p => new WmsPackSpecEntity { IsDel = 1, UpdateTime = now, UpdateBy = updateBy })
				.Where(p => (p.SpecId == specId || p.ParentId == specId) && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}
	}
}
