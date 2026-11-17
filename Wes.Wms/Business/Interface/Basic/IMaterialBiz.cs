using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 物料 业务逻辑接口
	/// </summary>
	public interface IMaterialBiz
	{
		RowData<WmsMaterialEntity> GetList(ParamData<WmsMaterialParam> param);
		ResultData<WmsMaterialEntity> GetById(long id);
		ResultData<long> Save(WmsMaterialEntity model);
		ReturnData Delete(string ids);
		ReturnData ChangeStatus(WmsMaterialEntity model);
	}
}
