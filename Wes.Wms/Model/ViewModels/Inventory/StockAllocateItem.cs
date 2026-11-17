namespace Wes.Model
{
	/// <summary>
	/// 一次库存分配的结果项：某个库存行本次可扣多少
	/// </summary>
	public class StockAllocateItem
	{
		/// <summary>库存行</summary>
		public WmsStockEntity Stock { get; set; }

		/// <summary>本次从该行扣除的数量</summary>
		public int Quantity { get; set; }
	}
}
