using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存流水 数据访问接口
	/// </summary>
	public interface IStockRecordService
	{
		List<WmsStockRecordEntity> GetList(ParamData<WmsStockRecordParam> param, out int total);
		WmsStockRecordEntity? GetById(long id);
		bool Save(WmsStockRecordEntity model);
		bool Delete(List<long> ids);
	}
}
