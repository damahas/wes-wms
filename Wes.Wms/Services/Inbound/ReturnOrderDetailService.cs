using SqlSugar;
using Wes.Model;
using Wes.Utils;

namespace Wes.Service
{
	/// <summary>
	/// 退货单明细 数据访问
	/// </summary>
	public class ReturnOrderDetailService : Repository<WmsReturnOrderDetailEntity>, IReturnOrderDetailService
	{
		public ReturnOrderDetailService(ISqlSugarClient db) : base(db) { }

		public List<WmsReturnOrderDetailEntity> GetListByReturnOrderId(long returnOrderId)
		{
			return Context.Queryable<WmsReturnOrderDetailEntity>()
				.Includes(p => p.Material).Includes(p => p.Location)
				.Where(p => p.ReturnOrderId == returnOrderId && p.IsDel == 0)
				.ToList();
		}

		public bool DeleteByReturnOrderId(long returnOrderId)
		{
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsReturnOrderDetailEntity>()
				.SetColumns(p => new WmsReturnOrderDetailEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => p.ReturnOrderId == returnOrderId && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		public void BatchSave(List<WmsReturnOrderDetailEntity> details)
		{
			if (details == null || details.Count == 0) return;
			foreach (var item in details)
			{
				item.IsDel = 0;
				item.CreateBy = GlobalContext.CurrentUser?.Account;
				item.CreateTime = DateTime.Now;
			}
			Context.Insertable(details).ExecuteReturnSnowflakeId();
		}
	}
}
