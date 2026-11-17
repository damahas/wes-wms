using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库存预警
	/// </summary>
	[ApiController]
	[Route("wms/stockWarning")]
	[ApiExplorerSettings(GroupName = "Inventory")]
	public class StockWarningController : ControllerBase
	{
		private readonly IStockWarningBiz _stockWarningBiz;

		public StockWarningController(IStockWarningBiz stockWarningBiz)
		{
			_stockWarningBiz = stockWarningBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsStockWarningParam> param)
		{
			return _stockWarningBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _stockWarningBiz.GetById(id);
		}

		/// <summary>预警周期内的触发历史（每天一条快照）</summary>
		[HttpGet("logs/{id}")]
		public ReturnData GetLogs(long id)
		{
			return _stockWarningBiz.GetLogs(id);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _stockWarningBiz.Delete(ids);
		}

		/// <summary>手动跑一遍预警（定时任务也会跑）</summary>
		[HttpPost("run")]
		public ReturnData Run()
		{
			return _stockWarningBiz.Run();
		}

		/// <summary>处理 / 忽略预警（status: handled-已处理 ignored-已忽略）</summary>
		[HttpPut("handle/{id}")]
		public ReturnData Handle(long id, [FromQuery] string status, [FromQuery] string remark)
		{
			return _stockWarningBiz.Handle(id, status, remark);
		}
	}
}
