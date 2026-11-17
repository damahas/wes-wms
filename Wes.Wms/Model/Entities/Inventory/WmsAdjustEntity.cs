using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_adjust", "库存调整单表")]
	public class WmsAdjustEntity
	{
		/// <summary>
		/// 调整单主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "adjust_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "调整id")]
		public long AdjustId { get; set; }

		/// <summary>
		/// 调整单号
		/// </summary>
		[SugarColumn(ColumnName = "adjust_no", IsNullable = false, Length = 50, ColumnDescription = "调整单号")]
		public string AdjustNo { get; set; }

		/// <summary>
		/// 调整类型（字典：盘盈/盘亏/报损/报溢等）
		/// </summary>
		[SugarColumn(ColumnName = "adjust_type", IsNullable = false, Length = 20, ColumnDescription = "调整类型 check_profit-盘盈 check_loss-盘亏 damage-报损 overflow-报溢 validity-有效期调整 other-其他")]
		public string AdjustType { get; set; }

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
		/// 来源类型（关联单据类型）
		/// </summary>
		[SugarColumn(ColumnName = "source_type", IsNullable = true, Length = 20, ColumnDescription = "来源类型 check-盘点 manual-人工")]
		public string SourceType { get; set; }

		/// <summary>
		/// 来源单据ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "source_id", IsNullable = true, ColumnDescription = "来源id")]
		public long? SourceId { get; set; }

		/// <summary>
		/// 调整总数量
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "调整总数量")]
		public int? TotalQuantity { get; set; }

		/// <summary>
		/// 调整原因
		/// </summary>
		[SugarColumn(ColumnName = "adjust_reason", IsNullable = true, Length = 255, ColumnDescription = "调整原因")]
		public string AdjustReason { get; set; }

		/// <summary>
		/// 调整单状态（字典：草稿/待审核/已审核/已作废）
		/// </summary>
		[SugarColumn(ColumnName = "adjust_status", IsNullable = true, Length = 20, ColumnDescription = "调整状态 pending-待审核 approved-已审核 completed-已完成")]
		public string AdjustStatus { get; set; }

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
		/// 调整明细列表
		/// </summary>
		[Navigate(NavigateType.OneToMany, nameof(WmsAdjustDetailEntity.AdjustId), nameof(AdjustId))]
		public List<WmsAdjustDetailEntity> Details { get; set; }

	}
}
