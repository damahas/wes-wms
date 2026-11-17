using System;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 往来单位地址 业务逻辑
	/// </summary>
	public class PartnerAddressBiz : IPartnerAddressBiz
	{
		private readonly IPartnerAddressService _partnerAddressService;

		public PartnerAddressBiz(IPartnerAddressService partnerAddressService)
		{
			_partnerAddressService = partnerAddressService;
		}

		public RowData<WmsPartnerAddressEntity> GetList(ParamData<WmsPartnerAddressParam> param)
		{
			int total = 0;
			var result = new RowData<WmsPartnerAddressEntity>(_partnerAddressService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsPartnerAddressEntity> GetById(long id)
		{
			return new ResultData<WmsPartnerAddressEntity>(_partnerAddressService.GetById(id));
		}

		public ReturnData Save(WmsPartnerAddressEntity model)
		{
			return _partnerAddressService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _partnerAddressService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}
	}
}
