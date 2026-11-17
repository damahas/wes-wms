using SqlSugar;
using Wes.Model;
using Wes.Utils;
using System.Collections.Generic;

namespace Wes.Service
{
    /// <summary>
    /// 收货单明细 数据访问
    /// </summary>
    public class ReceivingDetailService : Repository<WmsReceivingDetailEntity>, IReceivingDetailService
    {
        public ReceivingDetailService(ISqlSugarClient db) : base(db) { }

        public List<WmsReceivingDetailEntity> GetListByReceivingId(long receivingId)
        {
            return Context.Queryable<WmsReceivingDetailEntity>().Includes(p => p.Receiving).Includes(p => p.Material)
                .Where(p => p.ReceivingId == receivingId && p.IsDel == 0)
                .OrderBy(p => p.DetailId)
                .ToList();
        }

        public bool BatchSave(List<WmsReceivingDetailEntity> details)
        {
            if (details == null || details.Count == 0) return true;
            foreach (var item in details)
            {
                if (item.DetailId > 0)
                {
                    item.UpdateBy = GlobalContext.CurrentUser?.Account;
                    item.UpdateTime = System.DateTime.Now;
                    Context.Updateable(item).ExecuteCommand();
                }
                else
                {
                    item.IsDel = 0;
                    item.CreateBy = GlobalContext.CurrentUser?.Account;
                    item.CreateTime = System.DateTime.Now;
                    Context.Insertable(item).ExecuteReturnSnowflakeId();
                }
            }
            return true;
        }

        public bool DeleteByReceivingId(long receivingId)
        {
            var updateBy = GlobalContext.CurrentUser?.Account;
            return Context.Updateable<WmsReceivingDetailEntity>()
                .SetColumns(p => new WmsReceivingDetailEntity { IsDel = 1, UpdateTime = System.DateTime.Now, UpdateBy = updateBy })
                .Where(p => p.ReceivingId == receivingId && p.IsDel == 0)
                .ExecuteCommand() > 0;
        }

		public bool UpdateInboundQuantities(long detailId, decimal returnQty, decimal stockInQty)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = System.DateTime.Now;
			return Context.Updateable<WmsReceivingDetailEntity>()
				.SetColumns(p => p.ReturnQty == (p.ReturnQty ?? 0) + returnQty)
				.SetColumns(p => p.StockInQuantity == (p.StockInQuantity ?? 0) + stockInQty)
				.SetColumns(p => new WmsReceivingDetailEntity { UpdateBy = account, UpdateTime = now })
				.Where(p => p.DetailId == detailId)
				.ExecuteCommand() > 0;
		}

		public bool UpdatePendingInboundQty(long detailId, decimal qty)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = System.DateTime.Now;
			return Context.Updateable<WmsReceivingDetailEntity>()
				.SetColumns(p => p.PendingInboundQty == (p.PendingInboundQty ?? 0) + qty)
				.SetColumns(p => new WmsReceivingDetailEntity { UpdateBy = account, UpdateTime = now })
				.Where(p => p.DetailId == detailId)
				.ExecuteCommand() > 0;
		}

		public bool UpdateReturnQty(long detailId, decimal qty)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = System.DateTime.Now;
			return Context.Updateable<WmsReceivingDetailEntity>()
				.SetColumns(p => p.ReturnQty == (p.ReturnQty ?? 0) + qty)
				.SetColumns(p => new WmsReceivingDetailEntity { UpdateBy = account, UpdateTime = now })
				.Where(p => p.DetailId == detailId)
				.ExecuteCommand() > 0;
		}

		public bool UpdateStockInQuantity(long detailId, decimal qty)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = System.DateTime.Now;
			return Context.Updateable<WmsReceivingDetailEntity>()
				.SetColumns(p => p.StockInQuantity == (p.StockInQuantity ?? 0) + qty)
				.SetColumns(p => new WmsReceivingDetailEntity { UpdateBy = account, UpdateTime = now })
				.Where(p => p.DetailId == detailId)
				.ExecuteCommand() > 0;
		}

		/// <summary>
		/// 获取收货单所有有效明细（用于判断收货单是否可完成）
		/// </summary>
		public List<WmsReceivingDetailEntity> GetIncompleteDetails(long receivingId)
		{
			return Context.Queryable<WmsReceivingDetailEntity>()
				.Where(p => p.ReceivingId == receivingId && p.IsDel == 0)
				.ToList();
		}
	}
}
