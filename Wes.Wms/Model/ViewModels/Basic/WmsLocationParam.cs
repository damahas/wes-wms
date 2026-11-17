namespace Wes.Model
{
	/// <summary>
	/// 库位 查询参数
	/// </summary>
	public class WmsLocationParam
	{
		public string? Keywords { get; set; }
		public long? WarehouseId { get; set; }
		public long? ZoneId { get; set; }
	}
}
