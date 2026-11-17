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
	/// 库区 业务逻辑
	/// </summary>
	public class ZoneBiz : IZoneBiz
	{
		private readonly IZoneService _zoneService;

		public ZoneBiz(IZoneService zoneService)
		{
			_zoneService = zoneService;
		}

		public RowData<WmsZoneEntity> GetList(ParamData<WmsZoneParam> param)
		{
			int total = 0;
			var result = new RowData<WmsZoneEntity>(_zoneService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsZoneEntity> GetById(long id)
		{
			return new ResultData<WmsZoneEntity>(_zoneService.GetById(id));
		}

		public ReturnData Save(WmsZoneEntity model)
		{
			return _zoneService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _zoneService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}

		public ReturnData ChangeStatus(WmsZoneEntity model)
		{
			return _zoneService.ChangeStatus(model)
				? new ReturnData()
				: new ReturnData(500, "状态更新失败！");
		}
	}
}
