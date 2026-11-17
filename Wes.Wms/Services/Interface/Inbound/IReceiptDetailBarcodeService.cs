using System.Collections.Generic;
using Wes.Model;

namespace Wes.Service
{
	/// <summary>
	/// 入库单明细条码（一箱一码）数据访问接口
	/// </summary>
	public interface IReceiptDetailBarcodeService
	{
		List<WmsReceiptDetailBarcodeEntity> GetListByDetailId(long detailId);
		WmsReceiptDetailBarcodeEntity GetById(long barcodeId);
		WmsReceiptDetailBarcodeEntity GetByBarcode(string barcode);
		bool UpdateQuantity(long barcodeId, decimal quantity, decimal baseQuantity);
		bool DeleteById(long barcodeId);
		bool BatchSave(List<WmsReceiptDetailBarcodeEntity> barcodes);
		bool UpdateShelved(long barcodeId, long locationId);
		bool DeleteByReceiptId(long receiptId);
		bool DeleteByDetailId(long detailId);
	}
}
