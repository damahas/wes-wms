using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库位 业务逻辑接口
	/// </summary>
	public interface ILocationBiz
	{
		RowData<WmsLocationEntity> GetList(ParamData<WmsLocationParam> param);
		ResultData<WmsLocationEntity> GetById(long id);
		ReturnData Save(WmsLocationEntity model);
		ReturnData ChangeStatus(WmsLocationEntity model);
		ReturnData Delete(string ids);
	}
}
