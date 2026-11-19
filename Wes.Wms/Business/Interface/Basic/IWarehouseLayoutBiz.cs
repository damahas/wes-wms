using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 仓库平面布局 业务逻辑接口
	/// </summary>
	public interface IWarehouseLayoutBiz
	{
		ResultData<WmsWarehouseLayoutEntity> GetByWarehouse(long warehouseId);
		ReturnData Save(WmsWarehouseLayoutEntity model);
		ReturnData Publish(long layoutId);
	}
}
