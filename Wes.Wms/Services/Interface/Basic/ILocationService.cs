using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库位 数据访问接口
	/// </summary>
	public interface ILocationService
	{
		List<WmsLocationEntity> GetList(ParamData<WmsLocationParam> param, out int total);
		WmsLocationEntity? GetById(long id);
		bool Save(WmsLocationEntity model);
		bool ChangeStatus(WmsLocationEntity model);
		bool Delete(List<long> ids);
	}
}
