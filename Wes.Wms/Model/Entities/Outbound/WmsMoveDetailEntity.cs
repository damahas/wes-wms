using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_move_detail", "移库明细表")]
	public class WmsMoveDetailEntity
	{
		/// <summary>
		/// 移库明细主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "detail_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "明细id")]
		public long DetailId { get; set; }

		/// <summary>
		/// 移库单ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "move_id", IsNullable = false, ColumnDescription = "移库id")]
		public long MoveId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MoveId), nameof(WmsMoveEntity.MoveId))]
		public WmsMoveEntity Move { get; set; }

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

		/// <summary>
		/// 批次号
		/// </summary>
		[SugarColumn(ColumnName = "batch_no", IsNullable = true, Length = 50, ColumnDescription = "批次号")]
		public string BatchNo { get; set; }

		/// <summary>
		/// 包装层级（关联包装规格层级名称）
		/// </summary>
		[SugarColumn(ColumnName = "pack_level", IsNullable = true, Length = 30, ColumnDescription = "包装层级")]
		public string PackLevel { get; set; }

		/// <summary>
		/// 移出库区ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "from_zone_id", IsNullable = false, ColumnDescription = "源库区id")]
		public long FromZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(FromZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity FromZone { get; set; }

		/// <summary>源库区编码（冗余，随库区 id 写入）</summary>
		[SugarColumn(ColumnName = "from_zone_code", IsNullable = true, Length = 50, ColumnDescription = "源库区编码")]
		public string FromZoneCode { get; set; }

		/// <summary>
		/// 移出库位ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "from_location_id", IsNullable = false, ColumnDescription = "原库位id")]
		public long FromLocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(FromLocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity FromLocation { get; set; }

		/// <summary>源库位编码（冗余，随库位 id 写入）</summary>
		[SugarColumn(ColumnName = "from_location_code", IsNullable = true, Length = 50, ColumnDescription = "源库位编码")]
		public string FromLocationCode { get; set; }

		/// <summary>
		/// 移入库区ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "to_zone_id", IsNullable = false, ColumnDescription = "目标库区id")]
		public long ToZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ToZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity ToZone { get; set; }

		/// <summary>目标库区编码（冗余，随库区 id 写入）</summary>
		[SugarColumn(ColumnName = "to_zone_code", IsNullable = true, Length = 50, ColumnDescription = "目标库区编码")]
		public string ToZoneCode { get; set; }

		/// <summary>
		/// 移入库位ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "to_location_id", IsNullable = false, ColumnDescription = "目标库位id")]
		public long ToLocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ToLocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity ToLocation { get; set; }

		/// <summary>目标库位编码（冗余，随库位 id 写入）</summary>
		[SugarColumn(ColumnName = "to_location_code", IsNullable = true, Length = 50, ColumnDescription = "目标库位编码")]
		public string ToLocationCode { get; set; }

		/// <summary>
		/// 移库数量
		/// </summary>
		[SugarColumn(ColumnName = "move_quantity", IsNullable = false, ColumnDescription = "移库数量")]
		public decimal MoveQuantity { get; set; }

		/// <summary>
		/// 移库状态（字典：待移库/已完成）
		/// </summary>
		[SugarColumn(ColumnName = "move_status", IsNullable = true, Length = 20, ColumnDescription = "状态 pending-待执行 completed-已完成")]
		public string MoveStatus { get; set; }

		/// <summary>
		/// 移库时间
		/// </summary>
		[SugarColumn(ColumnName = "move_time", IsNullable = true, ColumnDescription = "移库时间")]
		public DateTime? MoveTime { get; set; }

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
