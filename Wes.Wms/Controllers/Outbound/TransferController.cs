using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 调拨单（跨仓调拨，库内搬货请用移库单）
	/// </summary>
	[ApiController]
	[Route("wms/transfer")]
	[ApiExplorerSettings(GroupName = "Outbound")]
	public class TransferController : ControllerBase
	{
		private readonly ITransferBiz _transferBiz;

		public TransferController(ITransferBiz transferBiz)
		{
			_transferBiz = transferBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsTransferParam> param)
		{
			return _transferBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _transferBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsTransferEntity model)
		{
			model.TransferId = 0;
			return _transferBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsTransferEntity model)
		{
			return _transferBiz.Save(model);
		}

		/// <summary>移出：调出仓扣库存并记在途，由调出仓责任人操作</summary>
		[HttpPost("{id}/out")]
		public ReturnData TransferOut(long id)
		{
			return _transferBiz.TransferOut(id);
		}

		/// <summary>移入：调入仓按实收数量入账并消在途，由调入仓责任人操作</summary>
		[HttpPost("in")]
		public ReturnData TransferIn([FromBody] TransferInDto dto)
		{
			return _transferBiz.TransferIn(dto);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _transferBiz.Delete(ids);
		}
	}
}
