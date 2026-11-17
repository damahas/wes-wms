using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_return_out", "出库退货表")]
	public class WmsReturnOutEntity
	{
		/// <summary>
		/// 出库退货主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "return_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "退货id")]
		public long ReturnId { get; set; }

		/// <summary>
		/// 退货单号
		/// </summary>
		[SugarColumn(ColumnName = "return_no", IsNullable = false, Length = 50, ColumnDescription = "退货单号")]
		public string ReturnNo { get; set; }

		/// <summary>
		/// 退货类型
		/// </summary>
		[SugarColumn(ColumnName = "return_type", IsNullable = true, Length = 20, ColumnDescription = "退货类型 return_out-退货出库")]
		public string ReturnType { get; set; }

		/// <summary>
		/// 关联出库单ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "delivery_id", IsNullable = true, ColumnDescription = "原出库id")]
		public long? DeliveryId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(DeliveryId), nameof(WmsDeliveryEntity.DeliveryId))]
		public WmsDeliveryEntity Delivery { get; set; }

		/// <summary>
		/// 客户ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "customer_id", IsNullable = true, ColumnDescription = "客户id")]
		public long? CustomerId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(CustomerId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Customer { get; set; }

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
		/// 退货日期
		/// </summary>
		[SugarColumn(ColumnName = "return_date", IsNullable = false, ColumnDescription = "退货日期")]
		public DateTime? ReturnDate { get; set; }

		/// <summary>
		/// 退货总数量
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "退货总数量")]
		public int? TotalQuantity { get; set; }

		/// <summary>
		/// 退货原因
		/// </summary>
		[SugarColumn(ColumnName = "return_reason", IsNullable = true, Length = 255, ColumnDescription = "退货原因")]
		public string ReturnReason { get; set; }

		/// <summary>
		/// 退货状态（字典：待入库/已入库）
		/// </summary>
		[SugarColumn(ColumnName = "return_status", IsNullable = true, Length = 20, ColumnDescription = "退货状态 pending-待审核 approved-已审核 received-已收货 completed-已完成")]
		public string ReturnStatus { get; set; }

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
