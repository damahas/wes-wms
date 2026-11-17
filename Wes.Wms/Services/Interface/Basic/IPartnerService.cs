using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 往来单位 数据访问接口
	/// </summary>
	public interface IPartnerService
	{
		List<WmsPartnerEntity> GetList(ParamData<WmsPartnerParam> param, out int total);
		WmsPartnerEntity? GetById(long id);
		bool Save(WmsPartnerEntity model);
		bool Delete(List<long> ids);
		bool ChangeStatus(WmsPartnerEntity model);
	}
}
