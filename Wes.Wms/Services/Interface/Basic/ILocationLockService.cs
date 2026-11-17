using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库位锁定 数据访问接口
	/// </summary>
	public interface ILocationLockService
	{
		List<WmsLocationLockEntity> GetList(ParamData<WmsLocationLockParam> param, out int total);
		WmsLocationLockEntity? GetById(long id);
		bool Save(WmsLocationLockEntity model);
		bool Delete(List<long> ids);
	}
}
