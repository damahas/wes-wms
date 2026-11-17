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
	/// 库位锁定 业务逻辑
	/// </summary>
	public class LocationLockBiz : ILocationLockBiz
	{
		private readonly ILocationLockService _locationLockService;

		public LocationLockBiz(ILocationLockService locationLockService)
		{
			_locationLockService = locationLockService;
		}

		public RowData<WmsLocationLockEntity> GetList(ParamData<WmsLocationLockParam> param)
		{
			int total = 0;
			var result = new RowData<WmsLocationLockEntity>(_locationLockService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsLocationLockEntity> GetById(long id)
		{
			return new ResultData<WmsLocationLockEntity>(_locationLockService.GetById(id));
		}

		public ReturnData Save(WmsLocationLockEntity model)
		{
			return _locationLockService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _locationLockService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}
	}
}
