using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_delivery", "出库单主表")]
	public class WmsDeliveryEntity
	{
		/// <summary>
		/// 出库单主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "delivery_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "出库id")]
		public long DeliveryId { get; set; }

		/// <summary>
		/// 出库单号
		/// </summary>
		[SugarColumn(ColumnName = "delivery_no", IsNullable = false, Length = 50, ColumnDescription = "出库单号")]
		public string DeliveryNo { get; set; }

		/// <summary>
		/// 来源单据类型 sale-销售出库订单 issue-领料单 manual-手工创建
		/// </summary>
		[SugarColumn(ColumnName = "source_type", IsNullable = true, Length = 20, ColumnDescription = "来源单据类型 sale-销售出库订单 issue-领料单 manual-手工创建")]
		public string SourceType { get; set; }

		/// <summary>
		/// 来源单据id（配合 SourceType 使用；一张领料单可多次发料，对应多个出库单）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "source_id", IsNullable = true, ColumnDescription = "来源单据id")]
		public long? SourceId { get; set; }

		/// <summary>
		/// 来源单据号（冗余，列表直接显示）
		/// </summary>
		[SugarColumn(ColumnName = "source_no", IsNullable = true, Length = 50, ColumnDescription = "来源单据号")]
		public string SourceNo { get; set; }

		/// <summary>
		/// 仓库ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = false, ColumnDescription = "仓库id")]
		public long WarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(WarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity Warehouse { get; set; }

		/// <summary>
		/// 库区ID（出库库区）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "zone_id", IsNullable = true, ColumnDescription = "发货库区id")]
		public long? ZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity Zone { get; set; }

		/// <summary>
		/// 客户ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "customer_id", IsNullable = true, ColumnDescription = "客户id")]
		public long? CustomerId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(CustomerId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Customer { get; set; }

		/// <summary>
		/// 出库日期
		/// </summary>
		[SugarColumn(ColumnName = "delivery_date", IsNullable = false, ColumnDescription = "出库日期")]
		public DateTime? DeliveryDate { get; set; }

		/// <summary>
		/// 出库总数量
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "总数量")]
		public int? TotalQuantity { get; set; }

		/// <summary>
		/// 出库总金额
		/// </summary>
		[SugarColumn(ColumnName = "total_amount", IsNullable = true, ColumnDescription = "总金额")]
		public decimal? TotalAmount { get; set; }

		/// <summary>
		/// 承运商名称
		/// </summary>
		[SugarColumn(ColumnName = "carrier_name", IsNullable = true, Length = 100, ColumnDescription = "承运商")]
		public string CarrierName { get; set; }

		/// <summary>
		/// 运单号
		/// </summary>
		[SugarColumn(ColumnName = "waybill_no", IsNullable = true, Length = 100, ColumnDescription = "运单号")]
		public string WaybillNo { get; set; }

		/// <summary>
		/// 包裹件数
		/// </summary>
		[SugarColumn(ColumnName = "package_count", IsNullable = true, ColumnDescription = "包裹数量")]
		public int? PackageCount { get; set; }

		/// <summary>
		/// 总重量
		/// </summary>
		[SugarColumn(ColumnName = "total_weight", IsNullable = true, ColumnDescription = "总重量")]
		public decimal? TotalWeight { get; set; }

		/// <summary>
		/// 出库单状态（字典：待出库/出库中/已出库/已签收/已作废）
		/// </summary>
		[SugarColumn(ColumnName = "delivery_status", IsNullable = true, Length = 20, ColumnDescription = "出库状态 pending-待确认 shipped-已发货 delivered-已签收")]
		public string DeliveryStatus { get; set; }

		/// <summary>
		/// 发货时间
		/// </summary>
		[SugarColumn(ColumnName = "ship_time", IsNullable = true, ColumnDescription = "发货时间")]
		public DateTime? ShipTime { get; set; }

		/// <summary>
		/// 签收时间
		/// </summary>
		[SugarColumn(ColumnName = "sign_time", IsNullable = true, ColumnDescription = "签收时间")]
		public DateTime? SignTime { get; set; }

		/// <summary>
		/// 操作人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "operator_id", IsNullable = true, ColumnDescription = "操作人id")]
		public long? OperatorId { get; set; }

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
		/// 出库明细列表
		/// </summary>
		[Navigate(NavigateType.OneToMany, nameof(WmsDeliveryDetailEntity.DeliveryId), nameof(DeliveryId))]
		public List<WmsDeliveryDetailEntity> Details { get; set; }

	}
}
