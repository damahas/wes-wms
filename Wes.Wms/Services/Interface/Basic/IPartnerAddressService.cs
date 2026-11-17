using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 往来单位地址 数据访问接口
	/// </summary>
	public interface IPartnerAddressService
	{
		List<WmsPartnerAddressEntity> GetList(ParamData<WmsPartnerAddressParam> param, out int total);
		WmsPartnerAddressEntity? GetById(long id);
		bool Save(WmsPartnerAddressEntity model);
		bool Delete(List<long> ids);
	}
}
