using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库区 数据访问接口
	/// </summary>
	public interface IZoneService
	{
		List<WmsZoneEntity> GetList(ParamData<WmsZoneParam> param, out int total);
		WmsZoneEntity? GetById(long id);
		bool Save(WmsZoneEntity model);
		bool ChangeStatus(WmsZoneEntity model);
		bool Delete(List<long> ids);
	}
}
