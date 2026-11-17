using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 物料
	/// </summary>
	[ApiController]
	[Route("wms/material")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class MaterialController : ControllerBase
	{
		private readonly IMaterialBiz _materialBiz;

		public MaterialController(IMaterialBiz materialBiz)
		{
			_materialBiz = materialBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsMaterialParam> param)
		{
			return _materialBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _materialBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsMaterialEntity model)
		{
			model.MaterialId = 0;
			return _materialBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsMaterialEntity model)
		{
			return _materialBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _materialBiz.Delete(ids);
		}

		[HttpPut("changeStatus")]
		public ReturnData ChangeStatus([FromBody] WmsMaterialEntity model)
		{
			return _materialBiz.ChangeStatus(model);
		}
	}
}
