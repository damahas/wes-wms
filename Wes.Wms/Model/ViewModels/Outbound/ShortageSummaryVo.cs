namespace Wes.Model
{
	/// <summary>
	/// 缺料看板汇总
	/// </summary>
	public class ShortageSummaryVo
	{
		/// <summary>涉及领料单数</summary>
		public int IssueCount { get; set; }

		/// <summary>涉及工单数</summary>
		public int WorkOrderCount { get; set; }

		/// <summary>缺料明细行数</summary>
		public int RowCount { get; set; }

		/// <summary>欠料数量合计</summary>
		public int ShortageQuantity { get; set; }

		/// <summary>真实缺口合计（扣掉可用与已预留后仍缺的）</summary>
		public int GapQuantity { get; set; }

		/// <summary>存在真实缺口的行数</summary>
		public int GapRowCount { get; set; }

		/// <summary>已超期的行数（需求日期早于今天）</summary>
		public int OverdueCount { get; set; }
	}
}
