using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 实时库存（只读）
	/// 库存只能由入库上架、出库、移库、盘点、库存调整等业务单据驱动，
	/// 不提供新增/修改/删除接口，避免绕过业务规则直接改账。
	/// </summary>
	[ApiController]
	[Route("wms/stock")]
	[ApiExplorerSettings(GroupName = "Inventory")]
	public class StockController : ControllerBase
	{
		private readonly IStockBiz _stockBiz;

		public StockController(IStockBiz stockBiz)
		{
			_stockBiz = stockBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsStockParam> param)
		{
			return _stockBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _stockBiz.GetById(id);
		}
	}
}
