using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 往来单位联系人
	/// </summary>
	[ApiController]
	[Route("wms/partnerContact")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class PartnerContactController : ControllerBase
	{
		private readonly IPartnerContactBiz _partnerContactBiz;

		public PartnerContactController(IPartnerContactBiz partnerContactBiz)
		{
			_partnerContactBiz = partnerContactBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsPartnerContactParam> param)
		{
			return _partnerContactBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _partnerContactBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsPartnerContactEntity model)
		{
			model.ContactId = 0;
			return _partnerContactBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsPartnerContactEntity model)
		{
			return _partnerContactBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _partnerContactBiz.Delete(ids);
		}
	}
}
