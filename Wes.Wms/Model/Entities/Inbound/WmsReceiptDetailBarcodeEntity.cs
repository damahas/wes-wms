using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_receipt_detail_barcode", "入库单明细条码表")]
	public class WmsReceiptDetailBarcodeEntity
	{
		/// <summary>
		/// 条码ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "barcode_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "条码id")]
		public long BarcodeId { get; set; }

		/// <summary>
		/// 入库单ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receipt_id", IsNullable = false, ColumnDescription = "入库单id")]
		public long ReceiptId { get; set; }

		/// <summary>
		/// 入库明细ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "detail_id", IsNullable = false, ColumnDescription = "入库明细id")]
		public long DetailId { get; set; }

		/// <summary>
		/// 库存条码：一箱一码，上架后写入库存，出库全程使用
		/// </summary>
		[SugarColumn(ColumnName = "barcode", IsNullable = true, Length = 50, ColumnDescription = "库存条码")]
		public string Barcode { get; set; }

		/// <summary>
		/// 箱序号（同一明细下的第几箱）
		/// </summary>
		[SugarColumn(ColumnName = "seq_no", IsNullable = true, ColumnDescription = "箱序号")]
		public int? SeqNo { get; set; }

		/// <summary>
		/// 本码承载数量（明细单位：箱装即为 1 箱）
		/// </summary>
		[SugarColumn(ColumnName = "quantity", IsNullable = true, ColumnDescription = "本码承载数量")]
		public decimal? Quantity { get; set; }

		/// <summary>
		/// 折算基本单位数量（拆箱出零时按此记账）
		/// </summary>
		[SugarColumn(ColumnName = "base_quantity", IsNullable = true, ColumnDescription = "折算基本单位数量")]
		public decimal? BaseQuantity { get; set; }

		/// <summary>
		/// 单位（明细单位）
		/// </summary>
		[SugarColumn(ColumnName = "unit", IsNullable = true, Length = 20, ColumnDescription = "单位")]
		public string Unit { get; set; }

		/// <summary>
		/// 包装规格层级ID（对应 wms_pack_spec.spec_id）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "pack_spec_id", IsNullable = true, ColumnDescription = "包装规格层级id")]
		public long? PackSpecId { get; set; }

		/// <summary>
		/// 批次号
		/// </summary>
		[SugarColumn(ColumnName = "batch_no", IsNullable = true, Length = 50, ColumnDescription = "批次号")]
		public string BatchNo { get; set; }

		/// <summary>
		/// 上架库位ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "location_id", IsNullable = true, ColumnDescription = "上架库位id")]
		public long? LocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(LocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity Location { get; set; }

		/// <summary>
		/// 状态 pending-待上架 shelved-已上架
		/// </summary>
		[SugarColumn(ColumnName = "status", IsNullable = true, Length = 20, ColumnDescription = "状态")]
		public string Status { get; set; }

		/// <summary>
		/// 上架时间
		/// </summary>
		[SugarColumn(ColumnName = "shelve_time", IsNullable = true, ColumnDescription = "上架时间")]
		public DateTime? ShelveTime { get; set; }

		/// <summary>
		/// 逻辑删除标记（0=否 1=是）
		/// </summary>
		[SugarColumn(ColumnName = "is_del", DefaultValue = "0", IsNullable = true, ColumnDescription = "删除标志")]
		public int IsDel { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		[SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建者")]
		public string CreateBy { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
		public DateTime? CreateTime { get; set; }

		/// <summary>
		/// 更新人
		/// </summary>
		[SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64, ColumnDescription = "更新者")]
		public string UpdateBy { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
		public DateTime? UpdateTime { get; set; }
	}
}
