using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 退货单 数据访问接口
	/// </summary>
	public interface IReturnOrderService
	{
		List<WmsReturnOrderEntity> GetList(ParamData<WmsReturnOrderParam> param, out int total);
		WmsReturnOrderEntity? GetById(long id);
		bool Save(WmsReturnOrderEntity model);
		bool Delete(List<long> ids);
	}
}
