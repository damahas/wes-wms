using System;

namespace Wes.Model
{
	/// <summary>
	/// 缺料看板行：一张领料单明细一行，汇总需求/已发/欠料与可用量口径
	/// </summary>
	public class WmsShortageVo
	{
		/// <summary>领料单id</summary>
		public long IssueId { get; set; }

		/// <summary>领料单号</summary>
		public string IssueNo { get; set; }

		/// <summary>生产工单号</summary>
		public string WorkOrderNo { get; set; }

		/// <summary>领用部门 / 车间</summary>
		public string DeptName { get; set; }

		/// <summary>仓库id</summary>
		public long? WarehouseId { get; set; }

		/// <summary>仓库名称</summary>
		public string WarehouseName { get; set; }

		/// <summary>需求日期</summary>
		public DateTime? RequireDate { get; set; }

		/// <summary>物料id</summary>
		public long MaterialId { get; set; }

		/// <summary>物料编码</summary>
		public string MaterialCode { get; set; }

		/// <summary>物料名称</summary>
		public string MaterialName { get; set; }

		/// <summary>规格</summary>
		public string Spec { get; set; }

		/// <summary>单位</summary>
		public string Unit { get; set; }

		/// <summary>需求数量</summary>
		public int RequireQuantity { get; set; }

		/// <summary>已发数量</summary>
		public int IssuedQuantity { get; set; }

		/// <summary>欠料数量 = 需求 - 已发</summary>
		public int ShortageQuantity { get; set; }

		/// <summary>该物料全仓可用量（库存 - 冻结 - 锁定）</summary>
		public int AvailableQuantity { get; set; }

		/// <summary>已为该工单预留的数量（wms_lock 未释放部分）</summary>
		public int LockedQuantity { get; set; }

		/// <summary>待检数量（IQC 未完成的入库）</summary>
		public int InspectingQuantity { get; set; }

		/// <summary>真实缺口 = 欠料 - 可用 - 已预留（不含待检，待检合格后可覆盖）</summary>
		public int GapQuantity { get; set; }

		/// <summary>领料单状态</summary>
		public string IssueStatus { get; set; }

		/// <summary>距需求日期天数：负数代表已超期</summary>
		public int? DaysToRequire { get; set; }
	}
}
