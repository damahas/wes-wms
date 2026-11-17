using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 实时库存 数据访问
	/// </summary>
	public class StockService : Repository<WmsStockEntity>, IStockService
	{
		public StockService(ISqlSugarClient db) : base(db) { }

		public List<WmsStockEntity> GetList(ParamData<WmsStockParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsStockEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null)
			{
				if (!string.IsNullOrWhiteSpace(param.Params.Keywords))
				{
					var kw = param.Params.Keywords.Trim();
					exp.Or(p => p.Barcode.Contains(kw));
					exp.Or(p => p.BatchNo.Contains(kw));
					exp.Or(p => p.MaterialCode.Contains(kw));
					exp.Or(p => p.MaterialName.Contains(kw));
					exp.Or(p => p.LocationCode.Contains(kw));
				}
				if (param.Params.WarehouseId.HasValue)
					exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
				if (param.Params.ZoneId.HasValue)
					exp.And(p => p.ZoneId == param.Params.ZoneId);
				if (param.Params.LocationId.HasValue)
					exp.And(p => p.LocationId == param.Params.LocationId);
				if (param.Params.MaterialId.HasValue)
					exp.And(p => p.MaterialId == param.Params.MaterialId);
				if (!string.IsNullOrWhiteSpace(param.Params.BatchNo))
					exp.And(p => p.BatchNo == param.Params.BatchNo.Trim());
				if (param.Params.HasQuantity == true)
					exp.And(p => (p.StockQuantity ?? 0) > 0);
				// 可用 = 库存 - 冻结 - 锁定，移库/调拨/挑货都按这个口径，不能拿账面数量唬人
				if (param.Params.AvailableOnly == true)
					exp.And(p => (p.StockQuantity ?? 0) - (p.FreezeQuantity ?? 0) - (p.LockQuantity ?? 0) > 0);
			}
			total = 0;
			var query = Context.Queryable<WmsStockEntity>().Includes(p => p.Material).Includes(p => p.Supplier)
				.Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location)
				.Where(exp.ToExpression()).OrderBy(p => p.StockId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsStockEntity? GetById(long id)
		{
			return Context.Queryable<WmsStockEntity>().Includes(p => p.Material).Includes(p => p.Supplier)
				.Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location)
				.Where(p => p.StockId == id && p.IsDel == 0).First();
		}

		/// <summary>按条码取库存（PDA 扫码用）</summary>
		public WmsStockEntity GetByBarcode(string barcode)
		{
			if (string.IsNullOrWhiteSpace(barcode)) return null;
			return Context.Queryable<WmsStockEntity>().Includes(p => p.Material).Includes(p => p.Location)
				.Where(p => p.Barcode == barcode && p.IsDel == 0).First();
		}

		public bool Save(WmsStockEntity model)
		{
			if (model.StockId > 0)
			{
				model.UpdateBy = GlobalContext.CurrentUser?.Account;
				model.UpdateTime = DateTime.Now;
				return Update(model);
			}
			model.CreateBy = GlobalContext.CurrentUser?.Account;
			model.CreateTime = DateTime.Now;
			return InsertReturnSnowflakeId(model) > 0;
		}

		/// <summary>
		/// 上架写入库存：同一条码（无条码时按 物料+批号+库位+HU）已存在则累加数量，否则新增
		/// </summary>
		public bool AddOrUpdateStock(WmsStockEntity model)
		{
			if (model == null) return false;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			var qty = model.StockQuantity ?? 0;
			var hasBarcode = !string.IsNullOrWhiteSpace(model.Barcode);

			var exist = Context.Queryable<WmsStockEntity>().Where(p => p.IsDel == 0)
				.WhereIF(hasBarcode, p => p.Barcode == model.Barcode)
				.WhereIF(!hasBarcode, p => p.MaterialId == model.MaterialId && p.LocationId == model.LocationId
					&& p.BatchNo == model.BatchNo && p.HuId == model.HuId)
				.First();

			if (exist != null)
			{
				return Context.Updateable<WmsStockEntity>()
					.SetColumns(p => p.StockQuantity == (p.StockQuantity ?? 0) + qty)
					.SetColumns(p => p.AvailableQuantity == (p.AvailableQuantity ?? 0) + qty)
					.SetColumns(p => new WmsStockEntity { UpdateBy = account, UpdateTime = now })
					.Where(p => p.StockId == exist.StockId)
					.ExecuteCommand() > 0;
			}

			model.StockId = 0;
			model.AvailableQuantity = qty;
			model.FreezeQuantity = model.FreezeQuantity ?? 0;
			model.LockQuantity = model.LockQuantity ?? 0;
			model.IsDel = 0;
			model.CreateBy = account;
			model.CreateTime = now;
			return InsertReturnSnowflakeId(model) > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsStockEntity>()
				.SetColumns(p => new WmsStockEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.StockId)).ExecuteCommand() > 0;
		}
	}
}
