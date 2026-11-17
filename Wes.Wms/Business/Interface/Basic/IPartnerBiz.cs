using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 往来单位 业务逻辑接口
	/// </summary>
	public interface IPartnerBiz
	{
		RowData<WmsPartnerEntity> GetList(ParamData<WmsPartnerParam> param);
		ResultData<WmsPartnerEntity> GetById(long id);
		ReturnData Save(WmsPartnerEntity model);
		ReturnData Delete(string ids);
		ReturnData ChangeStatus(WmsPartnerEntity model);
	}
}
