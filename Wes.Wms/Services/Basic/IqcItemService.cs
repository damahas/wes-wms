using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 检验项 数据访问
	/// </summary>
	public class IqcItemService : Repository<WmsIqcItemEntity>, IIqcItemService
	{
		public IqcItemService(ISqlSugarClient db) : base(db) { }

		public List<WmsIqcItemEntity> GetList(string categoryId)
		{
			return Context.Queryable<WmsIqcItemEntity>()
				.Where(p => p.IsDel == 0 && p.CategoryId == categoryId)
				.OrderBy(p => p.SortNo)
				.OrderBy(p => p.ItemId)
				.ToList();
		}

		public WmsIqcItemEntity? GetById(long id)
		{
			return Context.Queryable<WmsIqcItemEntity>()
				.Where(p => p.ItemId == id && p.IsDel == 0)
				.First();
		}

		public bool BatchSave(string categoryId, List<WmsIqcItemEntity> items)
		{
			// 软删除该分类下所有旧记录
			Context.Updateable<WmsIqcItemEntity>()
				.SetColumns(p => new WmsIqcItemEntity { IsDel = 1 })
				.Where(p => p.CategoryId == categoryId && p.IsDel == 0)
				.ExecuteCommand();
			// 逐条插入新记录（每条生成独立雪花ID）
			if (items != null && items.Count > 0)
			{
				var account = GlobalContext.CurrentUser?.Account;
				var now = System.DateTime.Now;
				foreach (var item in items)
				{
					item.CategoryId = categoryId;
					item.IsDel = 0;
					item.CreateBy = account;
					item.CreateTime = now;
					item.UpdateBy = account;
					item.UpdateTime = now;
					Context.Insertable(item).ExecuteReturnSnowflakeId();
				}
				return true;
			}
			return true;
		}

		public bool Delete(long id)
		{
			return Context.Updateable<WmsIqcItemEntity>()
				.SetColumns(p => new WmsIqcItemEntity { IsDel = 1 })
				.Where(p => p.ItemId == id)
				.ExecuteCommand() > 0;
		}

		public List<WmsIqcItemEntity> GetItemsByMaterial(long materialId)
		{
			// 1. 获取物料的分类ID
			var categoryId = Context.Queryable<WmsMaterialEntity>()
				.Where(p => p.MaterialId == materialId)
				.Select(p => p.CategoryId)
				.First();

			if (string.IsNullOrWhiteSpace(categoryId))
				return new List<WmsIqcItemEntity>();

			// 2. 递归获取分类链（含当前分类 + 所有祖先分类）
			var categoryIds = new HashSet<string> { categoryId };
			var currentId = categoryId;
			while (!string.IsNullOrWhiteSpace(currentId))
			{
				var parentId = Context.Ado.SqlQuerySingle<string>(
					"SELECT parent_id FROM sys_dict_data WHERE dict_type = 'wms_material_category' AND dict_value = @dictValue AND is_del = 0 LIMIT 1",
					new { dictValue = currentId });
				if (!string.IsNullOrWhiteSpace(parentId) && categoryIds.Add(parentId))
					currentId = parentId;
				else
					break;
			}

			// 3. 按所有分类ID加载检验项
			if (categoryIds.Count == 0)
				return new List<WmsIqcItemEntity>();

			return Context.Queryable<WmsIqcItemEntity>()
				.Where(p => p.IsDel == 0 && categoryIds.Contains(p.CategoryId))
				.OrderBy(p => p.SortNo)
				.OrderBy(p => p.ItemId)
				.ToList();
		}
	}
}
