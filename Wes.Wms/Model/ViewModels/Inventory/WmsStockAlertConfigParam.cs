namespace Wes.Model
{
	/// <summary>
	/// 库存预警配置 查询参数
	/// </summary>
	public class WmsStockAlertConfigParam
	{
		public string? Keywords { get; set; }
		public long? WarehouseId { get; set; }
		public int? Enabled { get; set; }
	}
}
