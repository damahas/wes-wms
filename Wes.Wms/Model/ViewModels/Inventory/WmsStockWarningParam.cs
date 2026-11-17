namespace Wes.Model
{
	/// <summary>
	/// 库存预警 查询参数
	/// </summary>
	public class WmsStockWarningParam
	{
		public string? Keywords { get; set; }
		public long? WarehouseId { get; set; }
		public long? MaterialId { get; set; }
		public string? WarningStatus { get; set; }
	}
}
