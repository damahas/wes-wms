using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存预警配置 业务逻辑接口
	/// </summary>
	public interface IStockAlertConfigBiz
	{
		RowData<WmsStockAlertConfigEntity> GetList(ParamData<WmsStockAlertConfigParam> param);
		ResultData<WmsStockAlertConfigEntity> GetById(long id);
		ReturnData Save(WmsStockAlertConfigEntity model);
		ReturnData Delete(string ids);
	}
}
