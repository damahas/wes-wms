using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_issue_order_detail", "领料单明细")]
	public class WmsIssueOrderDetailEntity
	{
		/// <summary>
		/// 明细id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "detail_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "明细id")]
		public long DetailId { get; set; }

		/// <summary>
		/// 领料单id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "issue_id", IsNullable = false, ColumnDescription = "领料单id")]
		public long IssueId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(IssueId), nameof(WmsIssueOrderEntity.IssueId))]
		public WmsIssueOrderEntity IssueOrder { get; set; }

		/// <summary>
		/// 物料id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

		/// <summary>
		/// 物料编码（冗余）
		/// </summary>
		[SugarColumn(ColumnName = "material_code", IsNullable = true, Length = 50, ColumnDescription = "物料编码")]
		public string MaterialCode { get; set; }

		/// <summary>
		/// 物料名称（冗余）
		/// </summary>
		[SugarColumn(ColumnName = "material_name", IsNullable = true, Length = 100, ColumnDescription = "物料名称")]
		public string MaterialName { get; set; }

		/// <summary>
		/// 规格
		/// </summary>
		[SugarColumn(ColumnName = "spec", IsNullable = true, Length = 100, ColumnDescription = "规格")]
		public string Spec { get; set; }

		/// <summary>
		/// 计量单位
		/// </summary>
		[SugarColumn(ColumnName = "unit", IsNullable = true, Length = 20, ColumnDescription = "单位")]
		public string Unit { get; set; }

		/// <summary>
		/// 指定批次号（为空则按 FEFO/FIFO 自动分配）
		/// </summary>
		[SugarColumn(ColumnName = "batch_no", IsNullable = true, Length = 50, ColumnDescription = "指定批次号")]
		public string BatchNo { get; set; }

		/// <summary>
		/// 需求数量
		/// </summary>
		[SugarColumn(ColumnName = "require_quantity", IsNullable = true, ColumnDescription = "需求数量")]
		public int? RequireQuantity { get; set; }

		/// <summary>
		/// 已发数量
		/// </summary>
		[SugarColumn(ColumnName = "issued_quantity", IsNullable = true, ColumnDescription = "已发数量")]
		public int? IssuedQuantity { get; set; }

		/// <summary>
		/// 欠料数量（需求 - 已发）
		/// </summary>
		[SugarColumn(ColumnName = "shortage_quantity", IsNullable = true, ColumnDescription = "欠料数量")]
		public int? ShortageQuantity { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

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
