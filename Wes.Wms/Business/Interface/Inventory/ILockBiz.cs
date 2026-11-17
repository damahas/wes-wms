using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存锁定 业务逻辑接口
	/// </summary>
	public interface ILockBiz
	{
		RowData<WmsLockEntity> GetList(ParamData<WmsLockParam> param);
		ResultData<WmsLockEntity> GetById(long id);
		ReturnData Save(WmsLockEntity model);

		/// <summary>执行锁定：库存锁定数量增加、可用数量减少</summary>
		ReturnData Execute(long lockId);

		/// <summary>释放锁定：未使用部分回补可用数量</summary>
		ReturnData Release(long lockId);

		ReturnData Delete(string ids);
	}
}
