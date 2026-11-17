using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_iqc_order", "检验单")]
	public class WmsIqcOrderEntity
	{
		/// <summary>
		/// 检验单id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "inspection_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "检验单id")]
		public long InspectionId { get; set; }

		/// <summary>
		/// 检验单编号
		/// </summary>
		[SugarColumn(ColumnName = "inspection_no", IsNullable = true, Length = 50, ColumnDescription = "检验单编号")]
		public string InspectionNo { get; set; }

		/// <summary>
		/// 收货单id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receiving_id", IsNullable = false, ColumnDescription = "收货单id")]
		public long ReceivingId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ReceivingId), nameof(WmsReceivingEntity.ReceivingId))]
		public WmsReceivingEntity Receiving { get; set; }

		/// <summary>
		/// 收货单明细id（一个检验单对应一条收货单明细）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receiving_detail_id", IsNullable = false, ColumnDescription = "收货单明细id")]
		public long ReceivingDetailId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ReceivingDetailId), nameof(WmsReceivingDetailEntity.DetailId))]
		public WmsReceivingDetailEntity ReceivingDetail { get; set; }

		/// <summary>
		/// 供应商id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "supplier_id", IsNullable = true, ColumnDescription = "供应商id")]
		public long? SupplierId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(SupplierId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Supplier { get; set; }

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
		[SugarColumn(ColumnName = "delivery_quantity", IsNullable = true, ColumnDescription = "收货数量")]
		public int? DeliveryQuantity { get; set; }

		/// <summary>
		/// 允许入库数量（合格数量）
		/// </summary>
		[SugarColumn(ColumnName = "allow_quantity", DefaultValue = "0", IsNullable = true, ColumnDescription = "允许入库数量")]
		public int? AllowQuantity { get; set; }

		/// <summary>
		/// 不良数量（收货数量 - 允许入库数）
		/// </summary>
		[SugarColumn(ColumnName = "defect_quantity", DefaultValue = "0", IsNullable = true, ColumnDescription = "不良数量")]
		public int? DefectQuantity { get; set; }

		/// <summary>
		/// 退货数量（仅处置方式为退回供应商时等于不良数量，其余处置方式为0）
		/// </summary>
		[SugarColumn(ColumnName = "return_quantity", DefaultValue = "0", IsNullable = true, ColumnDescription = "退货数量")]
		public int? ReturnQuantity { get; set; }

		/// <summary>
		/// 不良品处置方式 return-退回供应商 concession-让步接收 sorting-挑选使用
		/// </summary>
		[SugarColumn(ColumnName = "dispose_type", DefaultValue = "return", IsNullable = true, Length = 20, ColumnDescription = "不良品处置方式 return-退回供应商 concession-让步接收 sorting-挑选使用")]
		public string DisposeType { get; set; }

		/// <summary>
		/// 状态 pending-待检验 inspecting-检验中 completed-已完成
		/// </summary>
		[SugarColumn(ColumnName = "inspect_status", IsNullable = true, Length = 20, ColumnDescription = "状态 pending-待检验 inspecting-检验中 completed-已完成")]
		public string InspectStatus { get; set; }

		/// <summary>
		/// 检验日期
		/// </summary>
		[SugarColumn(ColumnName = "inspect_date", IsNullable = true, ColumnDescription = "检验日期")]
		public DateTime? InspectDate { get; set; }

		/// <summary>
		/// 检验人
		/// </summary>
		[SugarColumn(ColumnName = "inspector", IsNullable = true, Length = 50, ColumnDescription = "检验人")]
		public string Inspector { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

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
