using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库存锁定
	/// </summary>
	[ApiController]
	[Route("wms/lock")]
	[ApiExplorerSettings(GroupName = "Inventory")]
	public class LockController : ControllerBase
	{
		private readonly ILockBiz _lockBiz;

		public LockController(ILockBiz lockBiz)
		{
			_lockBiz = lockBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsLockParam> param)
		{
			return _lockBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _lockBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsLockEntity model)
		{
			model.LockId = 0;
			return _lockBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsLockEntity model)
		{
			return _lockBiz.Save(model);
		}

		/// <summary>执行锁定（库存锁定数量增加、可用数量减少）</summary>
		[HttpPost("{id}/execute")]
		public ReturnData Execute(long id)
		{
			return _lockBiz.Execute(id);
		}

		/// <summary>释放锁定（未使用部分回补可用数量）</summary>
		[HttpPost("{id}/release")]
		public ReturnData Release(long id)
		{
			return _lockBiz.Release(id);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _lockBiz.Delete(ids);
		}
	}
}
