using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_transfer", "调拨单表")]
	public class WmsTransferEntity
	{
		/// <summary>
		/// 调拨单主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "transfer_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "调拨id")]
		public long TransferId { get; set; }

		/// <summary>
		/// 调拨单号
		/// </summary>
		[SugarColumn(ColumnName = "transfer_no", IsNullable = false, Length = 50, ColumnDescription = "调拨单号")]
		public string TransferNo { get; set; }

		/// <summary>
		/// 调出仓库ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "from_warehouse_id", IsNullable = false, ColumnDescription = "调出仓库id")]
		public long FromWarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(FromWarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity FromWarehouse { get; set; }

		/// <summary>
		/// 调入仓库ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "to_warehouse_id", IsNullable = false, ColumnDescription = "调入仓库id")]
		public long ToWarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ToWarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity ToWarehouse { get; set; }

		/// <summary>
		/// 调拨类型
		/// </summary>
		[SugarColumn(ColumnName = "transfer_type", IsNullable = true, Length = 20, ColumnDescription = "调拨类型 normal-普通调拨 urgent-紧急调拨")]
		public string TransferType { get; set; }

		/// <summary>
		/// 调拨原因
		/// </summary>
		[SugarColumn(ColumnName = "transfer_reason", IsNullable = true, Length = 255, ColumnDescription = "调拨原因")]
		public string TransferReason { get; set; }

		/// <summary>
		/// 调拨总数量
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "调拨总数量")]
		public decimal? TotalQuantity { get; set; }

		/// <summary>已出库数量（多次移出累加）</summary>
		[SugarColumn(ColumnName = "out_quantity", IsNullable = true, ColumnDescription = "已出库数量")]
		public decimal? OutQuantity { get; set; }

		/// <summary>已入库数量（多次移入累加）</summary>
		[SugarColumn(ColumnName = "in_quantity", IsNullable = true, ColumnDescription = "已入库数量")]
		public decimal? InQuantity { get; set; }

		/// <summary>
		/// 在途数量（已移出未移入，允许少收时差额会继续挂在这里）
		/// </summary>
		[SugarColumn(ColumnName = "in_transit_quantity", IsNullable = true, ColumnDescription = "在途数量")]
		public decimal? InTransitQuantity { get; set; }

		/// <summary>
		/// 调拨状态
		/// </summary>
		[SugarColumn(ColumnName = "transfer_status", IsNullable = true, Length = 20, ColumnDescription = "调拨状态 pending-待调拨 in_transit-在途 completed-已完成 cancelled-已取消")]
		public string TransferStatus { get; set; }

		/// <summary>移出人（调出仓责任人）</summary>
		[SugarColumn(ColumnName = "out_by", IsNullable = true, Length = 64, ColumnDescription = "移出人")]
		public string OutBy { get; set; }

		/// <summary>移出时间</summary>
		[SugarColumn(ColumnName = "out_time", IsNullable = true, ColumnDescription = "移出时间")]
		public DateTime? OutTime { get; set; }

		/// <summary>移入人（调入仓责任人）</summary>
		[SugarColumn(ColumnName = "in_by", IsNullable = true, Length = 64, ColumnDescription = "移入人")]
		public string InBy { get; set; }

		/// <summary>移入时间</summary>
		[SugarColumn(ColumnName = "in_time", IsNullable = true, ColumnDescription = "移入时间")]
		public DateTime? InTime { get; set; }

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
		/// 调拨明细列表
		/// </summary>
		[Navigate(NavigateType.OneToMany, nameof(WmsTransferDetailEntity.TransferId), nameof(TransferId))]
		public List<WmsTransferDetailEntity> Details { get; set; }

	}
}
