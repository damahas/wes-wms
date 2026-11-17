namespace Wes.Model
{
	/// <summary>
	/// 仓库 查询参数
	/// </summary>
	public class WmsWarehouseParam
	{
		public string? Keywords { get; set; }
		public string? Status { get; set; }
		public long? WarehouseId { get; set; }
	}
}
