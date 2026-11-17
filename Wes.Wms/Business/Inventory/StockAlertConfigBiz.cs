using System;
using SqlSugar;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存预警配置 业务逻辑
	/// 预警范围按 仓库 → 库区 → 库位 逐级收窄：只选仓库 = 全仓，选了库区 = 该库区，选了库位 = 该库位。
	/// 物料可空：空 = 范围内所有物料一起考核总量；选了物料 = 只考核该物料。
	/// </summary>
	public class StockAlertConfigBiz : IStockAlertConfigBiz
	{
		private readonly IStockAlertConfigService _configService;
		private readonly ISqlSugarClient _client;

		public StockAlertConfigBiz(IStockAlertConfigService configService, ISqlSugarClient sqlSugarClient)
		{
			_configService = configService;
			_client = sqlSugarClient;
		}

		public RowData<WmsStockAlertConfigEntity> GetList(ParamData<WmsStockAlertConfigParam> param)
		{
			int total = 0;
			var result = new RowData<WmsStockAlertConfigEntity>(_configService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsStockAlertConfigEntity> GetById(long id)
		{
			return new ResultData<WmsStockAlertConfigEntity>(_configService.GetById(id));
		}

		public ReturnData Save(WmsStockAlertConfigEntity model)
		{
			if (model.WarehouseId <= 0)
				return new ReturnData(500, "请选择仓库！");

			// 层级合法性：库位必须挂在自己选的库区下，不能跨层级乱配
			if (model.LocationId.HasValue && model.LocationId.Value > 0 && (!model.ZoneId.HasValue || model.ZoneId.Value <= 0))
				return new ReturnData(500, "选了库位就必须先选库区！");

			// 库区/库位必须真实存在且属于所选仓库，防止配了个不存在的范围导致预警永远不触发
			if (model.ZoneId.HasValue && model.ZoneId.Value > 0)
			{
				var zone = _client.Queryable<WmsZoneEntity>()
					.Where(p => p.ZoneId == model.ZoneId.Value && p.WarehouseId == model.WarehouseId && p.IsDel == 0).First();
				if (zone == null)
					return new ReturnData(500, "所选库区不存在或不属于该仓库，请重新选择！");
			}
			if (model.LocationId.HasValue && model.LocationId.Value > 0)
			{
				var loc = _client.Queryable<WmsLocationEntity>()
					.Where(p => p.LocationId == model.LocationId.Value && p.IsDel == 0).First();
				if (loc == null || loc.ZoneId != model.ZoneId)
					return new ReturnData(500, "所选库位不存在或不属于所选库区，请重新选择！");
			}

			if (model.WarningType != "min_stock" && model.WarningType != "max_stock")
				return new ReturnData(500, "预警类型只能是 低于下限 或 高于上限！");

			if (model.ThresholdValue <= 0)
				return new ReturnData(500, "阈值必须大于 0！");

			if (string.IsNullOrWhiteSpace(model.NotifyUserIds))
				return new ReturnData(500, "请选择处理人！");

			if (string.IsNullOrWhiteSpace(model.WarningLevel))
				model.WarningLevel = "normal";
			if (model.Enabled != 0) model.Enabled = 1;

			return _configService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _configService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}
	}
}
