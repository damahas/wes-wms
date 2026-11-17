using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 退货单
	/// </summary>
	[ApiController]
	[Route("wms/returnOrder")]
	[ApiExplorerSettings(GroupName = "Inbound")]
	public class ReturnOrderController : ControllerBase
	{
		private readonly IReturnOrderBiz _returnOrderBiz;

		public ReturnOrderController(IReturnOrderBiz returnOrderBiz)
		{
			_returnOrderBiz = returnOrderBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsReturnOrderParam> param)
		{
			return _returnOrderBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _returnOrderBiz.GetById(id);
		}

		[HttpGet("detailList/{returnOrderId}")]
		public ReturnData GetDetailList(long returnOrderId)
		{
			return _returnOrderBiz.GetDetailList(returnOrderId);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsReturnOrderSaveDto dto)
		{
			if (dto != null && dto.ReturnOrder != null)
				dto.ReturnOrder.ReturnOrderId = 0;
			return _returnOrderBiz.Save(dto);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsReturnOrderSaveDto dto)
		{
			return _returnOrderBiz.Save(dto);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _returnOrderBiz.Delete(ids);
		}
	}
}
