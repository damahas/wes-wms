using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存流水 数据访问
	/// </summary>
	public class StockRecordService : Repository<WmsStockRecordEntity>, IStockRecordService
	{
		public StockRecordService(ISqlSugarClient db) : base(db) { }

		public List<WmsStockRecordEntity> GetList(ParamData<WmsStockRecordParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsStockRecordEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.RecordNo.Contains(kw));
				exp.Or(p => p.BatchNo.Contains(kw));
				exp.Or(p => p.SourceNo.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsStockRecordEntity>().Includes(p => p.Material).Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Where(exp.ToExpression()).OrderBy(p => p.RecordId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsStockRecordEntity? GetById(long id)
		{
			return Context.Queryable<WmsStockRecordEntity>().Includes(p => p.Material).Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Where(p => p.RecordId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsStockRecordEntity model)
		{
			if (model.RecordId > 0)
			{
				model.UpdateBy = GlobalContext.CurrentUser?.Account;
				model.UpdateTime = DateTime.Now;
				return Update(model);
			}
			model.CreateBy = GlobalContext.CurrentUser?.Account;
			model.CreateTime = DateTime.Now;
			return InsertReturnSnowflakeId(model) > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsStockRecordEntity>()
				.SetColumns(p => new WmsStockRecordEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.RecordId)).ExecuteCommand() > 0;
		}
	}
}
