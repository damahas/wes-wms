using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_transfer_detail", "调拨明细表")]
	public class WmsTransferDetailEntity
	{
		/// <summary>
		/// 调拨明细主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "detail_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "明细id")]
		public long DetailId { get; set; }

		/// <summary>
		/// 调拨单ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "transfer_id", IsNullable = false, ColumnDescription = "调拨id")]
		public long TransferId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(TransferId), nameof(WmsTransferEntity.TransferId))]
		public WmsTransferEntity Transfer { get; set; }

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

		/// <summary>物料编码（冗余，随物料 id 写入）</summary>
		[SugarColumn(ColumnName = "material_code", IsNullable = true, Length = 50, ColumnDescription = "物料编码")]
		public string MaterialCode { get; set; }

		/// <summary>物料名称（冗余，随物料 id 写入）</summary>
		[SugarColumn(ColumnName = "material_name", IsNullable = true, Length = 100, ColumnDescription = "物料名称")]
		public string MaterialName { get; set; }

		/// <summary>规格型号（冗余）</summary>
		[SugarColumn(ColumnName = "spec", IsNullable = true, Length = 100, ColumnDescription = "规格型号")]
		public string Spec { get; set; }

		/// <summary>单位（冗余）</summary>
		[SugarColumn(ColumnName = "unit", IsNullable = true, Length = 20, ColumnDescription = "单位")]
		public string Unit { get; set; }

		/// <summary>
		/// 批次号
		/// </summary>
		[SugarColumn(ColumnName = "batch_no", IsNullable = true, Length = 50, ColumnDescription = "批次号")]
		public string BatchNo { get; set; }

		/// <summary>
		/// 条码（跟着库存走）
		/// 移出时记下源库存行的条码，移入时带到调入仓的新库存行上——
		/// 一条库存一个条码，容器还是那个容器，只是换了地方；除非发生拆分或合并，条码不变。
		/// </summary>
		[SugarColumn(ColumnName = "barcode", IsNullable = true, Length = 50, ColumnDescription = "条码")]
		public string Barcode { get; set; }

		/// <summary>调出库区ID</summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "from_zone_id", IsNullable = true, ColumnDescription = "调出库区id")]
		public long? FromZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(FromZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity FromZone { get; set; }

		/// <summary>
		/// 调出库位ID（为空时移出环节按 FEFO 自动挑库位）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "from_location_id", IsNullable = true, ColumnDescription = "调出库位id")]
		public long? FromLocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(FromLocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity FromLocation { get; set; }

		/// <summary>调出库位编码（冗余，FEFO 自动挑库位时回填）</summary>
		[SugarColumn(ColumnName = "from_location_code", IsNullable = true, Length = 50, ColumnDescription = "调出库位编码")]
		public string FromLocationCode { get; set; }

		/// <summary>调入库区ID</summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "to_zone_id", IsNullable = true, ColumnDescription = "调入库区id")]
		public long? ToZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ToZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity ToZone { get; set; }

		/// <summary>
		/// 调入库位ID
		/// 跨仓调拨时开单不知道对方仓哪个货位空着，所以这里允许为空，收货时再定。
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "to_location_id", IsNullable = true, ColumnDescription = "调入库位id")]
		public long? ToLocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ToLocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity ToLocation { get; set; }

		/// <summary>调入库位编码（冗余，收货时回填）</summary>
		[SugarColumn(ColumnName = "to_location_code", IsNullable = true, Length = 50, ColumnDescription = "调入库位编码")]
		public string ToLocationCode { get; set; }

		/// <summary>
		/// 调拨数量
		/// </summary>
		[SugarColumn(ColumnName = "transfer_quantity", IsNullable = false, ColumnDescription = "调拨数量")]
		public decimal TransferQuantity { get; set; }

		/// <summary>已出库数量</summary>
		[SugarColumn(ColumnName = "out_quantity", IsNullable = true, ColumnDescription = "已出库数量")]
		public decimal? OutQuantity { get; set; }

		/// <summary>已入库数量（实收）</summary>
		[SugarColumn(ColumnName = "in_quantity", IsNullable = true, ColumnDescription = "已入库数量")]
		public decimal? InQuantity { get; set; }

		/// <summary>
		/// 在途数量（已移出未移入）
		/// 允许少收：移入按实收入账后，差额继续留在这里，不会凭空消失也不会凭空入库。
		/// </summary>
		[SugarColumn(ColumnName = "in_transit_quantity", IsNullable = true, ColumnDescription = "在途数量")]
		public decimal? InTransitQuantity { get; set; }

		/// <summary>
		/// 明细状态
		/// </summary>
		[SugarColumn(ColumnName = "transfer_status", IsNullable = true, Length = 20, ColumnDescription = "状态 pending-待调拨 in_transit-在途 completed-已完成")]
		public string TransferStatus { get; set; }

		/// <summary>移出时间</summary>
		[SugarColumn(ColumnName = "out_time", IsNullable = true, ColumnDescription = "移出时间")]
		public DateTime? OutTime { get; set; }

		/// <summary>移入时间</summary>
		[SugarColumn(ColumnName = "in_time", IsNullable = true, ColumnDescription = "移入时间")]
		public DateTime? InTime { get; set; }

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

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

	}
}
