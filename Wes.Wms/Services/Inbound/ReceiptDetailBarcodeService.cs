using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils;

namespace Wes.Service
{
	/// <summary>
	/// 入库单明细条码（一箱一码）数据访问
	/// </summary>
	public class ReceiptDetailBarcodeService : Repository<WmsReceiptDetailBarcodeEntity>, IReceiptDetailBarcodeService
	{
		public ReceiptDetailBarcodeService(ISqlSugarClient db) : base(db) { }

		public List<WmsReceiptDetailBarcodeEntity> GetListByDetailId(long detailId)
		{
			return Context.Queryable<WmsReceiptDetailBarcodeEntity>().Includes(x => x.Location)
				.Where(p => p.DetailId == detailId && p.IsDel == 0)
				.OrderBy(p => p.SeqNo)
				.ToList();
		}

		public WmsReceiptDetailBarcodeEntity GetByBarcode(string barcode)
		{
			if (string.IsNullOrWhiteSpace(barcode)) return null;
			return Context.Queryable<WmsReceiptDetailBarcodeEntity>()
				.Where(p => p.Barcode == barcode && p.IsDel == 0).First();
		}

		public WmsReceiptDetailBarcodeEntity GetById(long barcodeId)
		{
			return Context.Queryable<WmsReceiptDetailBarcodeEntity>()
				.Where(p => p.BarcodeId == barcodeId && p.IsDel == 0).First();
		}

		/// <summary>软删单个条码（拆分/合并时作废旧码）</summary>
		public bool DeleteById(long barcodeId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsReceiptDetailBarcodeEntity>()
				.SetColumns(p => new WmsReceiptDetailBarcodeEntity
				{
					IsDel = 1,
					UpdateBy = account,
					UpdateTime = DateTime.Now
				})
				.Where(p => p.BarcodeId == barcodeId)
				.ExecuteCommand() > 0;
		}

		/// <summary>更新条码承载数量（合并后累加）</summary>
		public bool UpdateQuantity(long barcodeId, decimal quantity, decimal baseQuantity)
		{
			var account = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsReceiptDetailBarcodeEntity>()
				.SetColumns(p => new WmsReceiptDetailBarcodeEntity
				{
					Quantity = quantity,
					BaseQuantity = baseQuantity,
					UpdateBy = account,
					UpdateTime = DateTime.Now
				})
				.Where(p => p.BarcodeId == barcodeId)
				.ExecuteCommand() > 0;
		}

		public bool BatchSave(List<WmsReceiptDetailBarcodeEntity> barcodes)
		{
			if (barcodes == null || barcodes.Count == 0) return true;
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			foreach (var b in barcodes)
			{
				b.BarcodeId = 0;
				b.IsDel = 0;
				b.CreateBy = account;
				b.CreateTime = now;
				Context.Insertable(b).ExecuteReturnSnowflakeId();
			}
			return true;
		}

		/// <summary>条码上架：落库位、置为已上架</summary>
		public bool UpdateShelved(long barcodeId, long locationId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			var now = DateTime.Now;
			return Context.Updateable<WmsReceiptDetailBarcodeEntity>()
				.SetColumns(p => new WmsReceiptDetailBarcodeEntity
				{
					Status = "shelved",
					LocationId = locationId,
					ShelveTime = now,
					UpdateBy = account,
					UpdateTime = now
				})
				.Where(p => p.BarcodeId == barcodeId)
				.ExecuteCommand() > 0;
		}

		/// <summary>整单重建时软删该单全部条码</summary>
		public bool DeleteByReceiptId(long receiptId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsReceiptDetailBarcodeEntity>()
				.SetColumns(p => new WmsReceiptDetailBarcodeEntity
				{
					IsDel = 1,
					UpdateBy = account,
					UpdateTime = DateTime.Now
				})
				.Where(p => p.ReceiptId == receiptId)
				.ExecuteCommand() > 0;
		}

		/// <summary>明细重建时软删其旧条码</summary>
		public bool DeleteByDetailId(long detailId)
		{
			var account = GlobalContext.CurrentUser?.Account;
			return Context.Updateable<WmsReceiptDetailBarcodeEntity>()
				.SetColumns(p => new WmsReceiptDetailBarcodeEntity
				{
					IsDel = 1,
					UpdateBy = account,
					UpdateTime = DateTime.Now
				})
				.Where(p => p.DetailId == detailId)
				.ExecuteCommand() > 0;
		}
	}
}
