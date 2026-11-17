using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 出库单
	/// </summary>
	[ApiController]
	[Route("wms/delivery")]
	[ApiExplorerSettings(GroupName = "Outbound")]
	public class DeliveryController : ControllerBase
	{
		private readonly IDeliveryBiz _deliveryBiz;

		public DeliveryController(IDeliveryBiz deliveryBiz)
		{
			_deliveryBiz = deliveryBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsDeliveryParam> param)
		{
			return _deliveryBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _deliveryBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsDeliveryEntity model)
		{
			model.DeliveryId = 0;
			return _deliveryBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsDeliveryEntity model)
		{
			return _deliveryBiz.Save(model);
		}

		/// <summary>执行出库/发货（扣减库存可用量 + 写出库流水）</summary>
		[HttpPost("{id}/execute")]
		public ReturnData Execute(long id)
		{
			return _deliveryBiz.Execute(id);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _deliveryBiz.Delete(ids);
		}
	}
}
