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
	/// 库位 业务逻辑
	/// </summary>
	public class LocationBiz : ILocationBiz
	{
		private readonly ILocationService _locationService;

		public LocationBiz(ILocationService locationService)
		{
			_locationService = locationService;
		}

		public RowData<WmsLocationEntity> GetList(ParamData<WmsLocationParam> param)
		{
			int total = 0;
			var result = new RowData<WmsLocationEntity>(_locationService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsLocationEntity> GetById(long id)
		{
			return new ResultData<WmsLocationEntity>(_locationService.GetById(id));
		}

		public ReturnData Save(WmsLocationEntity model)
		{
			return _locationService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _locationService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}

		public ReturnData ChangeStatus(WmsLocationEntity model)
		{
			return _locationService.ChangeStatus(model)
				? new ReturnData()
				: new ReturnData(500, "状态更新失败！");
		}
	}
}
