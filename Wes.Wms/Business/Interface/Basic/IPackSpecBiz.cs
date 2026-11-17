using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 包装规格 业务逻辑接口
	/// </summary>
	public interface IPackSpecBiz
	{
		RowData<WmsPackSpecEntity> GetList(ParamData<WmsPackSpecParam> param);
		RowData<WmsPackSpecEntity> GetTree(long rootId);
		ResultData<WmsPackSpecEntity> GetById(long id);
		ReturnData Save(WmsPackSpecEntity model);
		ReturnData Delete(string ids);
	}
}
