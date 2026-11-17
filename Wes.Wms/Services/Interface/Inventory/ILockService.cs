using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存锁定 数据访问接口
	/// </summary>
	public interface ILockService
	{
		List<WmsLockEntity> GetList(ParamData<WmsLockParam> param, out int total);
		WmsLockEntity? GetById(long id);
		bool Save(WmsLockEntity model);
		bool Delete(List<long> ids);
	}
}
