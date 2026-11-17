using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库区 业务逻辑接口
	/// </summary>
	public interface IZoneBiz
	{
		RowData<WmsZoneEntity> GetList(ParamData<WmsZoneParam> param);
		ResultData<WmsZoneEntity> GetById(long id);
		ReturnData Save(WmsZoneEntity model);
		ReturnData ChangeStatus(WmsZoneEntity model);
		ReturnData Delete(string ids);
	}
}
