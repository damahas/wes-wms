using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 移库单
	/// </summary>
	[ApiController]
	[Route("wms/move")]
	[ApiExplorerSettings(GroupName = "Outbound")]
	public class MoveController : ControllerBase
	{
		private readonly IMoveBiz _moveBiz;

		public MoveController(IMoveBiz moveBiz)
		{
			_moveBiz = moveBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsMoveParam> param)
		{
			return _moveBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _moveBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsMoveEntity model)
		{
			model.MoveId = 0;
			return _moveBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsMoveEntity model)
		{
			return _moveBiz.Save(model);
		}

		/// <summary>执行移库（源库位扣减、目标库位增加，并写移出/移入流水）</summary>
		[HttpPost("{id}/execute")]
		public ReturnData Execute(long id)
		{
			return _moveBiz.Execute(id);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _moveBiz.Delete(ids);
		}
	}
}
