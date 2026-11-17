using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 仓库
	/// </summary>
	[ApiController]
	[Route("wms/warehouse")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class WarehouseController : ControllerBase
	{
		private readonly IWarehouseBiz _warehouseBiz;

		public WarehouseController(IWarehouseBiz warehouseBiz)
		{
			_warehouseBiz = warehouseBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsWarehouseParam> param)
		{
			return _warehouseBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _warehouseBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsWarehouseEntity model)
		{
			model.WarehouseId = 0;
			return _warehouseBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsWarehouseEntity model)
		{
			return _warehouseBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _warehouseBiz.Delete(ids);
		}

		[HttpPut("changeStatus")]
		public ReturnData ChangeStatus([FromBody] WmsWarehouseEntity model)
		{
			return _warehouseBiz.ChangeStatus(model);
		}
	}
}
