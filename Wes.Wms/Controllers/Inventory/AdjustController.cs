using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库存调整
	/// </summary>
	[ApiController]
	[Route("wms/adjust")]
	[ApiExplorerSettings(GroupName = "Inventory")]
	public class AdjustController : ControllerBase
	{
		private readonly IAdjustBiz _adjustBiz;

		public AdjustController(IAdjustBiz adjustBiz)
		{
			_adjustBiz = adjustBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsAdjustParam> param)
		{
			return _adjustBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _adjustBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsAdjustEntity model)
		{
			model.AdjustId = 0;
			return _adjustBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsAdjustEntity model)
		{
			return _adjustBiz.Save(model);
		}

		/// <summary>执行调整单（更新库存 + 写库存流水）</summary>
		[HttpPost("{id}/execute")]
		public ReturnData Execute(long id)
		{
			return _adjustBiz.Execute(id);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _adjustBiz.Delete(ids);
		}
	}
}
