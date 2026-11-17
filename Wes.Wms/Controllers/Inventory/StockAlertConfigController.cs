using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库存预警配置
	/// </summary>
	[ApiController]
	[Route("wms/stockWarningConfig")]
	[ApiExplorerSettings(GroupName = "Inventory")]
	public class StockAlertConfigController : ControllerBase
	{
		private readonly IStockAlertConfigBiz _configBiz;

		public StockAlertConfigController(IStockAlertConfigBiz configBiz)
		{
			_configBiz = configBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsStockAlertConfigParam> param)
		{
			return _configBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _configBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsStockAlertConfigEntity model)
		{
			model.ConfigId = 0;
			return _configBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsStockAlertConfigEntity model)
		{
			return _configBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _configBiz.Delete(ids);
		}
	}
}
