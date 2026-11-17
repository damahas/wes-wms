using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 出库单 业务逻辑接口
	/// </summary>
	public interface IDeliveryBiz
	{
		RowData<WmsDeliveryEntity> GetList(ParamData<WmsDeliveryParam> param);
		ResultData<WmsDeliveryEntity> GetById(long id);
		ReturnData Save(WmsDeliveryEntity model);

		/// <summary>执行出库（发货）：扣减库存可用量 + 写库存流水流水</summary>
		ReturnData Execute(long deliveryId);

		ReturnData Delete(string ids);
	}
}
