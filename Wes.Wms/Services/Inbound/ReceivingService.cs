using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
    /// <summary>
    /// 收货单 数据访问
    /// </summary>
    public class ReceivingService : Repository<WmsReceivingEntity>, IReceivingService
    {
        public ReceivingService(ISqlSugarClient db) : base(db) { }

        public List<WmsReceivingEntity> GetList(ParamData<WmsReceivingDetailParam> param, out int total)
        {
            var exp = Expressionable.Create<WmsReceivingEntity>();
            exp.And(p => p.IsDel == 0);
            if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
            {
                var kw = param.Params.Keywords.Trim();
                exp.Or(p => p.ReceivingNo.Contains(kw));
                exp.Or(p => p.CarrierName.Contains(kw));
            }
            if (param.Params != null && param.Params.SupplierId.HasValue)
                exp.And(p => p.SupplierId == param.Params.SupplierId.Value);
            if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.ReceivingStatus))
                exp.And(p => p.ReceivingStatus == param.Params.ReceivingStatus);
            total = 0;
            var query = Context.Queryable<WmsReceivingEntity>().Includes(p => p.Supplier).Where(exp.ToExpression()).OrderBy(p => p.ReceivingId, OrderByType.Desc);
            return param.PageSize == 0
                ? query.ToList()
                : query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public WmsReceivingEntity? GetById(long id)
        {
            return Context.Queryable<WmsReceivingEntity>().Includes(p => p.Supplier).Where(p => p.ReceivingId == id && p.IsDel == 0).First();
        }

        public bool Save(WmsReceivingEntity model)
        {
            if (model.ReceivingId > 0)
            {
                model.UpdateBy = GlobalContext.CurrentUser?.Account;
                model.UpdateTime = DateTime.Now;
                return Context.Updateable(model).ExecuteCommand() > 0;
            }
            model.IsDel = 0;
            model.CreateBy = GlobalContext.CurrentUser?.Account;
            model.CreateTime = DateTime.Now;
            return Context.Insertable(model).ExecuteReturnSnowflakeId() > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            var updateBy = GlobalContext.CurrentUser?.Account;
            return Context.Updateable<WmsReceivingEntity>()
                .SetColumns(p => new WmsReceivingEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
                .Where(p => ids.Contains(p.ReceivingId)).ExecuteCommand() > 0;
        }

		public bool CompleteReceipt(long receivingId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsReceivingEntity>()
				.SetColumns(p => new WmsReceivingEntity { ReceivingStatus = "completed", UpdateBy = account, UpdateTime = now })
				.Where(p => p.ReceivingId == receivingId)
				.ExecuteCommand() > 0;
		}

		public bool UpdateStatus(long receivingId, string status)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsReceivingEntity>()
				.SetColumns(p => new WmsReceivingEntity { ReceivingStatus = status, UpdateBy = account, UpdateTime = now })
				.Where(p => p.ReceivingId == receivingId)
				.ExecuteCommand() > 0;
		}

        public List<WmsReceivingEntity> GetList(List<long> ids)
        {
            return Context.Queryable<WmsReceivingEntity>()
                .Where(p => ids.Contains(p.ReceivingId) && p.IsDel == 0)
                .ToList();
        }
    }
}
