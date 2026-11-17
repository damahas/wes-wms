using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_issue_order", "出库申请单")]
	public class WmsIssueOrderEntity
	{
		/// <summary>
		/// 出库申请单id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "issue_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "出库申请单id")]
		public long IssueId { get; set; }

		/// <summary>
		/// 出库申请单号
		/// </summary>
		[SugarColumn(ColumnName = "issue_no", IsNullable = true, Length = 50, ColumnDescription = "出库申请单号")]
		public string IssueNo { get; set; }

		/// <summary>
		/// 出库类型：issue-生产领料 sales-销售发货 other-其他
		/// </summary>
		[SugarColumn(ColumnName = "outbound_type", IsNullable = true, Length = 20, ColumnDescription = "出库类型 issue-生产领料 sales-销售发货 other-其他")]
		public string OutboundType { get; set; }

		/// <summary>
		/// 客户名称（销售发货时填写）
		/// </summary>
		[SugarColumn(ColumnName = "customer_name", IsNullable = true, Length = 100, ColumnDescription = "客户名称")]
		public string CustomerName { get; set; }

		/// <summary>
		/// 来源类型：mes-MES下发 manual-手工新建
		/// </summary>
		[SugarColumn(ColumnName = "source_type", IsNullable = true, Length = 20, ColumnDescription = "来源类型 mes-MES下发 manual-手工新建")]
		public string SourceType { get; set; }

		/// <summary>
		/// 来源单据号（MES 下发时带入）
		/// </summary>
		[SugarColumn(ColumnName = "source_no", IsNullable = true, Length = 50, ColumnDescription = "来源单据号")]
		public string SourceNo { get; set; }

		/// <summary>
		/// 生产工单号
		/// </summary>
		[SugarColumn(ColumnName = "work_order_no", IsNullable = true, Length = 50, ColumnDescription = "生产工单号")]
		public string WorkOrderNo { get; set; }

		/// <summary>
		/// 领用部门 / 车间
		/// </summary>
		[SugarColumn(ColumnName = "dept_name", IsNullable = true, Length = 100, ColumnDescription = "领用部门/车间")]
		public string DeptName { get; set; }

		/// <summary>
		/// 领用人id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receiver_id", IsNullable = true, ColumnDescription = "领用人id")]
		public long? ReceiverId { get; set; }

		/// <summary>
		/// 领用人
		/// </summary>
		[SugarColumn(ColumnName = "receiver_name", IsNullable = true, Length = 50, ColumnDescription = "领用人")]
		public string ReceiverName { get; set; }

		/// <summary>
		/// 仓库id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = true, ColumnDescription = "仓库id")]
		public long? WarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(WarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity Warehouse { get; set; }

		/// <summary>
		/// 仓库名称（冗余）
		/// </summary>
		[SugarColumn(ColumnName = "warehouse_name", IsNullable = true, Length = 100, ColumnDescription = "仓库名称")]
		public string WarehouseName { get; set; }

		/// <summary>
		/// 需求日期
		/// </summary>
		[SugarColumn(ColumnName = "require_date", IsNullable = true, ColumnDescription = "需求日期")]
		public DateTime? RequireDate { get; set; }

		/// <summary>
		/// 需求总数量（明细汇总）
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "需求总数量")]
		public int? TotalQuantity { get; set; }

		/// <summary>
		/// 已发总数量
		/// </summary>
		[SugarColumn(ColumnName = "issued_quantity", IsNullable = true, ColumnDescription = "已发总数量")]
		public int? IssuedQuantity { get; set; }

		/// <summary>
		/// 欠料总数量（需求 - 已发）
		/// </summary>
		[SugarColumn(ColumnName = "shortage_quantity", IsNullable = true, ColumnDescription = "欠料总数量")]
		public int? ShortageQuantity { get; set; }

		/// <summary>
		/// 发料状态 pending-待发料 partial-部分发料 issued-已发完 cancelled-已取消
		/// </summary>
		[SugarColumn(ColumnName = "issue_status", IsNullable = true, Length = 20, ColumnDescription = "发料状态 pending-待发料 partial-部分发料 issued-已发完 cancelled-已取消")]
		public string IssueStatus { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

		/// <summary>
		/// 领料明细（非数据库字段，由 Biz/Service 组装）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public List<WmsIssueOrderDetailEntity> Details { get; set; }

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
