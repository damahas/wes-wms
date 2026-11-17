using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 检验单 数据访问
	/// </summary>
	public class IqcOrderService : Repository<WmsIqcOrderEntity>, IIqcOrderService
	{
		public IqcOrderService(ISqlSugarClient db) : base(db) { }

		public List<WmsIqcOrderEntity> GetList(ParamData<WmsIqcOrderParam> param, out int total)
		{
			var q = Context.Queryable<WmsIqcOrderEntity>()
				.Includes(p => p.Supplier)
				.Includes(p => p.Material)
				.Includes(p => p.Receiving)
				.Where(p => p.IsDel == 0);

			if (param != null && param.Params != null)
			{
				if (!string.IsNullOrWhiteSpace(param.Params.Keywords))
					q = q.Where(p => p.InspectionNo.Contains(param.Params.Keywords));
				if (!string.IsNullOrWhiteSpace(param.Params.InspectStatus))
					q = q.Where(p => p.InspectStatus == param.Params.InspectStatus);
				if (param.Params.ReceivingId.HasValue)
					q = q.Where(p => p.ReceivingId == param.Params.ReceivingId.Value);
				if (param.Params.SupplierId.HasValue)
					q = q.Where(p => p.SupplierId == param.Params.SupplierId.Value);
			}

			total = 0;
			return q.OrderByDescending(p => p.CreateTime)
				.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		/// <summary>
		/// 来料不良台账：已完成且存在不良（退货数量 &gt; 0 或 存在 NG 检验项）的检验单
		/// </summary>
		public List<WmsIqcOrderEntity> GetDefectList(ParamData<WmsIqcDefectParam> param, out int total)
		{
			var q = Context.Queryable<WmsIqcOrderEntity>()
				.Includes(p => p.Supplier)
				.Includes(p => p.Material)
				.Includes(p => p.Receiving)
				.Where(p => p.IsDel == 0 && p.InspectStatus == "completed");

			if (param != null && param.Params != null)
			{
				var pm = param.Params;
				if (!string.IsNullOrWhiteSpace(pm.Keywords))
				{
					var kw = pm.Keywords.Trim();
					q = q.Where(p => p.InspectionNo.Contains(kw)
						|| SqlFunc.Subqueryable<WmsMaterialEntity>().Where(m => m.MaterialId == p.MaterialId
							&& (m.MaterialCode.Contains(kw) || m.MaterialName.Contains(kw))).Any()
						|| SqlFunc.Subqueryable<WmsReceivingEntity>().Where(r => r.ReceivingId == p.ReceivingId
							&& r.ReceivingNo.Contains(kw)).Any());
				}
				if (pm.SupplierId.HasValue)
					q = q.Where(p => p.SupplierId == pm.SupplierId.Value);
				if (pm.MaterialId.HasValue)
					q = q.Where(p => p.MaterialId == pm.MaterialId.Value);
				if (!string.IsNullOrWhiteSpace(pm.DisposeType))
					q = q.Where(p => p.DisposeType == pm.DisposeType);
				if (DateTime.TryParse(pm.BeginDate, out var begin))
					q = q.Where(p => p.InspectDate >= begin);
				if (DateTime.TryParse(pm.EndDate, out var end))
					q = q.Where(p => p.InspectDate < end.AddDays(1));
			}

			// 只统计存在不良的检验单（不良数量 &gt; 0、有退货 或 存在 NG 检验项）
			q = q.Where(p => (p.DefectQuantity ?? 0) > 0
				|| (p.ReturnQuantity ?? 0) > 0
				|| SqlFunc.Subqueryable<WmsIqcOrderItemEntity>()
					.Where(it => it.InspectionId == p.InspectionId && it.IsDel == 0 && it.IsPassed == false).Any());

			total = 0;
			return q.OrderByDescending(p => p.InspectDate)
				.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		/// <summary>
		/// 某检验单的不合格(NG)检验项
		/// </summary>
		public List<WmsIqcOrderItemEntity> GetNgItems(long inspectionId)
		{
			return Context.Queryable<WmsIqcOrderItemEntity>()
				.Where(p => p.InspectionId == inspectionId && p.IsDel == 0 && p.IsPassed == false)
				.OrderBy(p => p.SortNo)
				.OrderBy(p => p.ItemId)
				.ToList();
		}

		public WmsIqcOrderEntity? GetById(long id)
		{
			return Context.Queryable<WmsIqcOrderEntity>()
				.Includes(p => p.Supplier)
				.Includes(p => p.Material)
				.Includes(p => p.Receiving)
				.Includes(p => p.ReceivingDetail)
				.Where(p => p.InspectionId == id && p.IsDel == 0)
				.First();
		}

		public bool Save(WmsIqcOrderEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			if (model.InspectionId > 0)
			{
				model.UpdateBy = account;
				model.UpdateTime = now;
				return Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand() > 0;
			}
			else
			{
				model.IsDel = 0;
				model.CreateBy = account;
				model.CreateTime = now;
				model.InspectionId = Context.Insertable(model).ExecuteReturnSnowflakeId();
				return model.InspectionId > 0;
			}
		}

		public bool Delete(List<long> ids)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsIqcOrderEntity>()
				.SetColumns(p => new WmsIqcOrderEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
				.Where(p => ids.Contains(p.InspectionId))
				.ExecuteCommand() > 0;
		}

		/// <summary>
		/// 保存检验项（Upsert：已有 ItemId 则更新，否则插入；不删除未提交的项）
		/// </summary>
		public bool SaveItems(long inspectionId, List<WmsIqcOrderItemEntity> items)
		{
			if (items == null || items.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;

			foreach (var item in items)
			{
				item.InspectionId = inspectionId;
				if (item.ItemId > 0)
				{
					item.UpdateBy = account;
					item.UpdateTime = now;
					Context.Updateable(item).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();
				}
				else
				{
					item.ItemId = 0;
					item.IsDel = 0;
					item.CreateBy = account;
					item.CreateTime = now;
					Context.Insertable(item).ExecuteReturnSnowflakeId();
				}
			}
			return true;
		}

		public void BatchInsertItems(List<WmsIqcOrderItemEntity> items)
		{
			if (items == null || items.Count == 0) return;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			foreach (var item in items)
			{
				item.IsDel = 0;
				item.CreateBy = account;
				item.CreateTime = now;
				Context.Insertable(item).ExecuteReturnSnowflakeId();
			}
		}

		public bool DeleteItems(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsIqcOrderItemEntity>()
				.SetColumns(p => new WmsIqcOrderItemEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
				.Where(p => ids.Contains(p.InspectionId) && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		public List<WmsIqcOrderItemEntity> GetItems(long inspectionId)
		{
			return Context.Queryable<WmsIqcOrderItemEntity>()
				.Where(p => p.InspectionId == inspectionId && p.IsDel == 0)
				.OrderBy(p => p.SortNo)
				.OrderBy(p => p.ItemId)
				.ToList();
		}

		public List<WmsIqcOrderEntity> GetUncompletedByReceivingId(long receivingId)
		{
			return Context.Queryable<WmsIqcOrderEntity>()
				.Where(p => p.ReceivingId == receivingId && p.IsDel == 0 && p.InspectStatus != "completed")
				.ToList();
		}
	}
}
