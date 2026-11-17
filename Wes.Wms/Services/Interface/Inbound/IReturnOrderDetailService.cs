using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 退货单明细 数据访问接口
	/// </summary>
	public interface IReturnOrderDetailService
	{
		List<WmsReturnOrderDetailEntity> GetListByReturnOrderId(long returnOrderId);
		bool DeleteByReturnOrderId(long returnOrderId);
		void BatchSave(List<WmsReturnOrderDetailEntity> details);
	}
}
