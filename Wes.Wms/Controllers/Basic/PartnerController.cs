using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 往来单位
	/// </summary>
	[ApiController]
	[Route("wms/partner")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class PartnerController : ControllerBase
	{
		private readonly IPartnerBiz _partnerBiz;

		public PartnerController(IPartnerBiz partnerBiz)
		{
			_partnerBiz = partnerBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsPartnerParam> param)
		{
			return _partnerBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _partnerBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsPartnerEntity model)
		{
			model.PartnerId = 0;
			return _partnerBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsPartnerEntity model)
		{
			return _partnerBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _partnerBiz.Delete(ids);
		}

		[HttpPut("changeStatus")]
		public ReturnData ChangeStatus([FromBody] WmsPartnerEntity model)
		{
			return _partnerBiz.ChangeStatus(model);
		}
	}
}
