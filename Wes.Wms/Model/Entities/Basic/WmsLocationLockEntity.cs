using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_location_lock", "库位锁定记录表")]
	public class WmsLocationLockEntity
	{
		/// <summary>
		/// 锁定记录ID（雪花主键）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "lock_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "锁定id")]
		public long LockId { get; set; }

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
		/// 库位ID（被锁定的库位）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "location_id", IsNullable = false, ColumnDescription = "库位id")]
		public long LocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(LocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity Location { get; set; }

		/// <summary>
		/// 锁定类型（如 盘点/移库/冻结，按业务字典）
		/// </summary>
		[SugarColumn(ColumnName = "lock_type", IsNullable = false, Length = 20, ColumnDescription = "锁定类型 check-盘点异常 maintain-维护 other-其他")]
		public string LockType { get; set; }

		/// <summary>
		/// 锁定原因
		/// </summary>
		[SugarColumn(ColumnName = "lock_reason", IsNullable = true, Length = 255, ColumnDescription = "锁定原因")]
		public string LockReason { get; set; }

		/// <summary>
		/// 锁定人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "lock_user_id", IsNullable = true, ColumnDescription = "锁定人id")]
		public long? LockUserId { get; set; }

		/// <summary>
		/// 锁定时间
		/// </summary>
		[SugarColumn(ColumnName = "lock_time", IsNullable = true, ColumnDescription = "锁定时间")]
		public DateTime? LockTime { get; set; }

		/// <summary>
		/// 解锁人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "unlock_user_id", IsNullable = true, ColumnDescription = "解锁人id")]
		public long? UnlockUserId { get; set; }

		/// <summary>
		/// 解锁时间
		/// </summary>
		[SugarColumn(ColumnName = "unlock_time", IsNullable = true, ColumnDescription = "解锁时间")]
		public DateTime? UnlockTime { get; set; }

		/// <summary>
		/// 逻辑删除：0=未删，1=已删
		/// </summary>
		[SugarColumn(ColumnName = "is_del", DefaultValue = "0", IsNullable = true, ColumnDescription = "删除标志")]
		public int IsDel { get; set; }

		/// <summary>
		/// 创建人账号
		/// </summary>
		[SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建者")]
		public string CreateBy { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
		public DateTime? CreateTime { get; set; }

		/// <summary>
		/// 更新人账号
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
