using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库位
	/// </summary>
	[ApiController]
	[Route("wms/location")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class LocationController : ControllerBase
	{
		private readonly ILocationBiz _locationBiz;

		public LocationController(ILocationBiz locationBiz)
		{
			_locationBiz = locationBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsLocationParam> param)
		{
			return _locationBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _locationBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsLocationEntity model)
		{
			model.LocationId = 0;
			return _locationBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsLocationEntity model)
		{
			return _locationBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _locationBiz.Delete(ids);
		}

		[HttpPut("changeStatus")]
		public ReturnData ChangeStatus([FromBody] WmsLocationEntity model)
		{
			return _locationBiz.ChangeStatus(model);
		}
	}
}
