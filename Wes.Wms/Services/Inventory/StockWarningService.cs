using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存预警 数据访问
	/// </summary>
	public class StockWarningService : Repository<WmsStockWarningEntity>, IStockWarningService
	{
		public StockWarningService(ISqlSugarClient db) : base(db) { }

		public List<WmsStockWarningEntity> GetList(ParamData<WmsStockWarningParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsStockWarningEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			if (param.Params != null && param.Params.MaterialId.HasValue)
				exp.And(p => p.MaterialId == param.Params.MaterialId.Value);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.WarningStatus))
				exp.And(p => p.WarningStatus == param.Params.WarningStatus);
			total = 0;
			var query = Context.Queryable<WmsStockWarningEntity>().Includes(p => p.Material).Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Where(exp.ToExpression()).OrderBy(p => p.WarningId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsStockWarningEntity? GetById(long id)
		{
			return Context.Queryable<WmsStockWarningEntity>().Includes(p => p.Material).Includes(p => p.Warehouse).Includes(p => p.Zone).Where(p => p.WarningId == id && p.IsDel == 0).First();
		}

		public bool Save(WmsStockWarningEntity model)
		{
			if (model.WarningId > 0)
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
			return Context.Updateable<WmsStockWarningEntity>()
				.SetColumns(p => new WmsStockWarningEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.WarningId)).ExecuteCommand() > 0;
		}
	}
}
