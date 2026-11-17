using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 入库单 业务逻辑接口
	/// </summary>
	public interface IReceiptBiz
	{
		RowData<WmsReceiptEntity> GetList(ParamData<WmsReceiptParam> param);
		ResultData<WmsReceiptEntity> GetById(long id);
		ReturnData Save(WmsReceiptEntity model);
		ReturnData Delete(string ids);

		/// <summary>从收货单创建入库单（跨模块：Receiving → Receipt）</summary>
		WmsReceiptEntity CreateFromReceiving(string receiptNo, long receivingId, long? supplierId, List<WmsReceivingDetailEntity> exemptDetails);

		/// <summary>从检验单创建入库单（跨模块：Inspection → Receipt）；同一收货单有待入库单时合并明细</summary>
		WmsReceiptEntity CreateFromInspection(long receivingId, long? supplierId, List<WmsReceiptDetailEntity> receiptDetails);

		/// <summary>确认入库（兼容保留）：全部明细已上架时补置 in_stock，否则提示先上架</summary>
		ReturnData ConfirmInbound(long receiptId);

		/// <summary>明细上架：按条码逐个写库存 + 累加已上架数量 + 状态流转 + 回写收货单</summary>
		ReturnData Shelve(long receiptId, long detailId, long locationId, int quantity, string onlyBarcode = null);

		/// <summary>按条码上架（PDA：扫条码 → 扫库位 → 上架，一次一箱）</summary>
		ReturnData ShelveByBarcode(string barcode, long locationId, int quantity);

		/// <summary>按条码查询明细条码（PDA 扫码回显用）</summary>
		WmsReceiptDetailBarcodeEntity GetDetailByBarcode(string barcode);

		/// <summary>明细条码列表（每码承载数量可不等）</summary>
		List<WmsReceiptDetailBarcodeEntity> GetBarcodes(long detailId);

		/// <summary>拆分条码：一个待上架的码拆成多个，拆分数量之和 = 原数量</summary>
		ReturnData SplitBarcode(long detailId, long barcodeId, List<decimal> quantities);

		/// <summary>合并条码：同一明细多个待上架码合并为一个，数量相加</summary>
		ReturnData MergeBarcodes(long detailId, List<long> barcodeIds);
	}
}
