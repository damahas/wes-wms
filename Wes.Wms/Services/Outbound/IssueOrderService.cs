using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 领料单 数据访问
	/// </summary>
	public class IssueOrderService : Repository<WmsIssueOrderEntity>, IIssueOrderService
	{
		public IssueOrderService(ISqlSugarClient db) : base(db) { }

		public List<WmsIssueOrderEntity> GetList(ParamData<WmsIssueOrderParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsIssueOrderEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null)
			{
				if (!string.IsNullOrWhiteSpace(param.Params.Keywords))
				{
					var kw = param.Params.Keywords.Trim();
					exp.Or(p => p.IssueNo.Contains(kw));
					exp.Or(p => p.WorkOrderNo.Contains(kw));
					exp.Or(p => p.DeptName.Contains(kw));
					exp.Or(p => p.SourceNo.Contains(kw));
				}
				if (param.Params.WarehouseId.HasValue)
					exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
				if (!string.IsNullOrWhiteSpace(param.Params.IssueStatus))
					exp.And(p => p.IssueStatus == param.Params.IssueStatus);
				if (!string.IsNullOrWhiteSpace(param.Params.WorkOrderNo))
					exp.And(p => p.WorkOrderNo == param.Params.WorkOrderNo);
			}
			total = 0;
			var query = Context.Queryable<WmsIssueOrderEntity>().Includes(p => p.Warehouse)
				.Where(exp.ToExpression()).OrderBy(p => p.IssueId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsIssueOrderEntity? GetById(long id)
		{
			var entity = Context.Queryable<WmsIssueOrderEntity>().Includes(p => p.Warehouse)
				.Where(p => p.IssueId == id && p.IsDel == 0).First();

			// 明细手动加载：Includes 按外键加载不带 IsDel 条件，会把软删的旧明细一起查出
			if (entity != null)
				entity.Details = GetDetailsByIssueId(id);

			return entity;
		}

		/// <summary>加载有效明细（过滤软删行）</summary>
		public List<WmsIssueOrderDetailEntity> GetDetailsByIssueId(long issueId)
		{
			return Context.Queryable<WmsIssueOrderDetailEntity>()
				.Where(p => p.IssueId == issueId && p.IsDel == 0)
				.ToList();
		}

		/// <summary>
		/// 仅保存主表（插入或更新），不含子表级联。子表逻辑由 IssueOrderBiz 统一管理事务。
		/// </summary>
		public bool Save(WmsIssueOrderEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			if (model.IssueId > 0)
			{
				model.UpdateBy = account;
				model.UpdateTime = now;
				return Context.Updateable(model).ExecuteCommand() > 0;
			}
			model.CreateBy = account;
			model.CreateTime = now;
			model.IssueId = Context.Insertable(model).ExecuteReturnSnowflakeId();
			return model.IssueId > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsIssueOrderEntity>()
				.SetColumns(p => new WmsIssueOrderEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.IssueId)).ExecuteCommand() > 0;
		}

		public bool DeleteDetailsByIssueId(long issueId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsIssueOrderDetailEntity>()
				.SetColumns(p => new WmsIssueOrderDetailEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
				.Where(p => p.IssueId == issueId && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		public bool BatchSaveDetails(long issueId, List<WmsIssueOrderDetailEntity> details)
		{
			if (details == null || details.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			foreach (var d in details)
			{
				d.IssueId = issueId;
				d.DetailId = 0;
				d.IsDel = 0;
				d.CreateBy = account;
				d.CreateTime = now;
				d.DetailId = Context.Insertable(d).ExecuteReturnSnowflakeId();
			}
			return true;
		}

		public bool SaveDetail(WmsIssueOrderDetailEntity detail)
		{
			if (detail == null || detail.DetailId <= 0) return false;
			detail.UpdateBy = GlobalContext.CurrentUser?.Account;
			detail.UpdateTime = DateTime.Now;
			return Context.Updateable(detail).ExecuteCommand() > 0;
		}
	}
}
