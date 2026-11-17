using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 领料单
	/// </summary>
	[ApiController]
	[Route("wms/issueOrder")]
	[ApiExplorerSettings(GroupName = "Outbound")]
	public class IssueOrderController : ControllerBase
	{
		private readonly IIssueOrderBiz _issueOrderBiz;

		public IssueOrderController(IIssueOrderBiz issueOrderBiz)
		{
			_issueOrderBiz = issueOrderBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsIssueOrderParam> param)
		{
			return _issueOrderBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _issueOrderBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsIssueOrderEntity model)
		{
			model.IssueId = 0;
			return _issueOrderBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsIssueOrderEntity model)
		{
			return _issueOrderBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _issueOrderBiz.Delete(ids);
		}

		/// <summary>
		/// 发料：按 FEFO/FIFO 有多少发多少，生成出库单并扣库存（支持多次发料）
		/// </summary>
		[HttpPost("{id}/issue")]
		public ReturnData Issue(long id)
		{
			return _issueOrderBiz.Issue(id);
		}

		/// <summary>
		/// 取消领料单（未发过料的才能取消）
		/// </summary>
		[HttpPost("{id}/cancel")]
		public ReturnData Cancel(long id)
		{
			return _issueOrderBiz.Cancel(id);
		}
	}
}
