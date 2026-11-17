using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_stock_warning", "库存预警表")]
	public class WmsStockWarningEntity
	{
		/// <summary>
		/// 预警记录主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warning_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "预警id")]
		public long WarningId { get; set; }

		/// <summary>
		/// 预警类型（字典：低库存/高库存/效期预警等）
		/// </summary>
		[SugarColumn(ColumnName = "warning_type", IsNullable = false, Length = 30, ColumnDescription = "预警类型 min_stock-安全库存 max_stock-最大库存 shelf_life-保质期呆滞-stagnation")]
		public string WarningType { get; set; }

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

		/// <summary>
		/// 仓库ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = true, ColumnDescription = "仓库id")]
		public long? WarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(WarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity Warehouse { get; set; }

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
		/// 来源配置ID（由哪条预警配置触发）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "config_id", IsNullable = true, ColumnDescription = "来源配置id")]
		public long? ConfigId { get; set; }

		/// <summary>
		/// 通知人（预警产生时从配置快照，逗号分隔用户id）
		/// </summary>
		[SugarColumn(ColumnName = "notify_user_ids", IsNullable = true, Length = 500, ColumnDescription = "通知人（逗号分隔用户id）")]
		public string NotifyUserIds { get; set; }

		/// <summary>
		/// 当前数量
		/// </summary>
		[SugarColumn(ColumnName = "current_quantity", IsNullable = true, ColumnDescription = "当前库存")]
		public int? CurrentQuantity { get; set; }

		/// <summary>
		/// 阈值（预警触发数量/条件值）
		/// </summary>
		[SugarColumn(ColumnName = "threshold_value", IsNullable = true, ColumnDescription = "阈值")]
		public decimal? ThresholdValue { get; set; }

		/// <summary>
		/// 偏差值（当前值 - 阈值）
		/// </summary>
		[SugarColumn(ColumnName = "diff_value", IsNullable = true, ColumnDescription = "差值")]
		public decimal? DiffValue { get; set; }

		/// <summary>
		/// 预警级别（字典：提示/警告/严重）
		/// </summary>
		[SugarColumn(ColumnName = "warning_level", IsNullable = true, Length = 10, ColumnDescription = "预警级别 normal-一般 urgent-紧急 critical-危机")]
		public string WarningLevel { get; set; }

		/// <summary>
		/// 预警状态（字典：待处理/已处理/已忽略/已自动恢复）
		/// </summary>
		[SugarColumn(ColumnName = "warning_status", IsNullable = true, Length = 20, ColumnDescription = "处理状态 pending-待处理 handled-已处理 ignored-已忽略 resolved-已自动恢复")]
		public string WarningStatus { get; set; }

		/// <summary>
		/// 本周期累计触发次数
		/// </summary>
		[SugarColumn(ColumnName = "trigger_count", IsNullable = true, ColumnDescription = "本周期累计触发次数")]
		public int? TriggerCount { get; set; }

		/// <summary>
		/// 最近触发时间
		/// </summary>
		[SugarColumn(ColumnName = "last_trigger_time", IsNullable = true, ColumnDescription = "最近触发时间")]
		public DateTime? LastTriggerTime { get; set; }

		/// <summary>
		/// 自动恢复时间（库存回到阈值内时系统自动关闭）
		/// </summary>
		[SugarColumn(ColumnName = "resolved_time", IsNullable = true, ColumnDescription = "自动恢复时间")]
		public DateTime? ResolvedTime { get; set; }

		/// <summary>
		/// 恢复时的库存量
		/// </summary>
		[SugarColumn(ColumnName = "resolved_quantity", IsNullable = true, ColumnDescription = "恢复时的库存量")]
		public decimal? ResolvedQuantity { get; set; }

		/// <summary>
		/// 处理人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "handle_user_id", IsNullable = true, ColumnDescription = "处理人id")]
		public long? HandleUserId { get; set; }

		/// <summary>
		/// 处理时间
		/// </summary>
		[SugarColumn(ColumnName = "handle_time", IsNullable = true, ColumnDescription = "处理时间")]
		public DateTime? HandleTime { get; set; }

		/// <summary>
		/// 处理说明
		/// </summary>
		[SugarColumn(ColumnName = "handle_remark", IsNullable = true, Length = 500, ColumnDescription = "处理说明")]
		public string HandleRemark { get; set; }

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
