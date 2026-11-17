using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 移库单 业务逻辑接口
	/// </summary>
	public interface IMoveBiz
	{
		RowData<WmsMoveEntity> GetList(ParamData<WmsMoveParam> param);
		ResultData<WmsMoveEntity> GetById(long id);
		ReturnData Save(WmsMoveEntity model);

		/// <summary>执行移库：源库位扣减、目标库位增加，并写移出/移入流水</summary>
		ReturnData Execute(long moveId);

		ReturnData Delete(string ids);
	}
}
