namespace Wes.Model
{
	/// <summary>
	/// 库位锁定 查询参数
	/// </summary>
	public class WmsLocationLockParam
	{
		public string? Keywords { get; set; }
		public long? WarehouseId { get; set; }
	}
}
