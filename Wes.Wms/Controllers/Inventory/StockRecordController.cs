using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 库存流水（只读）
	/// 账务流水为审计记录，只能由业务单据在事务内写入，不提供手工新增/修改/删除。
	/// </summary>
	[ApiController]
	[Route("wms/stockRecord")]
	[ApiExplorerSettings(GroupName = "Inventory")]
	public class StockRecordController : ControllerBase
	{
		private readonly IStockRecordBiz _stockRecordBiz;

		public StockRecordController(IStockRecordBiz stockRecordBiz)
		{
			_stockRecordBiz = stockRecordBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsStockRecordParam> param)
		{
			return _stockRecordBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _stockRecordBiz.GetById(id);
		}
	}
}
