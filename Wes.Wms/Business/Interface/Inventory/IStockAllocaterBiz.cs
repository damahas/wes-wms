using System.Collections.Generic;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存分配器：出库挑货的唯一入口（销售出库、领料发料共用）
	/// </summary>
	public interface IStockAllocaterBiz
	{
		/// <summary>
		/// 可用量 = 库存量 - 冻结量 - 锁定量（冻结是质量隔离/查封，锁定是已预留给别人）
		/// </summary>
		int Available(WmsStockEntity row);

		/// <summary>
		/// 按出库优先级返回候选库存行。
		/// FEFO（先过期先出）→ FIFO（先进先出）：失效日期升序 → 生产日期升序 → 入库时间升序，
		/// 空日期一律排最后，否则没有填保质期的物料会被永久优先出掉。
		/// </summary>
		List<WmsStockEntity> Candidates(long warehouseId, long materialId, long? locationId, string batchNo);

		/// <summary>
		/// 分配（只算不扣）：按优先级累计到 quantity，返回每行可扣数量。不够时返回的行合计小于 quantity。
		/// </summary>
		List<StockAllocateItem> Allocate(long warehouseId, long materialId, int quantity, long? locationId, string batchNo);

		/// <summary>
		/// 某物料在某仓库的可用量合计
		/// </summary>
		int AvailableTotal(long warehouseId, long materialId);
	}
}
