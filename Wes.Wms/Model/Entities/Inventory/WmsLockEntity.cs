using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_lock", "库存锁定表")]
	public class WmsLockEntity
	{
		/// <summary>
		/// 锁定记录主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "lock_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "锁定id")]
		public long LockId { get; set; }

		/// <summary>
		/// 锁定类型（字典：出库锁定/拣货锁定等）
		/// </summary>
		[SugarColumn(ColumnName = "lock_type", IsNullable = true, Length = 20, ColumnDescription = "锁定类型 order-订单预留 produce-生产预留 exhibit-展会预留 manual-手动锁定")]
		public string LockType { get; set; }

		/// <summary>
		/// 来源类型（关联单据类型，如 出库单/拣货任务）
		/// </summary>
		[SugarColumn(ColumnName = "source_type", IsNullable = true, Length = 20, ColumnDescription = "来源类型")]
		public string SourceType { get; set; }

		/// <summary>
		/// 来源单据ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "source_id", IsNullable = true, ColumnDescription = "来源id")]
		public long? SourceId { get; set; }

		/// <summary>
		/// 来源单据号
		/// </summary>
		[SugarColumn(ColumnName = "source_no", IsNullable = true, Length = 50, ColumnDescription = "来源单据号")]
		public string SourceNo { get; set; }

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

		/// <summary>
		/// 批次号
		/// </summary>
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
		/// 库存行id（同物料同批次可能多条码行，必须精确到行）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "stock_id", IsNullable = true, ColumnDescription = "库存行id")]
		public long? StockId { get; set; }

		/// <summary>
		/// 锁定数量
		/// </summary>
		[SugarColumn(ColumnName = "lock_quantity", IsNullable = false, ColumnDescription = "锁定数量")]
		public int LockQuantity { get; set; }

		/// <summary>
		/// 已使用数量（已被实际扣减部分）
		/// </summary>
		[SugarColumn(ColumnName = "used_quantity", IsNullable = true, ColumnDescription = "已使用数量")]
		public int? UsedQuantity { get; set; }

		/// <summary>
		/// 锁定状态（字典：锁定中/已释放）
		/// </summary>
		[SugarColumn(ColumnName = "lock_status", IsNullable = true, Length = 20, ColumnDescription = "锁定状态 locked-已锁定 partial-部分释放 released-已释放")]
		public string LockStatus { get; set; }

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
		/// 失效时间（锁定到期自动释放）
		/// </summary>
		[SugarColumn(ColumnName = "expire_time", IsNullable = true, ColumnDescription = "过期时间")]
		public DateTime? ExpireTime { get; set; }

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
