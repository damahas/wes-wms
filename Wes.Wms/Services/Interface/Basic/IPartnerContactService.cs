using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 往来单位联系人 数据访问接口
	/// </summary>
	public interface IPartnerContactService
	{
		List<WmsPartnerContactEntity> GetList(ParamData<WmsPartnerContactParam> param, out int total);
		WmsPartnerContactEntity? GetById(long id);
		bool Save(WmsPartnerContactEntity model);
		bool Delete(List<long> ids);
	}
}
