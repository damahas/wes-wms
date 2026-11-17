using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
    /// <summary>
    /// 收货单 数据访问接口
    /// </summary>
	public interface IReceivingService
	{
		List<WmsReceivingEntity> GetList(ParamData<WmsReceivingDetailParam> param, out int total);
		WmsReceivingEntity? GetById(long id);
		bool Save(WmsReceivingEntity model);
		bool Delete(List<long> ids);
		bool CompleteReceipt(long receivingId);
		bool UpdateStatus(long receivingId, string status);
		List<WmsReceivingEntity> GetList(List<long> ids);
	}
}
