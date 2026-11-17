namespace Wes.Model
{
	/// <summary>
	/// 实时库存 查询参数
	/// </summary>
	public class WmsStockParam
	{
		/// <summary>关键字：条码/批次号/物料编码/物料名称/库位编码</summary>
		public string? Keywords { get; set; }

		/// <summary>仓库</summary>
		public long? WarehouseId { get; set; }

		/// <summary>库区</summary>
		public long? ZoneId { get; set; }

		/// <summary>库位</summary>
		public long? LocationId { get; set; }

		/// <summary>物料</summary>
		public long? MaterialId { get; set; }

		/// <summary>批次号</summary>
		public string? BatchNo { get; set; }

		/// <summary>只看有库存的（库存数量 &gt; 0）</summary>
		public bool? HasQuantity { get; set; }

		/// <summary>
		/// 只看可用的（库存 - 冻结 - 锁定 后仍 &gt; 0）
		/// 移库、调拨、出库挑货都要用这个口径：冻着的在等检验，锁着的已被工单占住，都不能搬。
		/// </summary>
		public bool? AvailableOnly { get; set; }
	}
}
