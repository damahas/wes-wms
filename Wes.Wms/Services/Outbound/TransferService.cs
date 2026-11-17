using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 调拨单 数据访问
	/// </summary>
	public class TransferService : Repository<WmsTransferEntity>, ITransferService
	{
		public TransferService(ISqlSugarClient db) : base(db) { }

		public List<WmsTransferEntity> GetList(ParamData<WmsTransferParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsTransferEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.TransferNo.Contains(kw));
			}
			if (param.Params != null && param.Params.FromWarehouseId.HasValue)
				exp.And(p => p.FromWarehouseId == param.Params.FromWarehouseId.Value);
			if (param.Params != null && param.Params.ToWarehouseId.HasValue)
				exp.And(p => p.ToWarehouseId == param.Params.ToWarehouseId.Value);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.TransferStatus))
				exp.And(p => p.TransferStatus == param.Params.TransferStatus);

			total = 0;
			var query = Context.Queryable<WmsTransferEntity>()
				.Includes(p => p.FromWarehouse)
				.Includes(p => p.ToWarehouse)
				.Where(exp.ToExpression())
				.OrderBy(p => p.TransferId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsTransferEntity? GetById(long id)
		{
			var entity = Context.Queryable<WmsTransferEntity>()
				.Includes(p => p.FromWarehouse)
				.Includes(p => p.ToWarehouse)
				.Where(p => p.TransferId == id && p.IsDel == 0).First();

			// 明细手动加载：Includes 按外键加载时不会带 IsDel 条件，
			// 会把编辑保存时软删的旧明细一并查出（明细数量翻倍）
			if (entity != null)
				entity.Details = GetDetailsByTransferId(id);

			return entity;
		}

		/// <summary>加载有效明细（过滤软删行）</summary>
		public List<WmsTransferDetailEntity> GetDetailsByTransferId(long transferId)
		{
			return Context.Queryable<WmsTransferDetailEntity>()
				.Where(p => p.TransferId == transferId && p.IsDel == 0)
				.ToList();
		}

		/// <summary>
		/// 仅保存主表（插入或更新），不含子表级联。子表逻辑由 TransferBiz 统一管理事务。
		/// </summary>
		public bool Save(WmsTransferEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			if (model.TransferId > 0)
			{
				model.UpdateBy = account;
				model.UpdateTime = now;
				return Context.Updateable(model).ExecuteCommand() > 0;
			}
			model.CreateBy = account;
			model.CreateTime = now;
			model.TransferId = Context.Insertable(model).ExecuteReturnSnowflakeId();
			return model.TransferId > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsTransferEntity>()
				.SetColumns(p => new WmsTransferEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.TransferId)).ExecuteCommand() > 0;
		}

		public bool DeleteDetailsByTransferId(long transferId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsTransferDetailEntity>()
				.SetColumns(p => new WmsTransferDetailEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
				.Where(p => p.TransferId == transferId && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		public bool BatchSaveDetails(long transferId, List<WmsTransferDetailEntity> details)
		{
			if (details == null || details.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			foreach (var d in details)
			{
				d.TransferId = transferId;
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
