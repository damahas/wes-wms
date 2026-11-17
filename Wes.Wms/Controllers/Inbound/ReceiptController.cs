using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 入库单
	/// </summary>
	[ApiController]
	[Route("wms/receipt")]
	[ApiExplorerSettings(GroupName = "Inbound")]
	public class ReceiptController : ControllerBase
	{
		private readonly IReceiptBiz _receiptBiz;

		public ReceiptController(IReceiptBiz receiptBiz)
		{
			_receiptBiz = receiptBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsReceiptParam> param)
		{
			return _receiptBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _receiptBiz.GetById(id);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsReceiptEntity model)
		{
			model.ReceiptId = 0;
			return _receiptBiz.Save(model);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsReceiptEntity model)
		{
			return _receiptBiz.Save(model);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _receiptBiz.Delete(ids);
		}

		/// <summary>
		/// 确认入库（兼容保留）：全部明细已上架才允许，否则提示先完成上架
		/// </summary>
		[HttpPut("{id}/confirm")]
		public ReturnData ConfirmInbound(long id)
		{
			return _receiptBiz.ConfirmInbound(id);
		}

		/// <summary>
		/// 明细上架：locationId + quantity，写库存并流转状态
		/// </summary>
		[HttpPut("{receiptId}/details/{detailId}/shelve")]
		public ReturnData Shelve(long receiptId, long detailId, [FromBody] ShelveRequest request)
		{
			return _receiptBiz.Shelve(receiptId, detailId, request.LocationId, request.Quantity);
		}

		/// <summary>
		/// 按条码上架（PDA 扫码场景）
		/// </summary>
		[HttpPut("shelve-by-barcode")]
		public ReturnData ShelveByBarcode([FromBody] ShelveByBarcodeRequest request)
		{
			return _receiptBiz.ShelveByBarcode(request.Barcode, request.LocationId, request.Quantity);
		}

		/// <summary>
		/// 按条码查询明细条码（PDA 扫码回显）
		/// </summary>
		[HttpGet("barcode/{barcode}")]
		public ReturnData GetDetailByBarcode(string barcode)
		{
			return new ResultData<WmsReceiptDetailBarcodeEntity>(_receiptBiz.GetDetailByBarcode(barcode));
		}

		/// <summary>
		/// 明细条码列表（每码承载数量可不等）
		/// </summary>
		[HttpGet("{receiptId}/details/{detailId}/barcodes")]
		public ReturnData GetBarcodes(long receiptId, long detailId)
		{
			return new RowData<WmsReceiptDetailBarcodeEntity>(_receiptBiz.GetBarcodes(detailId));
		}

		/// <summary>
		/// 拆分条码：一个待上架的码拆成多个（数量之和 = 原数量）
		/// </summary>
		[HttpPut("{receiptId}/details/{detailId}/barcodes/split")]
		public ReturnData SplitBarcode(long receiptId, long detailId, [FromBody] SplitBarcodeRequest request)
		{
			return _receiptBiz.SplitBarcode(detailId, request.BarcodeId, request.Quantities);
		}

		/// <summary>
		/// 合并条码：同一明细多个待上架的码合并为一个（数量相加）
		/// </summary>
		[HttpPut("{receiptId}/details/{detailId}/barcodes/merge")]
		public ReturnData MergeBarcodes(long receiptId, long detailId, [FromBody] MergeBarcodesRequest request)
		{
			return _receiptBiz.MergeBarcodes(detailId, request.BarcodeIds);
		}
	}

	/// <summary>上架请求</summary>
	public class ShelveRequest
	{
		public long LocationId { get; set; }
		public int Quantity { get; set; }
	}

	/// <summary>按条码上架请求</summary>
	public class ShelveByBarcodeRequest
	{
		public string Barcode { get; set; }
		public long LocationId { get; set; }
		public int Quantity { get; set; }
	}

	/// <summary>拆分条码请求</summary>
	public class SplitBarcodeRequest
	{
		public long BarcodeId { get; set; }
		public List<decimal> Quantities { get; set; }
	}

	/// <summary>合并条码请求</summary>
	public class MergeBarcodesRequest
	{
		public List<long> BarcodeIds { get; set; }
	}
}
