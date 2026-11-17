using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库区
	/// </summary>
	[ApiController]
	[Route("wms/zone")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class ZoneController : ControllerBase
	{
		private readonly IZoneBiz _zoneBiz;

		public ZoneController(IZoneBiz zoneBiz)
		{
			_zoneBiz = zoneBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsZoneParam> param)
		{
			return _zoneBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _zoneBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsZoneEntity model)
		{
			model.ZoneId = 0;
			return _zoneBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsZoneEntity model)
		{
			return _zoneBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _zoneBiz.Delete(ids);
		}

		[HttpPut("changeStatus")]
		public ReturnData ChangeStatus([FromBody] WmsZoneEntity model)
		{
			return _zoneBiz.ChangeStatus(model);
		}
	}
}
