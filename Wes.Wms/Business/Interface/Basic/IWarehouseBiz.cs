using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 仓库 业务逻辑接口
	/// </summary>
	public interface IWarehouseBiz
	{
		RowData<WmsWarehouseEntity> GetList(ParamData<WmsWarehouseParam> param);
		ResultData<WmsWarehouseEntity> GetById(long id);
		ReturnData Save(WmsWarehouseEntity model);
		ReturnData Delete(string ids);
		ReturnData ChangeStatus(WmsWarehouseEntity model);
	}
}
