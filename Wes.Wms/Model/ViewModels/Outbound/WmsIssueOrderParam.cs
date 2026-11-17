namespace Wes.Model
{
	/// <summary>
	/// 领料单 查询参数
	/// </summary>
	public class WmsIssueOrderParam
	{
		public string? Keywords { get; set; }
		public long? WarehouseId { get; set; }
		public string? IssueStatus { get; set; }
		public string? WorkOrderNo { get; set; }
	}
}
