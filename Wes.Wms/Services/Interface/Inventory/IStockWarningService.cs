using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存预警 数据访问接口
	/// </summary>
	public interface IStockWarningService
	{
		List<WmsStockWarningEntity> GetList(ParamData<WmsStockWarningParam> param, out int total);
		WmsStockWarningEntity? GetById(long id);
		bool Save(WmsStockWarningEntity model);
		bool Delete(List<long> ids);
	}
}
