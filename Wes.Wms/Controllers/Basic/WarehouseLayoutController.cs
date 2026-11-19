using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 仓库平面布局
	/// </summary>
	[ApiController]
	[Route("wms/warehouseLayout")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class WarehouseLayoutController : ControllerBase
	{
		private readonly IWarehouseLayoutBiz _warehouseLayoutBiz;

		public WarehouseLayoutController(IWarehouseLayoutBiz warehouseLayoutBiz)
		{
			_warehouseLayoutBiz = warehouseLayoutBiz;
		}

		[HttpGet("{warehouseId}")]
		public ReturnData GetByWarehouse(long warehouseId)
		{
			return _warehouseLayoutBiz.GetByWarehouse(warehouseId);
		}

		[HttpPut]
		public ReturnData Save([FromBody] WmsWarehouseLayoutEntity model)
		{
			return _warehouseLayoutBiz.Save(model);
		}

		[HttpPut("publish/{id}")]
		public ReturnData Publish(long id)
		{
			return _warehouseLayoutBiz.Publish(id);
		}
	}
}
