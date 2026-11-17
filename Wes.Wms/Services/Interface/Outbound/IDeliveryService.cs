using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 出库单 数据访问接口
	/// </summary>
	public interface IDeliveryService
	{
		List<WmsDeliveryEntity> GetList(ParamData<WmsDeliveryParam> param, out int total);
		WmsDeliveryEntity? GetById(long id);
		bool Save(WmsDeliveryEntity model);
		bool Delete(List<long> ids);
		bool DeleteDetailsByDeliveryId(long deliveryId);
		bool BatchSaveDetails(long deliveryId, List<WmsDeliveryDetailEntity> details);
	}
}
