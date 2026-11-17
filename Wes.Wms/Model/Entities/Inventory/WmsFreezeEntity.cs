using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_freeze", "库存冻结表")]
	public class WmsFreezeEntity
	{
		/// <summary>
		/// 冻结记录主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "freeze_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "冻结id")]
		public long FreezeId { get; set; }

		/// <summary>
		/// 冻结单号
		/// </summary>
		[SugarColumn(ColumnName = "freeze_no", IsNullable = true, Length = 50, ColumnDescription = "冻结单号")]
		public string FreezeNo { get; set; }

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
		/// 冻结数量
		/// </summary>
		[SugarColumn(ColumnName = "freeze_quantity", IsNullable = false, ColumnDescription = "冻结数量")]
		public int FreezeQuantity { get; set; }

		/// <summary>
		/// 冻结类型（字典：质检冻结/订单冻结/异常冻结等）
		/// </summary>
		[SugarColumn(ColumnName = "freeze_type", IsNullable = true, Length = 20, ColumnDescription = "冻结类型 quality-质量隔离 legal-司法查封 reserve-预留锁定 other-其他")]
		public string FreezeType { get; set; }

		/// <summary>
		/// 冻结原因
		/// </summary>
		[SugarColumn(ColumnName = "freeze_reason", IsNullable = true, Length = 255, ColumnDescription = "冻结原因")]
		public string FreezeReason { get; set; }

		/// <summary>
		/// 冻结状态（字典：冻结中/已解冻）
		/// </summary>
		[SugarColumn(ColumnName = "freeze_status", IsNullable = true, Length = 20, ColumnDescription = "冻结状态 frozen-已冻结 thawed-已解冻")]
		public string FreezeStatus { get; set; }

		/// <summary>
		/// 冻结人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "freeze_user_id", IsNullable = true, ColumnDescription = "冻结人id")]
		public long? FreezeUserId { get; set; }

		/// <summary>
		/// 冻结时间
		/// </summary>
		[SugarColumn(ColumnName = "freeze_time", IsNullable = true, ColumnDescription = "冻结时间")]
		public DateTime? FreezeTime { get; set; }

		/// <summary>
		/// 解冻人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "thaw_user_id", IsNullable = true, ColumnDescription = "解冻人id")]
		public long? ThawUserId { get; set; }

		/// <summary>
		/// 解冻时间
		/// </summary>
		[SugarColumn(ColumnName = "thaw_time", IsNullable = true, ColumnDescription = "解冻时间")]
		public DateTime? ThawTime { get; set; }

		/// <summary>
		/// 解冻原因
		/// </summary>
		[SugarColumn(ColumnName = "thaw_reason", IsNullable = true, Length = 255, ColumnDescription = "解冻原因")]
		public string ThawReason { get; set; }

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
