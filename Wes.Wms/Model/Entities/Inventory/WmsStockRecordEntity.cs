using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_stock_record", "库存流水表")]
	public class WmsStockRecordEntity
	{
		/// <summary>
		/// 库存流水记录主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "record_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "记录id")]
		public long RecordId { get; set; }

		/// <summary>
		/// 账务流水号
		/// </summary>
		[SugarColumn(ColumnName = "record_no", IsNullable = true, Length = 50, ColumnDescription = "记录单号")]
		public string RecordNo { get; set; }

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
		/// 业务类型（字典：入库/出库/盘点/调整等）
		/// </summary>
		[SugarColumn(ColumnName = "biz_type", IsNullable = false, Length = 30, ColumnDescription = "业务类型 receipt-入库 delivery-出库 move-移库 check-盘点 adjust-调整 freeze-冻结 thaw-解冻 lock-锁定 unlock-解锁")]
		public string BizType { get; set; }

		/// <summary>
		/// 来源类型（字典：采购入库/销售出库等）
		/// </summary>
		[SugarColumn(ColumnName = "source_type", IsNullable = true, Length = 30, ColumnDescription = "来源单据类型")]
		public string SourceType { get; set; }

		/// <summary>
		/// 来源单据ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "source_id", IsNullable = true, ColumnDescription = "来源单据id")]
		public long? SourceId { get; set; }

		/// <summary>
		/// 来源单据号
		/// </summary>
		[SugarColumn(ColumnName = "source_no", IsNullable = true, Length = 50, ColumnDescription = "来源单据号")]
		public string SourceNo { get; set; }

		/// <summary>
		/// 出入方向（字典：in/out）
		/// </summary>
		[SugarColumn(ColumnName = "direction", IsNullable = false, Length = 10, ColumnDescription = "方向 in-入库 out-出库")]
		public string Direction { get; set; }

		/// <summary>
		/// 变动前数量
		/// </summary>
		[SugarColumn(ColumnName = "before_quantity", IsNullable = true, ColumnDescription = "变动前数量")]
		public decimal? BeforeQuantity { get; set; }

		/// <summary>
		/// 变动数量
		/// </summary>
		[SugarColumn(ColumnName = "change_quantity", IsNullable = false, ColumnDescription = "变动数量")]
		public decimal ChangeQuantity { get; set; }

		/// <summary>
		/// 变动后数量
		/// </summary>
		[SugarColumn(ColumnName = "after_quantity", IsNullable = true, ColumnDescription = "变动后数量")]
		public decimal? AfterQuantity { get; set; }

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
		/// 操作人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "operator_id", IsNullable = true, ColumnDescription = "操作人id")]
		public long? OperatorId { get; set; }

		/// <summary>
		/// 操作时间
		/// </summary>
		[SugarColumn(ColumnName = "operate_time", IsNullable = true, ColumnDescription = "操作时间")]
		public DateTime? OperateTime { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

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
