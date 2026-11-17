using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_receiving_detail", "收货单明细表")]
	public class WmsReceivingDetailEntity
	{
		/// <summary>
		/// 明细id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "detail_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "明细id")]
		public long DetailId { get; set; }
		/// <summary>
		/// 收货单id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receiving_id", IsNullable = false, ColumnDescription = "收货单id")]
		public long ReceivingId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ReceivingId), nameof(WmsReceivingEntity.ReceivingId))]
		public WmsReceivingEntity Receiving { get; set; }
		/// <summary>
		/// 物料id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }
		/// <summary>
		/// 规格
		/// </summary>
		[SugarColumn(ColumnName = "spec", IsNullable = true, Length = 100, ColumnDescription = "规格")]
		public string Spec { get; set; }
		/// <summary>
		/// 单位
		/// </summary>
		[SugarColumn(ColumnName = "unit", IsNullable = true, Length = 20, ColumnDescription = "单位")]
		public string Unit { get; set; }
		/// <summary>
		/// 批次号
		/// </summary>
		[SugarColumn(ColumnName = "batch_no", IsNullable = true, Length = 50, ColumnDescription = "批次号")]
		public string BatchNo { get; set; }
		/// <summary>
		/// 收货数量
		/// </summary>
		[SugarColumn(ColumnName = "plan_quantity", IsNullable = false, ColumnDescription = "收货数量")]
		public int PlanQuantity { get; set; }
		/// <summary>
		/// 入库数
		/// </summary>
		[SugarColumn(ColumnName = "stock_in_quantity", IsNullable = true, ColumnDescription = "入库数")]
		public int? StockInQuantity { get; set; }
		/// <summary>
		/// 待入库数量（已生成入库单但未确认入库）
		/// </summary>
		[SugarColumn(ColumnName = "pending_inbound_qty", IsNullable = true, ColumnDescription = "待入库数量")]
		public int? PendingInboundQty { get; set; }
		/// <summary>
		/// 退货数量
		/// </summary>
		[SugarColumn(ColumnName = "return_qty", DefaultValue = "0", IsNullable = true, ColumnDescription = "退货数量")]
		public int? ReturnQty { get; set; }
		/// <summary>
		/// 删除标志(0存在 1删除)
		/// </summary>
		[SugarColumn(ColumnName = "is_del", DefaultValue = "0", IsNullable = true, ColumnDescription = "删除标志")]
		public int IsDel { get; set; }
		/// <summary>
		/// 创建者
		/// </summary>
		[SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建者")]
		public string CreateBy { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
		public DateTime? CreateTime { get; set; }
		/// <summary>
		/// 更新者
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
