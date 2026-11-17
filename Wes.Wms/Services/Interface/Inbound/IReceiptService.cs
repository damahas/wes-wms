using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 入库单 数据访问接口
	/// </summary>
	public interface IReceiptService
	{
		List<WmsReceiptEntity> GetList(ParamData<WmsReceiptParam> param, out int total);
		WmsReceiptEntity? GetById(long id);
		bool Save(WmsReceiptEntity model);
		bool Delete(List<long> ids);

		bool DeleteDetailsByReceiptId(long receiptId);
		bool BatchSaveDetails(long receiptId, List<WmsReceiptDetailEntity> details);

		/// <summary>明细入库数量累加（用于同一入库单合并明细）</summary>
		bool AddDetailQuantity(long detailId, int qty);

		/// <summary>主表总数量累加</summary>
		bool AddTotalQuantity(long receiptId, int qty);
		bool UpdateStatus(long receiptId, string status);
		List<WmsReceiptEntity> GetByReceivingId(long receivingId);
		List<WmsReceiptDetailEntity> GetDetailsByReceiptId(long receiptId);

		/// <summary>单条明细</summary>
		WmsReceiptDetailEntity GetDetailById(long detailId);

		/// <summary>按条码取明细（PDA 扫码上架用）</summary>
		WmsReceiptDetailEntity GetDetailByBarcode(string barcode);

		/// <summary>明细上架：累加已上架数量，并更新上架状态、条码与库位</summary>
		bool UpdateShelveInfo(long detailId, int addQty, string status, string barcode, long? locationId = null);
	}
}
