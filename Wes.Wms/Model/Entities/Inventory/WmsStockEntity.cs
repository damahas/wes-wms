using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_stock", "实时库存表")]
	public class WmsStockEntity
	{
		/// <summary>
		/// 库存主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "stock_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "库存id")]
		public long StockId { get; set; }

		/// <summary>
		/// 库存条码：入库单明细生成时分配，上架后随库存一起流转，出库/拣货/盘点全程使用
		/// </summary>
		[SugarColumn(ColumnName = "barcode", IsNullable = true, Length = 50, ColumnDescription = "库存条码")]
		public string Barcode { get; set; }

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

		[SugarColumn(ColumnName = "material_code", IsNullable = true, Length = 50)]
		public string MaterialCode { get; set; }

		[SugarColumn(ColumnName = "material_name", IsNullable = true, Length = 100)]
		public string MaterialName { get; set; }

		[SugarColumn(ColumnName = "spec", IsNullable = true, Length = 100, ColumnDescription = "规格")]
		public string Spec { get; set; }

		[SugarColumn(ColumnName = "unit", IsNullable = true, Length = 20, ColumnDescription = "单位")]
		public string Unit { get; set; }

		[SugarColumn(ColumnName = "batch_no", IsNullable = true, Length = 50, ColumnDescription = "批次号")]
		public string BatchNo { get; set; }

		/// <summary>
		/// 仓库ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = false, ColumnDescription = "仓库id")]
		public long WarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(WarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity Warehouse { get; set; }

		[SugarColumn(ColumnName = "warehouse_name", IsNullable = true, Length = 100)]
		public string WarehouseName { get; set; }

		/// <summary>
		/// 库区ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "zone_id", IsNullable = true, ColumnDescription = "库区id")]
		public long? ZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity Zone { get; set; }

		[SugarColumn(ColumnName = "zone_code", IsNullable = true, Length = 50)]
		public string ZoneCode { get; set; }

		[SugarColumn(ColumnName = "zone_name", IsNullable = true, Length = 100)]
		public string ZoneName { get; set; }

		/// <summary>
		/// 库位ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "location_id", IsNullable = false, ColumnDescription = "库位id")]
		public long LocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(LocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity Location { get; set; }

		[SugarColumn(ColumnName = "location_code", IsNullable = true, Length = 50)]
		public string LocationCode { get; set; }

		[SugarColumn(ColumnName = "location_name", IsNullable = true, Length = 100)]
		public string LocationName { get; set; }

		/// <summary>
		/// 处理单元ID（非HU物料为NULL）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "hu_id", IsNullable = true, ColumnDescription = "处理单元id")]
		public long? HuId { get; set; }

		/// <summary>
		/// 包装规格层级ID（本条码对应的包装层级，如"箱"）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "pack_spec_id", IsNullable = true, ColumnDescription = "包装规格层级id")]
		public long? PackSpecId { get; set; }

		/// <summary>
		/// 供应商ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "supplier_id", IsNullable = true)]
		public long? SupplierId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(SupplierId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Supplier { get; set; }

		[SugarColumn(ColumnName = "production_date", IsNullable = true, ColumnDescription = "生产日期")]
		public DateTime? ProductionDate { get; set; }

		[SugarColumn(ColumnName = "expire_date", IsNullable = true, ColumnDescription = "有效期至")]
		public DateTime? ExpireDate { get; set; }

		/// <summary>
		/// 库存数量
		/// </summary>
		[SugarColumn(ColumnName = "stock_quantity", IsNullable = true, ColumnDescription = "库存数量")]
		public decimal? StockQuantity { get; set; }

		/// <summary>
		/// 基本单位数量（按包装规格折算，拆箱出零时按此记账）
		/// </summary>
		[SugarColumn(ColumnName = "base_quantity", IsNullable = true, ColumnDescription = "基本单位数量")]
		public decimal? BaseQuantity { get; set; }

		/// <summary>
		/// 可用数量 = 库存数量 − 冻结 − 锁定
		/// </summary>
		[SugarColumn(ColumnName = "available_quantity", IsNullable = true, ColumnDescription = "可用数量")]
		public decimal? AvailableQuantity { get; set; }

		[SugarColumn(ColumnName = "freeze_quantity", IsNullable = true, ColumnDescription = "冻结数量")]
		public decimal? FreezeQuantity { get; set; }

		[SugarColumn(ColumnName = "lock_quantity", IsNullable = true, ColumnDescription = "锁定数量")]
		public decimal? LockQuantity { get; set; }

		[SugarColumn(ColumnName = "cost_price", IsNullable = true, ColumnDescription = "成本单价")]
		public decimal? CostPrice { get; set; }

		[SugarColumn(ColumnName = "cost_amount", IsNullable = true, ColumnDescription = "成本金额")]
		public decimal? CostAmount { get; set; }

		/// <summary>
		/// 来源入库单ID / 入库明细ID（溯源）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receipt_id", IsNullable = true)]
		public long? ReceiptId { get; set; }

		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receipt_detail_id", IsNullable = true)]
		public long? ReceiptDetailId { get; set; }

		/// <summary>
		/// 逻辑删除标记（0=否 1=是）
		/// </summary>
		[SugarColumn(ColumnName = "is_del", DefaultValue = "0", IsNullable = false, ColumnDescription = "删除标志")]
		public int IsDel { get; set; }

		[SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建者")]
		public string CreateBy { get; set; }

		[SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
		public DateTime? CreateTime { get; set; }

		[SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64, ColumnDescription = "更新者")]
		public string UpdateBy { get; set; }

		[SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
		public DateTime? UpdateTime { get; set; }
	}
}
