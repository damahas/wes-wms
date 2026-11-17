using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 往来单位地址 业务逻辑接口
	/// </summary>
	public interface IPartnerAddressBiz
	{
		RowData<WmsPartnerAddressEntity> GetList(ParamData<WmsPartnerAddressParam> param);
		ResultData<WmsPartnerAddressEntity> GetById(long id);
		ReturnData Save(WmsPartnerAddressEntity model);
		ReturnData Delete(string ids);
	}
}
