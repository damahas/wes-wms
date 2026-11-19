using System;
using System.Text.Json;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 仓库平面布局 业务逻辑
	/// </summary>
	public class WarehouseLayoutBiz : IWarehouseLayoutBiz
	{
		private readonly IWarehouseLayoutService _layoutService;
		private readonly IWarehouseService _warehouseService;

		public WarehouseLayoutBiz(IWarehouseLayoutService layoutService, IWarehouseService warehouseService)
		{
			_layoutService = layoutService;
			_warehouseService = warehouseService;
		}

		public ResultData<WmsWarehouseLayoutEntity> GetByWarehouse(long warehouseId)
		{
			var layout = _layoutService.GetByWarehouse(warehouseId);
			if (layout == null)
			{
				layout = new WmsWarehouseLayoutEntity
				{
					WarehouseId = warehouseId,
					Status = "0",
					CanvasJson = string.Empty
				};
			}
			return new ResultData<WmsWarehouseLayoutEntity>(layout);
		}

		public ReturnData Save(WmsWarehouseLayoutEntity model)
		{
			if (model.WarehouseId <= 0)
				return new ReturnData(500, "仓库id不能为空！");
			if (_warehouseService.GetById(model.WarehouseId) == null)
				return new ReturnData(500, "仓库不存在！");
			if (!string.IsNullOrWhiteSpace(model.CanvasJson))
			{
				try
				{
					using var _ = JsonDocument.Parse(model.CanvasJson);
				}
				catch (JsonException)
				{
					return new ReturnData(500, "画布JSON格式有误！");
				}
			}
			if (model.LayoutId > 0)
			{
				var exist = _layoutService.GetById(model.LayoutId);
				if (exist == null || exist.WarehouseId != model.WarehouseId)
					return new ReturnData(500, "布局不存在！");
				model.Version = exist.Version + 1;
				model.Status = exist.Status;
			}
			return _layoutService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Publish(long layoutId)
		{
			var layout = _layoutService.GetById(layoutId);
			if (layout == null)
				return new ReturnData(500, "布局不存在！");
			return _layoutService.Publish(layoutId)
				? new ReturnData()
				: new ReturnData(500, "发布失败！");
		}
	}
}
