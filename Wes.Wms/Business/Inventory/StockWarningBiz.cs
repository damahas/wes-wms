using System;
using SqlSugar;
using Wes.Entity;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存预警 业务逻辑
	/// 预警记录只由预警引擎产生（定时任务 StockWarningJob / 手动触发），页面只负责查看和处理。
	/// 引擎逻辑：遍历启用的配置 → 按配置范围（仓/区/位，物料可指定）聚合当前库存 → 对比阈值 →
	/// 已有未处理记录就刷新数量（不重复打扰），没有才新建，并给配置的处理人发站内信。
	/// </summary>
	public class StockWarningBiz : IStockWarningBiz
	{
		private readonly IStockWarningService _stockWarningService;
		private readonly IStockAlertConfigService _configService;
		private readonly ISqlSugarClient _client;

		public StockWarningBiz(IStockWarningService stockWarningService,
			IStockAlertConfigService configService, ISqlSugarClient sqlSugarClient)
		{
			_stockWarningService = stockWarningService;
			_configService = configService;
			_client = sqlSugarClient;
		}

		public RowData<WmsStockWarningEntity> GetList(ParamData<WmsStockWarningParam> param)
		{
			int total = 0;
			var result = new RowData<WmsStockWarningEntity>(_stockWarningService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsStockWarningEntity> GetById(long id)
		{
			return new ResultData<WmsStockWarningEntity>(_stockWarningService.GetById(id));
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _stockWarningService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}

		/// <summary>
		/// 跑一遍预警引擎。
		/// 一条 pending 预警 = 一个预警周期（同配置+同物料唯一）：
		/// 命中 → 周期内重复触发只累计次数并每天留快照、发站内信；库存恢复 → 自动置为 resolved 关闭周期。
		/// </summary>
		public ReturnData Run()
		{
			var configs = _configService.GetEnabledList();
			if (configs.Count == 0)
				return new ReturnData(200, "没有启用的预警配置！");

			var now = DateTime.Now;
			int created = 0, resolved = 0, triggered = 0, sent = 0;
			var logs = new List<WmsStockWarningLogEntity>();
			var messages = new List<SysMessageEntity>();

			foreach (var cfg in configs)
			{
				// 该配置范围内、按物料聚合的当前库存量
				var grouped = _client.Queryable<WmsStockEntity>()
					.Where(p => p.IsDel == 0 && p.WarehouseId == cfg.WarehouseId)
					.WhereIF(cfg.ZoneId.HasValue && cfg.ZoneId.Value > 0, p => p.ZoneId == cfg.ZoneId)
					.WhereIF(cfg.LocationId.HasValue && cfg.LocationId.Value > 0, p => p.LocationId == cfg.LocationId)
					.WhereIF(cfg.MaterialId.HasValue && cfg.MaterialId.Value > 0, p => p.MaterialId == cfg.MaterialId)
					.GroupBy(p => p.MaterialId)
					.Select(p => new { MaterialId = p.MaterialId, Qty = SqlFunc.AggregateSum(p.StockQuantity ?? 0) })
					.ToList();

				// 范围内一个物料都没有了，也可能让挂着的 pending 恢复——把配置涉及的 pending 一并查出来对比
				var pendingIds = grouped.Select(g => g.MaterialId).ToList();
				var pendings = _client.Queryable<WmsStockWarningEntity>()
					.Where(p => p.IsDel == 0 && p.ConfigId == cfg.ConfigId && p.WarningStatus == "pending")
					.WhereIF(pendingIds.Count > 0, p => pendingIds.Contains(p.MaterialId))
					.ToList();

				// 1) 库存回到阈值内 → 自动关闭周期（不发站内信，安静恢复）
				foreach (var pending in pendings)
				{
					var g = grouped.FirstOrDefault(x => x.MaterialId == pending.MaterialId);
					decimal qty = g?.Qty ?? 0;
					bool hit = cfg.WarningType == "min_stock" ? qty < cfg.ThresholdValue : qty > cfg.ThresholdValue;
					if (hit) continue;

					_client.Updateable<WmsStockWarningEntity>()
						.SetColumns(p => new WmsStockWarningEntity
						{
							WarningStatus = "resolved",
							ResolvedTime = now,
							ResolvedQuantity = qty,
							CurrentQuantity = (int)qty,
							DiffValue = qty - cfg.ThresholdValue,
							UpdateTime = now,
						})
						.Where(p => p.WarningId == pending.WarningId)
						.ExecuteCommand();
					resolved++;
				}

				// 2) 命中阈值的物料：周期内累计，每次触发都留快照、发站内信
				foreach (var g in grouped)
				{
					decimal qty = g.Qty;
					bool hit = cfg.WarningType == "min_stock" ? qty < cfg.ThresholdValue : qty > cfg.ThresholdValue;
					if (!hit) continue;

					var exist = pendings.FirstOrDefault(p => p.MaterialId == g.MaterialId);
					long warningId;
					if (exist != null)
					{
						warningId = exist.WarningId;
						_client.Updateable<WmsStockWarningEntity>()
							.SetColumns(p => new WmsStockWarningEntity
							{
								CurrentQuantity = (int)qty,
								DiffValue = qty - cfg.ThresholdValue,
								TriggerCount = (exist.TriggerCount ?? 1) + 1,
								LastTriggerTime = now,
								UpdateTime = now,
							})
							.Where(p => p.WarningId == exist.WarningId)
							.ExecuteCommand();
					}
					else
					{
						var warning = new WmsStockWarningEntity
						{
							WarningId = 0,
							WarningType = cfg.WarningType,
							MaterialId = g.MaterialId,
							WarehouseId = cfg.WarehouseId,
							ZoneId = cfg.ZoneId,
							LocationId = cfg.LocationId,
							ConfigId = cfg.ConfigId,
							CurrentQuantity = (int)qty,
							ThresholdValue = cfg.ThresholdValue,
							DiffValue = qty - cfg.ThresholdValue,
							WarningLevel = string.IsNullOrWhiteSpace(cfg.WarningLevel) ? "normal" : cfg.WarningLevel,
							WarningStatus = "pending",
							NotifyUserIds = cfg.NotifyUserIds,
							TriggerCount = 1,
							LastTriggerTime = now,
							CreateTime = now,
						};
						warningId = _client.Insertable(warning).ExecuteReturnSnowflakeId();
						created++;
					}
					triggered++;

					// 每次触发都给处理人发站内信：open_type=in 点击直接跳预警页面
					var scope = DescribeScope(cfg);
					var title = $"{(cfg.WarningType == "min_stock" ? "低库存" : "高库存")}预警：{scope}";
					var recipients = 0;
					foreach (var userIdStr in (cfg.NotifyUserIds ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
					{
						if (!long.TryParse(userIdStr.Trim(), out var uid) || uid <= 0) continue;
						messages.Add(new SysMessageEntity
						{
							MessageType = "warning",
							OpenType = "in",
							MessageTitle = title,
							MessageBody = "/stock/warning",
							UserId = uid,
							SendUserId = 0,
							IsRead = 0,
							CreateTime = now,
						});
						recipients++;
						sent++;
					}

					// 触发快照：过去每天缺多少、库存多少，都在这里
					logs.Add(new WmsStockWarningLogEntity
					{
						LogId = 0,
						WarningId = warningId,
						ConfigId = cfg.ConfigId,
						MaterialId = g.MaterialId,
						CurrentQuantity = qty,
						ThresholdValue = cfg.ThresholdValue,
						DiffValue = qty - cfg.ThresholdValue,
						MessageSent = recipients > 0 ? 1 : 0,
						CreateTime = now,
					});
				}
			}

			if (logs.Count > 0)
				_client.Insertable(logs).ExecuteReturnSnowflakeId();
			if (messages.Count > 0)
				_client.Insertable(messages).ExecuteReturnSnowflakeId();

			return new ReturnData(200, $"预警执行完成：新周期 {created} 个，触发 {triggered} 次，自动恢复 {resolved} 个，发送站内信 {sent} 条");
		}

		/// <summary>
		/// 预警周期内的触发历史（每天一条快照），详情弹窗用
		/// </summary>
		public ResultData<List<WmsStockWarningLogEntity>> GetLogs(long warningId)
		{
			var logs = _client.Queryable<WmsStockWarningLogEntity>()
				.Where(p => p.WarningId == warningId)
				.OrderBy(p => p.CreateTime, OrderByType.Desc)
				.ToList();
			return new ResultData<List<WmsStockWarningLogEntity>>(logs);
		}

		/// <summary>处理 / 忽略预警</summary>
		public ReturnData Handle(long warningId, string status, string remark)
		{
			if (status != "handled" && status != "ignored")
				return new ReturnData(500, "处理状态只能是 handled 或 ignored！");

			var exist = _stockWarningService.GetById(warningId);
			if (exist == null) return new ReturnData(500, "预警记录不存在！");
			if (exist.WarningStatus != "pending")
				return new ReturnData(500, "该预警已处理，不能重复处理！");

			var account = GlobalContext.CurrentUser?.Account;
			var userId = GlobalContext.CurrentUser?.UserId;
			_client.Updateable<WmsStockWarningEntity>()
				.SetColumns(p => new WmsStockWarningEntity
				{
					WarningStatus = status,
					HandleUserId = userId,
					HandleTime = DateTime.Now,
					HandleRemark = remark,
					UpdateBy = account,
					UpdateTime = DateTime.Now,
				})
				.Where(p => p.WarningId == warningId)
				.ExecuteCommand();
			return new ReturnData();
		}

		/// <summary>配置范围的中文描述（站内信标题用）</summary>
		private string DescribeScope(WmsStockAlertConfigEntity cfg)
		{
			var wh = cfg.Warehouse?.WarehouseName ?? $"仓库{cfg.WarehouseId}";
			if (cfg.LocationId.HasValue && cfg.LocationId.Value > 0)
				wh += $" / {cfg.Location?.LocationCode ?? $"库位{cfg.LocationId}"}";
			else if (cfg.ZoneId.HasValue && cfg.ZoneId.Value > 0)
				wh += $" / {cfg.Zone?.ZoneName ?? $"库区{cfg.ZoneId}"}";
			if (cfg.MaterialId.HasValue && cfg.MaterialId.Value > 0)
				wh += $" / {cfg.Material?.MaterialName ?? $"物料{cfg.MaterialId}"}";
			return wh;
		}
	}
}
