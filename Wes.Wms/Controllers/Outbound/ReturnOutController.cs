using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 出库退货
	/// </summary>
	[ApiController]
	[Route("wms/returnOut")]
	[ApiExplorerSettings(GroupName = "Outbound")]
	public class ReturnOutController : ControllerBase
	{
		private readonly IReturnOutBiz _returnOutBiz;

		public ReturnOutController(IReturnOutBiz returnOutBiz)
		{
			_returnOutBiz = returnOutBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsReturnOutParam> param)
		{
			return _returnOutBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _returnOutBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsReturnOutEntity model)
		{
			model.ReturnId = 0;
			return _returnOutBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsReturnOutEntity model)
		{
			return _returnOutBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _returnOutBiz.Delete(ids);
		}
	}
}
