namespace Wes.Model
{
	/// <summary>
	/// 入库单 查询参数
	/// </summary>
	public class WmsReceiptParam
	{
		public string? Keywords { get; set; }
		public long? WarehouseId { get; set; }

		/// <summary>入库状态 pending_in-待入库 shelfing-上架中 in_stock-已入库</summary>
		public string? ReceiptStatus { get; set; }
	}
}
