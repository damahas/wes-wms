namespace Wes.Model
{
	/// <summary>
	/// 库存锁定 查询参数
	/// </summary>
	public class WmsLockParam
	{
		public string? Keywords { get; set; }
		public long? WarehouseId { get; set; }
	}
}
