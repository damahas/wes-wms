using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 出库单 数据访问
	/// </summary>
	public class DeliveryService : Repository<WmsDeliveryEntity>, IDeliveryService
	{
		public DeliveryService(ISqlSugarClient db) : base(db) { }

		public List<WmsDeliveryEntity> GetList(ParamData<WmsDeliveryParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsDeliveryEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.DeliveryNo.Contains(kw));
				exp.Or(p => p.CarrierName.Contains(kw));
				exp.Or(p => p.WaybillNo.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			total = 0;
			var query = Context.Queryable<WmsDeliveryEntity>().Includes(p => p.Warehouse).Includes(p => p.Zone).Includes(p => p.Customer).Where(exp.ToExpression()).OrderBy(p => p.DeliveryId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsDeliveryEntity? GetById(long id)
		{
			var entity = Context.Queryable<WmsDeliveryEntity>().Includes(p => p.Warehouse)
				.Includes(p => p.Zone).Includes(p => p.Customer)
				.Where(p => p.DeliveryId == id && p.IsDel == 0).First();

			// 明细手动加载：Includes 按外键加载时不会带 IsDel 条件，
			// 会把编辑保存时软删的旧明细一并查出（明细数量翻倍）
			if (entity != null)
				entity.Details = GetDetailsByDeliveryId(id);

			return entity;
		}

		/// <summary>加载有效明细（过滤软删行）</summary>
		public List<WmsDeliveryDetailEntity> GetDetailsByDeliveryId(long deliveryId)
		{
			return Context.Queryable<WmsDeliveryDetailEntity>()
				.Where(p => p.DeliveryId == deliveryId && p.IsDel == 0)
				.ToList();
		}

		/// <summary>
		/// 仅保存主表（插入或更新），不含子表级联。子表逻辑由 DeliveryBiz 统一管理事务。
		/// </summary>
		public bool Save(WmsDeliveryEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			if (model.DeliveryId > 0)
			{
				model.UpdateBy = account;
				model.UpdateTime = now;
				return Context.Updateable(model).ExecuteCommand() > 0;
			}
			model.CreateBy = account;
			model.CreateTime = now;
			model.DeliveryId = Context.Insertable(model).ExecuteReturnSnowflakeId();
			return model.DeliveryId > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsDeliveryEntity>()
				.SetColumns(p => new WmsDeliveryEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.DeliveryId)).ExecuteCommand() > 0;
		}

		public bool DeleteDetailsByDeliveryId(long deliveryId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsDeliveryDetailEntity>()
				.SetColumns(p => new WmsDeliveryDetailEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
				.Where(p => p.DeliveryId == deliveryId && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		public bool BatchSaveDetails(long deliveryId, List<WmsDeliveryDetailEntity> details)
		{
			if (details == null || details.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			foreach (var d in details)
			{
				d.DeliveryId = deliveryId;
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
