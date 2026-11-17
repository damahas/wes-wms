using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_stock_alert_config", "库存预警配置表")]
	public class WmsStockAlertConfigEntity
	{
		/// <summary>
		/// 配置主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "config_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "配置id")]
		public long ConfigId { get; set; }

		/// <summary>
		/// 仓库ID（必选）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = false, ColumnDescription = "仓库id")]
		public long WarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(WarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity Warehouse { get; set; }

		/// <summary>
		/// 库区ID（空 = 整个仓库）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "zone_id", IsNullable = true, ColumnDescription = "库区id")]
		public long? ZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity Zone { get; set; }

		/// <summary>
		/// 库位ID（空 = 整个库区）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "location_id", IsNullable = true, ColumnDescription = "库位id")]
		public long? LocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(LocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity Location { get; set; }

		/// <summary>
		/// 物料ID（空 = 范围内全部物料）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = true, ColumnDescription = "物料id")]
		public long? MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

		/// <summary>
		/// 预警类型：min_stock-低于下限 max_stock-高于上限
		/// </summary>
		[SugarColumn(ColumnName = "warning_type", IsNullable = false, Length = 30, ColumnDescription = "预警类型 min_stock-低于下限 max_stock-高于上限")]
		public string WarningType { get; set; }

		/// <summary>
		/// 阈值
		/// </summary>
		[SugarColumn(ColumnName = "threshold_value", IsNullable = false, ColumnDescription = "阈值")]
		public decimal ThresholdValue { get; set; }

		/// <summary>
		/// 预警级别：normal-一般 urgent-紧急 critical-危机
		/// </summary>
		[SugarColumn(ColumnName = "warning_level", IsNullable = true, Length = 10, ColumnDescription = "预警级别 normal-一般 urgent-紧急 critical-危机")]
		public string WarningLevel { get; set; }

		/// <summary>
		/// 处理人（逗号分隔用户id，可多选）
		/// </summary>
		[SugarColumn(ColumnName = "notify_user_ids", IsNullable = true, Length = 500, ColumnDescription = "处理人（逗号分隔用户id）")]
		public string NotifyUserIds { get; set; }

		/// <summary>
		/// 是否启用（1-启用 0-停用）
		/// </summary>
		[SugarColumn(ColumnName = "enabled", IsNullable = true, ColumnDescription = "是否启用 1-启用 0-停用")]
		public int Enabled { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 255, ColumnDescription = "备注")]
		public string Remark { get; set; }

		/// <summary>
		/// 处理人姓名（按 NotifyUserIds 查出拼装，仅显示用，不落库）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string NotifyUserNames { get; set; }

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
