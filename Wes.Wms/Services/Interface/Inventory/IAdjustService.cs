using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存调整 数据访问接口
	/// </summary>
	public interface IAdjustService
	{
		List<WmsAdjustEntity> GetList(ParamData<WmsAdjustParam> param, out int total);
		WmsAdjustEntity? GetById(long id);
		bool Save(WmsAdjustEntity model);
		bool Delete(List<long> ids);
		bool DeleteDetailsByAdjustId(long adjustId);
		bool BatchSaveDetails(long adjustId, List<WmsAdjustDetailEntity> details);
	}
}
