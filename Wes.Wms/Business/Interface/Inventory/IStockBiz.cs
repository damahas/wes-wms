using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 实时库存 业务逻辑接口
	/// </summary>
	public interface IStockBiz
	{
		RowData<WmsStockEntity> GetList(ParamData<WmsStockParam> param);
		ResultData<WmsStockEntity> GetById(long id);
		ReturnData Save(WmsStockEntity model);
		ReturnData Delete(string ids);
	}
}
