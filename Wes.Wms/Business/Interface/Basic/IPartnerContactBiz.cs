using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 往来单位联系人 业务逻辑接口
	/// </summary>
	public interface IPartnerContactBiz
	{
		RowData<WmsPartnerContactEntity> GetList(ParamData<WmsPartnerContactParam> param);
		ResultData<WmsPartnerContactEntity> GetById(long id);
		ReturnData Save(WmsPartnerContactEntity model);
		ReturnData Delete(string ids);
	}
}
