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
	/// 往来单位联系人 业务逻辑
	/// </summary>
	public class PartnerContactBiz : IPartnerContactBiz
	{
		private readonly IPartnerContactService _partnerContactService;

		public PartnerContactBiz(IPartnerContactService partnerContactService)
		{
			_partnerContactService = partnerContactService;
		}

		public RowData<WmsPartnerContactEntity> GetList(ParamData<WmsPartnerContactParam> param)
		{
			int total = 0;
			var result = new RowData<WmsPartnerContactEntity>(_partnerContactService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsPartnerContactEntity> GetById(long id)
		{
			return new ResultData<WmsPartnerContactEntity>(_partnerContactService.GetById(id));
		}

		public ReturnData Save(WmsPartnerContactEntity model)
		{
			return _partnerContactService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _partnerContactService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}
	}
}
