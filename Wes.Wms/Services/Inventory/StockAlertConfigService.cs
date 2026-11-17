using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存预警配置 数据访问
	/// </summary>
	public interface IStockAlertConfigService
	{
		List<WmsStockAlertConfigEntity> GetList(ParamData<WmsStockAlertConfigParam> param, out int total);
		WmsStockAlertConfigEntity? GetById(long id);
		bool Save(WmsStockAlertConfigEntity model);
		bool Delete(List<long> ids);
		List<WmsStockAlertConfigEntity> GetEnabledList();
	}

	public class StockAlertConfigService : Repository<WmsStockAlertConfigEntity>, IStockAlertConfigService
	{
		public StockAlertConfigService(ISqlSugarClient db) : base(db) { }

		public List<WmsStockAlertConfigEntity> GetList(ParamData<WmsStockAlertConfigParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsStockAlertConfigEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			if (param.Params != null && param.Params.Enabled.HasValue)
				exp.And(p => p.Enabled == param.Params.Enabled.Value);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.And(p => p.Remark.Contains(kw));
			}
			var query = Context.Queryable<WmsStockAlertConfigEntity>()
				.Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Includes(p => p.Material)
				.Where(exp.ToExpression())
				.OrderBy(p => p.ConfigId, OrderByType.Desc);
			total = 0;
			var list = param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
			FillUserNames(list);
			return list;
		}

		public WmsStockAlertConfigEntity? GetById(long id)
		{
			var item = Context.Queryable<WmsStockAlertConfigEntity>()
				.Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Includes(p => p.Material)
				.Where(p => p.ConfigId == id && p.IsDel == 0).First();
			if (item != null) FillUserNames(new List<WmsStockAlertConfigEntity> { item });
			return item;
		}

		/// <summary>把逗号分隔的处理人 id 拼成姓名，供列表/编辑回显</summary>
		private void FillUserNames(List<WmsStockAlertConfigEntity> list)
		{
			var ids = list.Where(p => !string.IsNullOrWhiteSpace(p.NotifyUserIds))
				.SelectMany(p => p.NotifyUserIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
				.Select(s => long.TryParse(s.Trim(), out var id) ? id : 0)
				.Where(id => id > 0).Distinct().ToList();
			if (ids.Count == 0) return;

			var users = Context.Queryable<Wes.Entity.SysUserEntity>()
				.Where(u => ids.Contains(u.UserId)).ToList();
			var dict = users.ToDictionary(u => u.UserId, u => u.Account ?? u.UserName);
			foreach (var item in list)
			{
				if (string.IsNullOrWhiteSpace(item.NotifyUserIds)) continue;
				item.NotifyUserNames = string.Join("、", item.NotifyUserIds
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(s => long.TryParse(s.Trim(), out var id) ? dict.GetValueOrDefault(id) : null)
					.Where(n => !string.IsNullOrEmpty(n)));
			}
		}

		/// <summary>跑预警用：全部启用的配置（带导航，拼站内信标题要用名称）</summary>
		public List<WmsStockAlertConfigEntity> GetEnabledList()
		{
			return Context.Queryable<WmsStockAlertConfigEntity>()
				.Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Location).Includes(p => p.Material)
				.Where(p => p.IsDel == 0 && p.Enabled == 1).ToList();
		}

		public bool Save(WmsStockAlertConfigEntity model)
		{
			if (model.ConfigId > 0)
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
			return Context.Updateable<WmsStockAlertConfigEntity>()
				.SetColumns(p => new WmsStockAlertConfigEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.ConfigId)).ExecuteCommand() > 0;
		}
	}
}
