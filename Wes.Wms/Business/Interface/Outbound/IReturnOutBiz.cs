using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 出库退货 业务逻辑接口
	/// </summary>
	public interface IReturnOutBiz
	{
		RowData<WmsReturnOutEntity> GetList(ParamData<WmsReturnOutParam> param);
		ResultData<WmsReturnOutEntity> GetById(long id);
		ReturnData Save(WmsReturnOutEntity model);
		ReturnData Delete(string ids);
	}
}
