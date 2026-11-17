using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_receipt", "入库单主表")]
	public class WmsReceiptEntity
	{
		/// <summary>
		/// 入库单主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receipt_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "入库id")]
		public long ReceiptId { get; set; }

		/// <summary>
		/// 入库单号
		/// </summary>
		[SugarColumn(ColumnName = "receipt_no", IsNullable = false, Length = 50, ColumnDescription = "入库单号")]
		public string ReceiptNo { get; set; }

		/// <summary>
		/// 入库类型（字典：采购入库/生产入库/待入库/退货入库/调拨入库/其他）
		/// </summary>
		[SugarColumn(ColumnName = "receipt_type", IsNullable = false, Length = 20, ColumnDescription = "入库类型 purchase-采购入库 produce-生产入库 pending_in-待入库 return_in-退货入库 transfer_in-调拨入库 other-其他入库")]
		public string ReceiptType { get; set; }

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
		[SugarColumn(ColumnName = "zone_id", IsNullable = true, ColumnDescription = "默认入库库区id")]
		public long? ZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity Zone { get; set; }

		/// <summary>
		/// 供应商ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "supplier_id", IsNullable = true, ColumnDescription = "供应商id")]
		public long? SupplierId { get; set; }

		/// <summary>
		/// 供应商信息（导航，由 supplier_id 关联 wms_partner）
		/// </summary>
		[Navigate(NavigateType.OneToOne, nameof(SupplierId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Supplier { get; set; }

		/// <summary>
		/// 客户ID（退货入库时填）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "customer_id", IsNullable = true, ColumnDescription = "客户id")]
		public long? CustomerId { get; set; }

		/// <summary>
		/// 客户信息（导航，由 customer_id 关联 wms_partner）
		/// </summary>
		[Navigate(NavigateType.OneToOne, nameof(CustomerId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Customer { get; set; }

		/// <summary>
		/// 来源类型（关联上游单据类型）
		/// </summary>
		[SugarColumn(ColumnName = "source_type", IsNullable = true, Length = 20, ColumnDescription = "来源单据类型")]
		public string SourceType { get; set; }

		/// <summary>
		/// 来源单据ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "source_id", IsNullable = true, ColumnDescription = "来源单据id")]
		public long? SourceId { get; set; }

		/// <summary>
		/// 入库日期
		/// </summary>
		[SugarColumn(ColumnName = "receipt_date", IsNullable = false, ColumnDescription = "入库日期")]
		public DateTime? ReceiptDate { get; set; }

		/// <summary>
		/// 入库总数量
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "总数量")]
		public int? TotalQuantity { get; set; }

		/// <summary>
		/// 操作人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "operator_id", IsNullable = true, ColumnDescription = "作业人员id")]
		public long? OperatorId { get; set; }

		/// <summary>
		/// 入库单状态（字典：待入库/已入库/已完成）
		/// </summary>
		[SugarColumn(ColumnName = "receipt_status", IsNullable = true, Length = 20, ColumnDescription = "入库状态 pending_in-待入库 in_stock-已入库 completed-已完成")]
		public string ReceiptStatus { get; set; }

		/// <summary>
		/// 审核状态（字典：待审核/已通过/已驳回）
		/// </summary>
		[SugarColumn(ColumnName = "approve_status", IsNullable = true, Length = 20, ColumnDescription = "审核状态 0未审核 1已审核")]
		public string ApproveStatus { get; set; }

		/// <summary>
		/// 审核人ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "approve_id", IsNullable = true, ColumnDescription = "审核人id")]
		public long? ApproveId { get; set; }

		/// <summary>
		/// 审核时间
		/// </summary>
		[SugarColumn(ColumnName = "approve_time", IsNullable = true, ColumnDescription = "审核时间")]
		public DateTime? ApproveTime { get; set; }

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
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

		/// <summary>
		/// 入库明细列表
		/// </summary>
		[Navigate(NavigateType.OneToMany, nameof(WmsReceiptDetailEntity.ReceiptId), nameof(ReceiptId))]
		public List<WmsReceiptDetailEntity> Details { get; set; }

	}
}
