using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 移库单 数据访问
	/// </summary>
	public class MoveService : Repository<WmsMoveEntity>, IMoveService
	{
		public MoveService(ISqlSugarClient db) : base(db) { }

		public List<WmsMoveEntity> GetList(ParamData<WmsMoveParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsMoveEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.MoveNo.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsMoveEntity>().Includes(p => p.Warehouse).Where(exp.ToExpression()).OrderBy(p => p.MoveId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsMoveEntity? GetById(long id)
		{
			var entity = Context.Queryable<WmsMoveEntity>().Includes(p => p.Warehouse)
				.Where(p => p.MoveId == id && p.IsDel == 0).First();

			// 明细手动加载：Includes 按外键加载时不会带 IsDel 条件，
			// 会把编辑保存时软删的旧明细一并查出（明细数量翻倍）
			if (entity != null)
				entity.Details = GetDetailsByMoveId(id);

			return entity;
		}

		/// <summary>加载有效明细（过滤软删行）</summary>
		public List<WmsMoveDetailEntity> GetDetailsByMoveId(long moveId)
		{
			return Context.Queryable<WmsMoveDetailEntity>()
				.Where(p => p.MoveId == moveId && p.IsDel == 0)
				.ToList();
		}

		/// <summary>
		/// 仅保存主表（插入或更新），不含子表级联。子表逻辑由 MoveBiz 统一管理事务。
		/// </summary>
		public bool Save(WmsMoveEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			if (model.MoveId > 0)
			{
				model.UpdateBy = account;
				model.UpdateTime = now;
				return Context.Updateable(model).ExecuteCommand() > 0;
			}
			model.CreateBy = account;
			model.CreateTime = now;
			model.MoveId = Context.Insertable(model).ExecuteReturnSnowflakeId();
			return model.MoveId > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsMoveEntity>()
				.SetColumns(p => new WmsMoveEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.MoveId)).ExecuteCommand() > 0;
		}

		public bool DeleteDetailsByMoveId(long moveId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsMoveDetailEntity>()
				.SetColumns(p => new WmsMoveDetailEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
				.Where(p => p.MoveId == moveId && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		public bool BatchSaveDetails(long moveId, List<WmsMoveDetailEntity> details)
		{
			if (details == null || details.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			foreach (var d in details)
			{
				d.MoveId = moveId;
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
