using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 仓库 数据访问接口
	/// </summary>
	public interface IWarehouseService
	{
		List<WmsWarehouseEntity> GetList(ParamData<WmsWarehouseParam> param, out int total);
		WmsWarehouseEntity? GetById(long id);
		bool Save(WmsWarehouseEntity model);
		bool Delete(List<long> ids);
		bool ChangeStatus(WmsWarehouseEntity model);
	}
}
