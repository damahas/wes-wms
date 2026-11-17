using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_return_order_detail", "退货单明细表")]
	public class WmsReturnOrderDetailEntity
	{
		/// <summary>
		/// 明细id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "detail_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "明细id")]
		public long DetailId { get; set; }
		/// <summary>
		/// 退货单id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "return_order_id", IsNullable = false, ColumnDescription = "退货单id")]
		public long ReturnOrderId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ReturnOrderId), nameof(WmsReturnOrderEntity.ReturnOrderId))]
		public WmsReturnOrderEntity ReturnOrder { get; set; }
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
		/// 退货数量
		/// </summary>
		[SugarColumn(ColumnName = "plan_quantity", IsNullable = false, ColumnDescription = "退货数量")]
		public int PlanQuantity { get; set; }
		/// <summary>
		/// 退货原因
		/// </summary>
		[SugarColumn(ColumnName = "return_reason", IsNullable = true, Length = 255, ColumnDescription = "退货原因")]
		public string ReturnReason { get; set; }
		/// <summary>
		/// 默认库位id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "location_id", IsNullable = true, ColumnDescription = "默认库位id")]
		public long? LocationId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(LocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity Location { get; set; }
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
