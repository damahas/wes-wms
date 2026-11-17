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
	/// 入库单 数据访问
	/// </summary>
	public class ReceiptService : Repository<WmsReceiptEntity>, IReceiptService
	{
		public ReceiptService(ISqlSugarClient db) : base(db) { }

		public List<WmsReceiptEntity> GetList(ParamData<WmsReceiptParam> param, out int total)
		{
			var exp = Expressionable.Create<WmsReceiptEntity>();
			exp.And(p => p.IsDel == 0);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.Keywords))
			{
				var kw = param.Params.Keywords.Trim();
				exp.Or(p => p.ReceiptNo.Contains(kw));
			}
			if (param.Params != null && param.Params.WarehouseId.HasValue)
				exp.And(p => p.WarehouseId == param.Params.WarehouseId.Value);
			if (param.Params != null && !string.IsNullOrWhiteSpace(param.Params.ReceiptStatus))
				exp.And(p => p.ReceiptStatus == param.Params.ReceiptStatus);
			total = 0;
			var query = Context.Queryable<WmsReceiptEntity>().Includes(p => p.Warehouse)
				.Where(exp.ToExpression()).OrderBy(p => p.ReceiptId, OrderByType.Desc);
			return param.PageSize == 0
				? query.ToList()
				: query.ToPageList(param.PageNum, param.PageSize, ref total);
		}

		public WmsReceiptEntity? GetById(long id)
		{
			var entity = Context.Queryable<WmsReceiptEntity>().Includes(p => p.Warehouse)
				.Where(p => p.ReceiptId == id && p.IsDel == 0).First();

			// 明细手动加载：Includes 按外键加载时不会带 IsDel 条件，
			// 会把编辑保存时软删的旧明细一并查出（明细数量翻倍）
			if (entity != null)
				entity.Details = GetDetailsByReceiptId(id);

			return entity;
		}

		/// <summary>
		/// 仅保存主表（插入或更新），不含子表级联。子表逻辑由 ReceiptBiz 统一管理事务。
		/// </summary>
		public bool Save(WmsReceiptEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			if (model.ReceiptId > 0)
			{
				model.UpdateBy = account;
				model.UpdateTime = now;
				return Context.Updateable(model).ExecuteCommand() > 0;
			}
			model.CreateBy = account;
			model.CreateTime = now;
			model.ReceiptId = Context.Insertable(model).ExecuteReturnSnowflakeId();
			return model.ReceiptId > 0;
		}

		public bool Delete(List<long> ids)
		{
			if (ids == null || ids.Count == 0) return false;
			var updateBy = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsReceiptEntity>()
				.SetColumns(p => new WmsReceiptEntity { IsDel = 1, UpdateTime = DateTime.Now, UpdateBy = updateBy })
				.Where(p => ids.Contains(p.ReceiptId)).ExecuteCommand() > 0;
		}

		public bool DeleteDetailsByReceiptId(long receiptId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsReceiptDetailEntity>()
				.SetColumns(p => new WmsReceiptDetailEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
				.Where(p => p.ReceiptId == receiptId && p.IsDel == 0)
				.ExecuteCommand() > 0;
		}

		public bool BatchSaveDetails(long receiptId, List<WmsReceiptDetailEntity> details)
		{
			if (details == null || details.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			foreach (var d in details)
			{
				d.ReceiptId = receiptId;
				d.DetailId = 0;
				d.IsDel = 0;
				d.CreateBy = account;
				d.CreateTime = now;
				Context.Insertable(d).ExecuteReturnSnowflakeId();
			}
			return true;
		}

		/// <summary>
		/// 明细入库数量累加（合并明细时使用）
		/// </summary>
		public bool AddDetailQuantity(long detailId, int qty)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsReceiptDetailEntity>()
				.SetColumns(p => p.ReceiptQuantity == p.ReceiptQuantity + qty)
				.SetColumns(p => new WmsReceiptDetailEntity { UpdateBy = account, UpdateTime = now })
				.Where(p => p.DetailId == detailId)
				.ExecuteCommand() > 0;
		}

		/// <summary>
		/// 主表总数量累加（合并明细时同步）
		/// </summary>
		public bool AddTotalQuantity(long receiptId, int qty)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsReceiptEntity>()
				.SetColumns(p => p.TotalQuantity == (p.TotalQuantity ?? 0) + qty)
				.SetColumns(p => new WmsReceiptEntity { UpdateBy = account, UpdateTime = now })
				.Where(p => p.ReceiptId == receiptId)
				.ExecuteCommand() > 0;
		}

		public bool UpdateStatus(long receiptId, string status)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsReceiptEntity>()
				.SetColumns(p => new WmsReceiptEntity { ReceiptStatus = status, UpdateBy = account, UpdateTime = now })
				.Where(p => p.ReceiptId == receiptId)
				.ExecuteCommand() > 0;
		}

		public List<WmsReceiptEntity> GetByReceivingId(long receivingId)
		{
			return Context.Queryable<WmsReceiptEntity>()
				.Where(p => p.SourceId == receivingId && p.SourceType == "receiving" && p.IsDel == 0)
				.ToList();
		}

		public List<WmsReceiptDetailEntity> GetDetailsByReceiptId(long receiptId)
		{
			var list = Context.Queryable<WmsReceiptDetailEntity>()
				.Includes(x => x.Location)
				.Includes(x => x.Material)   // 物料对象一并带出，前端可用 detail.material.materialName
				.Where(p => p.ReceiptId == receiptId && p.IsDel == 0)
				.ToList();
			FillMaterialInfo(list);
			return list;
		}

		/// <summary>单条明细</summary>
		public WmsReceiptDetailEntity GetDetailById(long detailId)
		{
			var detail = Context.Queryable<WmsReceiptDetailEntity>()
				.Where(p => p.DetailId == detailId && p.IsDel == 0).First();
			FillMaterialInfo(detail);
			return detail;
		}

		/// <summary>
		/// 明细只存物料id，物料编码/名称/规格从物料表补出，供前端展示（非数据库字段）
		/// </summary>
		private void FillMaterialInfo(List<WmsReceiptDetailEntity> list)
		{
			if (list == null || list.Count == 0) return;
			var ids = list.Where(p => p.MaterialId > 0).Select(p => p.MaterialId).Distinct().ToList();
			if (ids.Count == 0) return;

			var materials = Context.Queryable<WmsMaterialEntity>()
				.Where(p => ids.Contains(p.MaterialId) && p.IsDel == 0)
				.Select(p => new { p.MaterialId, p.MaterialCode, p.MaterialName, p.Spec })
				.ToList();

			foreach (var d in list)
			{
				var m = materials.FirstOrDefault(p => p.MaterialId == d.MaterialId);
				if (m == null) continue;
				d.MaterialCode = m.MaterialCode;
				d.MaterialName = m.MaterialName;
				if (string.IsNullOrWhiteSpace(d.Spec)) d.Spec = m.Spec;
			}
		}

		private void FillMaterialInfo(WmsReceiptDetailEntity detail)
		{
			if (detail == null) return;
			FillMaterialInfo(new List<WmsReceiptDetailEntity> { detail });
		}

		/// <summary>按条码取明细（PDA 扫码上架用）</summary>
		public WmsReceiptDetailEntity GetDetailByBarcode(string barcode)
		{
			if (string.IsNullOrWhiteSpace(barcode)) return null;
			return Context.Queryable<WmsReceiptDetailEntity>()
				.Where(p => p.Barcode == barcode && p.IsDel == 0).First();
		}

		/// <summary>
		/// 明细上架：累加已上架数量，并更新上架状态、条码与库位
		/// （库位一并回写，保证明细上记录的就是实际上架库位）
		/// </summary>
		public bool UpdateShelveInfo(long detailId, int addQty, string status, string barcode, long? locationId = null)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsReceiptDetailEntity>()
				.SetColumns(p => p.ShelfQuantity == (p.ShelfQuantity ?? 0) + addQty)
				.SetColumns(p => new WmsReceiptDetailEntity
				{
					DetailStatus = status,
					UpdateBy = account,
					UpdateTime = now
				})
				.SetColumnsIF(!string.IsNullOrWhiteSpace(barcode), p => p.Barcode == barcode)
				.SetColumnsIF(locationId.HasValue && locationId.Value > 0, p => p.LocationId == locationId)
				.Where(p => p.DetailId == detailId)
				.ExecuteCommand() > 0;
		}
	}
}
