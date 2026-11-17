using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_stock_warning_log", "库存预警触发明细表")]
	public class WmsStockWarningLogEntity
	{
		/// <summary>
		/// 明细主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "log_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "明细id")]
		public long LogId { get; set; }

		/// <summary>
		/// 预警记录ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warning_id", IsNullable = false, ColumnDescription = "预警记录id")]
		public long WarningId { get; set; }

		/// <summary>
		/// 来源配置ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "config_id", IsNullable = true, ColumnDescription = "来源配置id")]
		public long? ConfigId { get; set; }

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }

		/// <summary>
		/// 触发时库存量
		/// </summary>
		[SugarColumn(ColumnName = "current_quantity", IsNullable = false, ColumnDescription = "触发时库存量")]
		public decimal CurrentQuantity { get; set; }

		/// <summary>
		/// 阈值
		/// </summary>
		[SugarColumn(ColumnName = "threshold_value", IsNullable = false, ColumnDescription = "阈值")]
		public decimal ThresholdValue { get; set; }

		/// <summary>
		/// 差值（当前值 - 阈值）
		/// </summary>
		[SugarColumn(ColumnName = "diff_value", IsNullable = true, ColumnDescription = "差值")]
		public decimal? DiffValue { get; set; }

		/// <summary>
		/// 是否发送站内信（1-是 0-否）
		/// </summary>
		[SugarColumn(ColumnName = "message_sent", IsNullable = true, ColumnDescription = "是否发送站内信 1-是 0-否")]
		public int MessageSent { get; set; }

		/// <summary>
		/// 触发时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "触发时间")]
		public DateTime? CreateTime { get; set; }
	}
}
