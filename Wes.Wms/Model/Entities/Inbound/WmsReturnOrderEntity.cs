using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_return_order", "退货单表")]
	public class WmsReturnOrderEntity
	{
		/// <summary>
		/// 退货单id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "return_order_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "退货单id")]
		public long ReturnOrderId { get; set; }
		/// <summary>
		/// 退货单号
		/// </summary>
		[SugarColumn(ColumnName = "return_order_no", IsNullable = false, Length = 50, ColumnDescription = "退货单号")]
		public string ReturnOrderNo { get; set; }
		/// <summary>
		/// 退货类型 iqc_unqualified-质检不合格 other-其他
		/// </summary>
		[SugarColumn(ColumnName = "return_type", IsNullable = true, Length = 20, ColumnDescription = "退货类型 iqc_unqualified-质检不合格 other-其他")]
		public string ReturnType { get; set; }
		/// <summary>
		/// 供应商id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "supplier_id", IsNullable = true, ColumnDescription = "供应商id")]
		public long? SupplierId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(SupplierId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Supplier { get; set; }
		/// <summary>
		/// 退货总数量
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "退货总数量")]
		public int? TotalQuantity { get; set; }
		/// <summary>
		/// 退货状态 pending-待处理 completed-已完成 cancelled-已取消
		/// </summary>
		[SugarColumn(ColumnName = "return_status", IsNullable = true, Length = 20, ColumnDescription = "退货状态 pending-待处理 completed-已完成 cancelled-已取消")]
		public string ReturnStatus { get; set; }
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
		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

	}
}
