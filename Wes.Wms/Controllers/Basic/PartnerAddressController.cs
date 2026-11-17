using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 往来单位地址
	/// </summary>
	[ApiController]
	[Route("wms/partnerAddress")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class PartnerAddressController : ControllerBase
	{
		private readonly IPartnerAddressBiz _partnerAddressBiz;

		public PartnerAddressController(IPartnerAddressBiz partnerAddressBiz)
		{
			_partnerAddressBiz = partnerAddressBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsPartnerAddressParam> param)
		{
			return _partnerAddressBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _partnerAddressBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsPartnerAddressEntity model)
		{
			model.AddressId = 0;
			return _partnerAddressBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsPartnerAddressEntity model)
		{
			return _partnerAddressBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _partnerAddressBiz.Delete(ids);
		}
	}
}
