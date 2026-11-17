using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库存冻结
	/// </summary>
	[ApiController]
	[Route("wms/freeze")]
	[ApiExplorerSettings(GroupName = "Inventory")]
	public class FreezeController : ControllerBase
	{
		private readonly IFreezeBiz _freezeBiz;

		public FreezeController(IFreezeBiz freezeBiz)
		{
			_freezeBiz = freezeBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsFreezeParam> param)
		{
			return _freezeBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _freezeBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsFreezeEntity model)
		{
			model.FreezeId = 0;
			return _freezeBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsFreezeEntity model)
		{
			return _freezeBiz.Save(model);
		}

		/// <summary>执行冻结（库存冻结数量增加、可用数量减少）</summary>
		[HttpPost("{id}/execute")]
		public ReturnData Execute(long id)
		{
			return _freezeBiz.Execute(id);
		}

		/// <summary>解冻（库存冻结数量减少、可用数量回补）</summary>
		[HttpPost("{id}/thaw")]
		public ReturnData Thaw(long id, [FromQuery] string thawReason)
		{
			return _freezeBiz.Thaw(id, thawReason);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _freezeBiz.Delete(ids);
		}
	}
}
