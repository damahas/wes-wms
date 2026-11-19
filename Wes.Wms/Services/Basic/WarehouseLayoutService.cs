using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 仓库平面布局 数据访问
	/// </summary>
	public class WarehouseLayoutService : Repository<WmsWarehouseLayoutEntity>, IWarehouseLayoutService
	{
		public WarehouseLayoutService(ISqlSugarClient db) : base(db) { }

		public WmsWarehouseLayoutEntity? GetById(long layoutId)
		{
			return Context.Queryable<WmsWarehouseLayoutEntity>()
				.Where(p => p.LayoutId == layoutId && p.IsDel == 0)
				.First();
		}

		public WmsWarehouseLayoutEntity? GetByWarehouse(long warehouseId)
		{
			return Context.Queryable<WmsWarehouseLayoutEntity>()
				.Where(p => p.WarehouseId == warehouseId && p.IsDel == 0)
				.OrderBy(p => p.Version, OrderByType.Desc)
				.First();
		}

		public bool Save(WmsWarehouseLayoutEntity model)
		{
			if (model.LayoutId > 0)
			{
				model.UpdateBy = GlobalContext.CurrentUser?.Account;
				model.UpdateTime = DateTime.Now;
				return Update(model);
			}
			model.CreateBy = GlobalContext.CurrentUser?.Account;
			model.CreateTime = DateTime.Now;
			model.Version = 1;
			return InsertReturnSnowflakeId(model) > 0;
		}

		public bool Publish(long layoutId)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsWarehouseLayoutEntity>()
				.SetColumns(p => new WmsWarehouseLayoutEntity { Status = "1", UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => p.LayoutId == layoutId).ExecuteCommand() > 0;
		}
	}
}
