using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 仓库平面布局 数据访问接口
	/// </summary>
	public interface IWarehouseLayoutService
	{
		WmsWarehouseLayoutEntity? GetById(long layoutId);
		WmsWarehouseLayoutEntity? GetByWarehouse(long warehouseId);
		bool Save(WmsWarehouseLayoutEntity model);
		bool Publish(long layoutId);
	}
}
