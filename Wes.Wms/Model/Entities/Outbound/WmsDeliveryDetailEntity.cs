using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_delivery_detail", "出库单明细表")]
	public class WmsDeliveryDetailEntity
	{
		/// <summary>
		/// 出库明细主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "detail_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "明细id")]
		public long DetailId { get; set; }

		/// <summary>
		/// 出库单ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "delivery_id", IsNullable = false, ColumnDescription = "出库id")]
		public long DeliveryId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(DeliveryId), nameof(WmsDeliveryEntity.DeliveryId))]
		public WmsDeliveryEntity Delivery { get; set; }

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

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
		/// 库区ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "zone_id", IsNullable = true, ColumnDescription = "库区id")]
		public long? ZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity Zone { get; set; }

		/// <summary>
		/// 库位ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "location_id", IsNullable = true, ColumnDescription = "库位id")]
		public long? LocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(LocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity Location { get; set; }

		/// <summary>
		/// 出库数量
		/// </summary>
		[SugarColumn(ColumnName = "delivery_quantity", IsNullable = false, ColumnDescription = "出库数量")]
		public int DeliveryQuantity { get; set; }

		/// <summary>
		/// 单价
		/// </summary>
		[SugarColumn(ColumnName = "unit_price", IsNullable = true, ColumnDescription = "单价")]
		public decimal? UnitPrice { get; set; }

		/// <summary>
		/// 金额
		/// </summary>
		[SugarColumn(ColumnName = "amount", IsNullable = true, ColumnDescription = "金额")]
		public decimal? Amount { get; set; }

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
