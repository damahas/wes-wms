namespace Wes.Model
{
	/// <summary>
	/// 调拨单 查询参数
	/// </summary>
	public class WmsTransferParam
	{
		/// <summary>关键字：调拨单号</summary>
		public string Keywords { get; set; }

		/// <summary>调出仓库</summary>
		public long? FromWarehouseId { get; set; }

		/// <summary>调入仓库</summary>
		public long? ToWarehouseId { get; set; }

		/// <summary>状态 pending-待调拨 in_transit-在途 completed-已完成 cancelled-已取消</summary>
		public string TransferStatus { get; set; }
	}
}
