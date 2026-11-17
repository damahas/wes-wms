using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存调整 数据访问
	/// </summary>
	public class AdjustService : Repository<WmsAdjustEntity>, IAdjustService
	{
		public AdjustService(ISqlSugarClient db) : base(db) { }

		public List<WmsAdjustEntity> GetList(ParamData<WmsAdjustParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsAdjustEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.AdjustNo.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsAdjustEntity>().Includes(p => p.Warehouse).Includes(p => p.Zone).Where(exp.ToExpression()).OrderBy(p => p.AdjustId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsAdjustEntity? GetById(long id)
		{
			var entity = Context.Queryable<WmsAdjustEntity>().Includes(p => p.Warehouse).Includes(p => p.Zone)
				.Where(p => p.AdjustId == id && p.IsDel == 0).First();

			// 明细手动加载：Includes 按外键加载时不会带 IsDel 条件，
			// 会把编辑保存时软删的旧明细一并查出（明细数量翻倍）
			if (entity != null)
				entity.Details = GetDetailsByAdjustId(id);

			return entity;
		}

		/// <summary>加载有效明细（过滤软删行）</summary>
		public List<WmsAdjustDetailEntity> GetDetailsByAdjustId(long adjustId)
		{
			return Context.Queryable<WmsAdjustDetailEntity>()
				.Where(p => p.AdjustId == adjustId && p.IsDel == 0)
				.ToList();
		}

		/// <summary>
		/// 仅保存主表（插入或更新），不含子表级联。子表逻辑由 AdjustBiz 统一管理事务。
		/// </summary>
		public bool Save(WmsAdjustEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			if (model.AdjustId > 0)
			{
				model.UpdateBy = account;
				model.UpdateTime = now;
				return Context.Updateable(model).ExecuteCommand() > 0;
			}
			model.CreateBy = account;
			model.CreateTime = now;
			model.AdjustId = Context.Insertable(model).ExecuteReturnSnowflakeId();
			return model.AdjustId > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsAdjustEntity>()
				.SetColumns(p => new WmsAdjustEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.AdjustId)).ExecuteCommand() > 0;
		}

		public bool DeleteDetailsByAdjustId(long adjustId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsAdjustDetailEntity>()
				.SetColumns(p => new WmsAdjustDetailEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
				.Where(p => p.AdjustId == adjustId && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		public bool BatchSaveDetails(long adjustId, List<WmsAdjustDetailEntity> details)
		{
			if (details == null || details.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			foreach (var d in details)
			{
				d.AdjustId = adjustId;
				d.DetailId = 0;
				d.IsDel = 0;
				d.CreateBy = account;
				d.CreateTime = now;
				d.DetailId = Context.Insertable(d).ExecuteReturnSnowflakeId();
			}
			return true;
		}
	}
}
