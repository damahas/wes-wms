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
	/// 仓库 业务逻辑
	/// </summary>
	public class WarehouseBiz : IWarehouseBiz
	{
		private readonly IWarehouseService _warehouseService;

		public WarehouseBiz(IWarehouseService warehouseService)
		{
			_warehouseService = warehouseService;
		}

		public RowData<WmsWarehouseEntity> GetList(ParamData<WmsWarehouseParam> param)
		{
			int total = 0;
			var result = new RowData<WmsWarehouseEntity>(_warehouseService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsWarehouseEntity> GetById(long id)
		{
			return new ResultData<WmsWarehouseEntity>(_warehouseService.GetById(id));
		}

		public ReturnData Save(WmsWarehouseEntity model)
		{
			return _warehouseService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _warehouseService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}

		public ReturnData ChangeStatus(WmsWarehouseEntity model)
		{
			if (_warehouseService.GetById(model.WarehouseId) == null)
				return new ReturnData(500, "仓库不存在！");
			return _warehouseService.ChangeStatus(model)
				? new ReturnData()
				: new ReturnData(500, "状态修改失败！");
		}
	}
}
