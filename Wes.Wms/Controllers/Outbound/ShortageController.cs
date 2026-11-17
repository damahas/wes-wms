using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 缺料看板
	/// </summary>
	[ApiController]
	[Route("wms/shortage")]
	[ApiExplorerSettings(GroupName = "Outbound")]
	public class ShortageController : ControllerBase
	{
		private readonly IShortageBiz _shortageBiz;

		public ShortageController(IShortageBiz shortageBiz)
		{
			_shortageBiz = shortageBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsShortageParam> param)
		{
			return _shortageBiz.GetList(param);
		}

		[HttpGet("summary")]
		public ResultData<ShortageSummaryVo> Summary([FromQuery] WmsShortageParam param)
		{
			return _shortageBiz.Summary(param);
		}
	}
}
