using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 退货单 业务逻辑接口
	/// </summary>
	public interface IReturnOrderBiz
	{
		RowData<WmsReturnOrderEntity> GetList(ParamData<WmsReturnOrderParam> param);
		ResultData<WmsReturnOrderEntity> GetById(long id);
		ResultData<List<WmsReturnOrderDetailEntity>> GetDetailList(long returnOrderId);
		ReturnData Save(WmsReturnOrderSaveDto dto);
		ReturnData Delete(string ids);
	}
}
