using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 包装规格
	/// </summary>
	[ApiController]
	[Route("wms/packSpec")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class PackSpecController : ControllerBase
	{
		private readonly IPackSpecBiz _packSpecBiz;

		public PackSpecController(IPackSpecBiz packSpecBiz)
		{
			_packSpecBiz = packSpecBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsPackSpecParam> param)
		{
			return _packSpecBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _packSpecBiz.GetById(id);
		}

		/// <summary>
		/// 获取某包装规格整套层级树（按 root_id 归组的所有层级）
		/// </summary>
		[HttpGet("tree")]
		public ReturnData GetTree(long rootId)
		{
			return _packSpecBiz.GetTree(rootId);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsPackSpecEntity model)
		{
			model.SpecId = 0;
			_packSpecBiz.Save(model);
			// 返回新生成的实体（含 spec_id / root_id），便于前端新建后直接进入树管理
			return _packSpecBiz.GetById(model.SpecId);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsPackSpecEntity model)
		{
			return _packSpecBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _packSpecBiz.Delete(ids);
		}
	}
}
