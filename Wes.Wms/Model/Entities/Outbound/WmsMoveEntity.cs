using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_move", "移库单表")]
	public class WmsMoveEntity
	{
		/// <summary>
		/// 移库单主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "move_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "移库id")]
		public long MoveId { get; set; }

		/// <summary>
		/// 移库单号
		/// </summary>
		[SugarColumn(ColumnName = "move_no", IsNullable = false, Length = 50, ColumnDescription = "移库单号")]
		public string MoveNo { get; set; }

		/// <summary>
		/// 仓库ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = false, ColumnDescription = "仓库id")]
		public long WarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(WarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity Warehouse { get; set; }

		/// <summary>
		/// 移库类型（字典：库位间/库区间/仓间）
		/// </summary>
		[SugarColumn(ColumnName = "move_type", IsNullable = true, Length = 20, ColumnDescription = "移库类型 adjust-库位调整 optimize-库位优化 zone_change-库区转移 sort-归类整理 split-拆层 merge-合并")]
		public string MoveType { get; set; }

		/// <summary>
		/// 移库原因
		/// </summary>
		[SugarColumn(ColumnName = "move_reason", IsNullable = true, Length = 255, ColumnDescription = "移库原因")]
		public string MoveReason { get; set; }

		/// <summary>
		/// 移库总数量
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "移库总数量")]
		public int? TotalQuantity { get; set; }

		/// <summary>
		/// 移库单状态（字典：待移库/移库中/已完成/已作废）
		/// </summary>
		[SugarColumn(ColumnName = "move_status", IsNullable = true, Length = 20, ColumnDescription = "移库状态 pending-待审核 approved-已审核 doing-移库中 completed-已完成 cancelled-已取消")]
		public string MoveStatus { get; set; }

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
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

		/// <summary>
		/// 移库明细列表
		/// </summary>
		[Navigate(NavigateType.OneToMany, nameof(WmsMoveDetailEntity.MoveId), nameof(MoveId))]
		public List<WmsMoveDetailEntity> Details { get; set; }

	}
}
