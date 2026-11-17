using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 缺料看板 业务逻辑接口
	/// </summary>
	public interface IShortageBiz
	{
		RowData<WmsShortageVo> GetList(ParamData<WmsShortageParam> param);
		ResultData<ShortageSummaryVo> Summary(WmsShortageParam param);
	}
}
