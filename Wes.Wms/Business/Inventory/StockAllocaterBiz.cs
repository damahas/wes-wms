using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils;

namespace Wes.Business
{
	/// <summary>
	/// 库存分配器：把"挑哪一批货出去"这件事收在一处，销售出库与领料发料共用。
	///
	/// 排序规则（用户要求）：过期近的先出，其次先进先出
	///   1. expire_date      升序，NULL 排最后   FEFO
	///   2. production_date  升序，NULL 排最后   过期日相同时先生产的先出
	///   3. create_time      升序                FIFO 兜底
	/// </summary>
	public class StockAllocaterBiz : IStockAllocaterBiz
	{
		private readonly ISqlSugarClient _client;

		public StockAllocaterBiz(ISqlSugarClient sqlSugarClient)
		{
			_client = sqlSugarClient;
		}

		public int Available(WmsStockEntity row)
		{
			if (row == null) return 0;
			var available = (row.StockQuantity ?? 0) - (row.FreezeQuantity ?? 0) - (row.LockQuantity ?? 0);
			// 库存字段是 decimal、出库数量是 int：向下取整，宁可少发也不能超发
			return (int)Math.Floor(available);
		}

		public List<WmsStockEntity> Candidates(long warehouseId, long materialId, long? locationId, string batchNo)
		{
			var query = _client.Queryable<WmsStockEntity>()
				.Where(p => p.IsDel == 0 && p.MaterialId == materialId)
				// 仓库必须过滤：不加的话会跨仓扣减别的仓库的库存
				.WhereIF(warehouseId > 0, p => p.WarehouseId == warehouseId)
				.WhereIF(locationId.HasValue && locationId.Value > 0, p => p.LocationId == locationId.Value)
				.WhereIF(!string.IsNullOrWhiteSpace(batchNo), p => p.BatchNo == batchNo)
				// MySQL 里 `expire_date is null` 返回 0/1，ASC 时非空值(0)在前、空值(1)在后
				.OrderBy("expire_date is null ASC, expire_date ASC, production_date is null ASC, production_date ASC, create_time ASC");

			return query.ToList();
		}

		public List<StockAllocateItem> Allocate(long warehouseId, long materialId, int quantity, long? locationId, string batchNo)
		{
			var result = new List<StockAllocateItem>();
			if (quantity <= 0) return result;

			var remain = quantity;
			foreach (var row in Candidates(warehouseId, materialId, locationId, batchNo))
			{
				if (remain <= 0) break;

				var available = Available(row);
				if (available <= 0) continue;

				var take = available < remain ? available : remain;
				result.Add(new StockAllocateItem { Stock = row, Quantity = take });
				remain -= take;
			}

			return result;
		}

		public int AvailableTotal(long warehouseId, long materialId)
		{
			var rows = _client.Queryable<WmsStockEntity>()
				.Where(p => p.IsDel == 0 && p.MaterialId == materialId)
				.WhereIF(warehouseId > 0, p => p.WarehouseId == warehouseId)
				.ToList();

			var total = 0;
			foreach (var row in rows)
			{
				var available = Available(row);
				if (available > 0) total += available;
			}
			return total;
		}
	}
}
