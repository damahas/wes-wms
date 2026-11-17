using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using Wes.Model;
using Wes.Service;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 缺料看板：把"欠多少"拆成"缺但已解决"和"真缺口"，否则生产看到一串欠料数字只会徒增焦虑。
	///
	/// 口径：
	///   欠料     = 需求数量 - 已发数量
	///   可用     = Σ(库存量 - 冻结量 - 锁定量)，冻结/锁定不能算可用
	///   已预留   = wms_lock 里给同一工单占住的量
	///   待检     = IQC 未完成的收货量（检完合格即可覆盖缺口）
	///   真实缺口 = 欠料 - 可用 - 已预留
	///
	/// 缺料是动态状态（随时有料入库、有单发料），所以**不落表**，实时聚合，永远准。
	/// </summary>
	public class ShortageBiz : IShortageBiz
	{
		private readonly ISqlSugarClient _client;
		private readonly IIssueOrderService _issueService;

		public ShortageBiz(ISqlSugarClient sqlSugarClient, IIssueOrderService issueService)
		{
			_client = sqlSugarClient;
			_issueService = issueService;
		}

		public RowData<WmsShortageVo> GetList(ParamData<WmsShortageParam> param)
		{
			var rows = BuildRows(param.Params);

			var total = rows.Count;
			var pageNum = param.PageNum <= 0 ? 1 : param.PageNum;
			var pageSize = param.PageSize;
			var page = pageSize <= 0
				? rows
				: rows.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();

			return new RowData<WmsShortageVo>(page) { total = total };
		}

		public ResultData<ShortageSummaryVo> Summary(WmsShortageParam param)
		{
			// 汇总只看仓库/工单口径的筛选，不受"只看有缺口"影响，否则数字会自证
			var filter = new WmsShortageParam
			{
				WarehouseId = param?.WarehouseId,
				WorkOrderNo = param?.WorkOrderNo,
			};
			var rows = BuildRows(filter);
			var summary = new ShortageSummaryVo
			{
				IssueCount = rows.Select(x => x.IssueId).Distinct().Count(),
				WorkOrderCount = rows.Select(x => x.WorkOrderNo)
					.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Count(),
				RowCount = rows.Count,
				ShortageQuantity = rows.Sum(x => x.ShortageQuantity),
				GapQuantity = rows.Sum(x => x.GapQuantity),
				GapRowCount = rows.Count(x => x.GapQuantity > 0),
				OverdueCount = rows.Count(x => (x.DaysToRequire ?? 0) < 0),
			};
			return new ResultData<ShortageSummaryVo>(summary);
		}

		/// <summary>构建看板数据</summary>
		private List<WmsShortageVo> BuildRows(WmsShortageParam p)
		{
			var result = new List<WmsShortageVo>();

			// 1. 只关心还没发完的领料单
			var exp = Expressionable.Create<WmsIssueOrderEntity>();
			exp.And(x => x.IsDel == 0 && (x.IssueStatus == "pending" || x.IssueStatus == "partial"));
			if (p != null)
			{
				if (p.WarehouseId.HasValue)
					exp.And(x => x.WarehouseId == p.WarehouseId.Value);
				if (!string.IsNullOrWhiteSpace(p.WorkOrderNo))
					exp.And(x => x.WorkOrderNo == p.WorkOrderNo);
			}
			var issues = _client.Queryable<WmsIssueOrderEntity>().Where(exp.ToExpression()).ToList();
			if (issues.Count == 0) return result;

			var issueIds = issues.Select(x => x.IssueId).ToList();
			var details = _client.Queryable<WmsIssueOrderDetailEntity>()
				.Where(x => x.IsDel == 0 && issueIds.Contains(x.IssueId))
				.ToList();
			if (details.Count == 0) return result;

			var materialIds = details.Select(x => x.MaterialId).Distinct().ToList();

			// 2. 一次性取三个口径，避免逐行查库
			var stocks = _client.Queryable<WmsStockEntity>()
				.Where(x => x.IsDel == 0 && materialIds.Contains(x.MaterialId))
				.ToList();

			var locks = _client.Queryable<WmsLockEntity>()
				.Where(x => x.IsDel == 0 && materialIds.Contains(x.MaterialId)
					&& (x.LockStatus == "locked" || x.LockStatus == "partial"))
				.ToList();

			var iqcs = _client.Queryable<WmsIqcOrderEntity>()
				.Where(x => x.IsDel == 0 && materialIds.Contains(x.MaterialId) && x.InspectStatus != "completed")
				.ToList();

			if (p != null && p.MaterialId.HasValue)
				details = details.Where(x => x.MaterialId == p.MaterialId.Value).ToList();
			if (p != null && !string.IsNullOrWhiteSpace(p.Keywords))
			{
				var kw = p.Keywords.Trim();
				details = details.Where(x =>
					(x.MaterialCode ?? "").Contains(kw) || (x.MaterialName ?? "").Contains(kw)).ToList();
			}

			var today = DateTime.Today;

			foreach (var d in details)
			{
				var require = d.RequireQuantity ?? 0;
				var issued = d.IssuedQuantity ?? 0;
				var shortage = require - issued;
				if (shortage <= 0) continue; // 看板只展示还缺料的行

				var issue = issues.FirstOrDefault(x => x.IssueId == d.IssueId);
				if (issue == null) continue;

				var availableDecimal = stocks.Where(x => x.MaterialId == d.MaterialId)
					.Sum(x => (x.StockQuantity ?? 0) - (x.FreezeQuantity ?? 0) - (x.LockQuantity ?? 0));
				// 库存字段是 decimal、单据数量是 int，向下取整保持口径一致（不夸大可用量）
				var available = (int)Math.Floor(availableDecimal);
				if (available < 0) available = 0;

				// 给同一工单占住的量：料回来了不会被别的单抢走
				var locked = locks
					.Where(x => x.MaterialId == d.MaterialId && x.SourceNo == issue.WorkOrderNo)
					.Sum(x => x.LockQuantity);

				var inspecting = iqcs.Where(x => x.MaterialId == d.MaterialId)
					.Sum(x => x.DeliveryQuantity ?? 0);

				var gap = shortage - available - locked;
				if (gap < 0) gap = 0;

				if (p != null && p.OnlyGap == true && gap <= 0) continue;

				result.Add(new WmsShortageVo
				{
					IssueId = issue.IssueId,
					IssueNo = issue.IssueNo,
					WorkOrderNo = issue.WorkOrderNo,
					DeptName = issue.DeptName,
					WarehouseId = issue.WarehouseId,
					WarehouseName = issue.WarehouseName,
					RequireDate = issue.RequireDate,
					DaysToRequire = issue.RequireDate.HasValue ? (int?)(issue.RequireDate.Value.Date - today).TotalDays : null,
					MaterialId = d.MaterialId,
					MaterialCode = d.MaterialCode,
					MaterialName = d.MaterialName,
					Spec = d.Spec,
					Unit = d.Unit,
					RequireQuantity = require,
					IssuedQuantity = issued,
					ShortageQuantity = shortage,
					AvailableQuantity = available,
					LockedQuantity = locked,
					InspectingQuantity = inspecting,
					GapQuantity = gap,
					IssueStatus = issue.IssueStatus,
				});
			}

			// 逾期的排前面，其次按缺口从大到小
			return result
				.OrderBy(x => x.DaysToRequire ?? 9999)
				.ThenByDescending(x => x.GapQuantity)
				.ToList();
		}
	}
}
