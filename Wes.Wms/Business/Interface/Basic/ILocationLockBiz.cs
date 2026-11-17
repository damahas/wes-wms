using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库位锁定 业务逻辑接口
	/// </summary>
	public interface ILocationLockBiz
	{
		RowData<WmsLocationLockEntity> GetList(ParamData<WmsLocationLockParam> param);
		ResultData<WmsLocationLockEntity> GetById(long id);
		ReturnData Save(WmsLocationLockEntity model);
		ReturnData Delete(string ids);
	}
}
