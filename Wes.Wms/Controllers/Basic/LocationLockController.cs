using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库位锁定
	/// </summary>
	[ApiController]
	[Route("wms/locationLock")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class LocationLockController : ControllerBase
	{
		private readonly ILocationLockBiz _locationLockBiz;

		public LocationLockController(ILocationLockBiz locationLockBiz)
		{
			_locationLockBiz = locationLockBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsLocationLockParam> param)
		{
			return _locationLockBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _locationLockBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsLocationLockEntity model)
		{
			model.LockId = 0;
			return _locationLockBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsLocationLockEntity model)
		{
			return _locationLockBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _locationLockBiz.Delete(ids);
		}
	}
}
