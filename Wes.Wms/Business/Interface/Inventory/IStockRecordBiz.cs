using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存流水 业务逻辑接口
	/// </summary>
	public interface IStockRecordBiz
	{
		RowData<WmsStockRecordEntity> GetList(ParamData<WmsStockRecordParam> param);
		ResultData<WmsStockRecordEntity> GetById(long id);
		ReturnData Save(WmsStockRecordEntity model);
		ReturnData Delete(string ids);
	}
}
