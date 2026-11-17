namespace Wes.Model
{
	/// <summary>
	/// 缺料看板 查询参数
	/// </summary>
	public class WmsShortageParam
	{
		public string? Keywords { get; set; }
		public long? WarehouseId { get; set; }
		public string? WorkOrderNo { get; set; }
		public long? MaterialId { get; set; }
		/// <summary>true 时只返回仍存在真实缺口的行（欠料 - 可用 - 已预留 > 0）</summary>
		public bool? OnlyGap { get; set; }
	}
}
