using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 物料 数据访问接口
	/// </summary>
	public interface IMaterialService
	{
		List<WmsMaterialEntity> GetList(ParamData<WmsMaterialParam> param, out int total);
		WmsMaterialEntity? GetById(long id);
		bool Save(WmsMaterialEntity model);
		bool Delete(List<long> ids);
		bool ChangeStatus(WmsMaterialEntity model);
	}
}
