using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_receipt_detail", "入库单明细表")]
	public class WmsReceiptDetailEntity
	{
		/// <summary>
		/// 入库明细主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "detail_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "明细id")]
		public long DetailId { get; set; }

		/// <summary>
		/// 入库单ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receipt_id", IsNullable = false, ColumnDescription = "入库id")]
		public long ReceiptId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ReceiptId), nameof(WmsReceiptEntity.ReceiptId))]
		public WmsReceiptEntity Receipt { get; set; }

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

		/// <summary>
		/// 物料编码（非数据库字段，查询明细时从物料表带出，供前端展示）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string MaterialCode { get; set; }

		/// <summary>
		/// 物料名称（非数据库字段，查询明细时从物料表带出，供前端展示）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string MaterialName { get; set; }

		/// <summary>
		/// 规格
		/// </summary>
		[SugarColumn(ColumnName = "spec", IsNullable = true, Length = 100, ColumnDescription = "规格")]
		public string Spec { get; set; }

		/// <summary>
		/// 计量单位
		/// </summary>
		[SugarColumn(ColumnName = "unit", IsNullable = true, Length = 20, ColumnDescription = "单位")]
		public string Unit { get; set; }

		/// <summary>
		/// 批次号
		/// </summary>
		[SugarColumn(ColumnName = "batch_no", IsNullable = true, Length = 50, ColumnDescription = "批次号")]
		public string BatchNo { get; set; }

		/// <summary>
		/// 生产日期
		/// </summary>
		[SugarColumn(ColumnName = "materialion_date", IsNullable = true)]
		public DateTime? MaterialionDate { get; set; }

		/// <summary>
		/// 失效日期
		/// </summary>
		[SugarColumn(ColumnName = "expire_date", IsNullable = true, ColumnDescription = "有效期至")]
		public DateTime? ExpireDate { get; set; }

		/// <summary>
		/// 入库数量
		/// </summary>
		[SugarColumn(ColumnName = "receipt_quantity", IsNullable = false, ColumnDescription = "入库数量")]
		public int ReceiptQuantity { get; set; }

		/// <summary>
		/// 已上架数量
		/// </summary>
		[SugarColumn(ColumnName = "shelf_quantity", IsNullable = true, ColumnDescription = "已上架数量")]
		public int? ShelfQuantity { get; set; }

		/// <summary>
		/// 库存条码：明细生成时分配，上架后写入库存，出库/拣货/盘点全程使用
		/// </summary>
		[SugarColumn(ColumnName = "barcode", IsNullable = true, Length = 50, ColumnDescription = "库存条码")]
		public string Barcode { get; set; }

		/// <summary>
		/// 明细上架状态 pending-待上架 shelving-上架中 shelved-已上架
		/// </summary>
		[SugarColumn(ColumnName = "detail_status", IsNullable = true, Length = 20, ColumnDescription = "上架状态")]
		public string DetailStatus { get; set; }

		/// <summary>
		/// 条码总数（一箱一码；非数据库字段，按明细条码表统计）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public int BarcodeCount { get; set; }

		/// <summary>
		/// 已上架条码数（非数据库字段）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public int ShelvedBarcodeCount { get; set; }

		/// <summary>
		/// 库位ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "location_id", IsNullable = true, ColumnDescription = "库位id")]
		public long? LocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(LocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity Location { get; set; }

		/// <summary>
		/// 供应商ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "supplier_id", IsNullable = true, ColumnDescription = "供应商id")]
		public long? SupplierId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(SupplierId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Supplier { get; set; }

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
