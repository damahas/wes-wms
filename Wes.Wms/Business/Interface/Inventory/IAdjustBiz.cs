using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存调整 业务逻辑接口
	/// </summary>
	public interface IAdjustBiz
	{
		RowData<WmsAdjustEntity> GetList(ParamData<WmsAdjustParam> param);
		ResultData<WmsAdjustEntity> GetById(long id);
		ReturnData Save(WmsAdjustEntity model);

		/// <summary>执行调整单：更新实时库存并写库存流水，单据置为已完成</summary>
		ReturnData Execute(long adjustId);

		ReturnData Delete(string ids);
	}
}
