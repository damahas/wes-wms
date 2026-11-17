using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存预警 业务逻辑接口
	/// </summary>
	public interface IStockWarningBiz
	{
		RowData<WmsStockWarningEntity> GetList(ParamData<WmsStockWarningParam> param);
		ResultData<WmsStockWarningEntity> GetById(long id);
		ReturnData Delete(string ids);

		/// <summary>预警周期内的触发历史（每天一条快照）</summary>
		ResultData<List<WmsStockWarningLogEntity>> GetLogs(long warningId);

		/// <summary>跑一遍预警引擎（定时任务 / 手动触发）</summary>
		ReturnData Run();

		/// <summary>处理 / 忽略预警（status: handled-已处理 ignored-已忽略）</summary>
		ReturnData Handle(long warningId, string status, string remark);
	}
}
