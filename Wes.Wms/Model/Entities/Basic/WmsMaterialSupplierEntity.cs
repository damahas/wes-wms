using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_material_supplier", "物料供应商关系表")]
	public class WmsMaterialSupplierEntity
	{
		/// <summary>
		/// 关系主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_supplier_id", IsPrimaryKey = true, IsNullable = false)]
		public long MaterialSupplierId { get; set; }

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(MaterialId), nameof(WmsMaterialEntity.MaterialId))]
		public WmsMaterialEntity Material { get; set; }

		/// <summary>
		/// 供应商ID（往来单位，partner_type=供应商）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "supplier_id", IsNullable = false, ColumnDescription = "供应商id")]
		public long SupplierId { get; set; }

		/// <summary>
		/// 最小起订量
		/// </summary>
		[SugarColumn(ColumnName = "moq", IsNullable = true, ColumnDescription = "最小起订量")]
		public decimal? Moq { get; set; }

		/// <summary>
		/// 交货周期(天)
		/// </summary>
		[SugarColumn(ColumnName = "lead_time", IsNullable = true, ColumnDescription = "交货周期")]
		public int? LeadTime { get; set; }
		/// <summary>
		/// 质检模式 exempt-免检 sampling-抽检 all-全检
		/// </summary>
		[SugarColumn(ColumnName = "inspection_mode", IsNullable = true, Length = 20, ColumnDescription = "质检模式 exempt-免检 sampling-抽检 all-全检")]
		public string InspectionMode { get; set; }
		/// <summary>
		/// AQL标准级别
		/// </summary>
		[SugarColumn(ColumnName = "aql_level", IsNullable = true, Length = 20, ColumnDescription = "AQL标准级别")]
		public string AqlLevel { get; set; }
		/// <summary>
		/// 抽检比例(%)
		/// </summary>
		[SugarColumn(ColumnName = "sample_ratio", IsNullable = true, ColumnDescription = "抽检比例%")]
		public decimal? SampleRatio { get; set; }
		/// <summary>
		/// 严重缺陷数量上限
		/// </summary>
		[SugarColumn(ColumnName = "critical_qty", IsNullable = true, ColumnDescription = "严重缺陷数量上限")]
		public int? CriticalQty { get; set; }
		/// <summary>
		/// 主要缺陷数量上限
		/// </summary>
		[SugarColumn(ColumnName = "major_qty", IsNullable = true, ColumnDescription = "主要缺陷数量上限")]
		public int? MajorQty { get; set; }
		/// <summary>
		/// 次要缺陷数量上限
		/// </summary>
		[SugarColumn(ColumnName = "minor_qty", IsNullable = true, ColumnDescription = "次要缺陷数量上限")]
		public int? MinorQty { get; set; }

		/// <summary>
		/// 是否默认供应商（0=否 1=是）
		/// </summary>
		[SugarColumn(ColumnName = "is_default", IsNullable = true, Length = 1, ColumnDescription = "是否默认")]
		public string IsDefault { get; set; }

		/// <summary>
		/// 状态（0=启用 1=停用）
		/// </summary>
		[SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "状态")]
		public string Status { get; set; }

		/// <summary>
		/// 逻辑删除标记（0=否 1=是）
		/// </summary>
		[SugarColumn(ColumnName = "is_del", DefaultValue = "0", IsNullable = true, ColumnDescription = "逻辑删除标记")]
		public int IsDel { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		[SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建人")]
		public string CreateBy { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
		public DateTime? CreateTime { get; set; }

		/// <summary>
		/// 更新人
		/// </summary>
		[SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64, ColumnDescription = "更新人")]
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
		/// 供应商信息（导航，由 supplier_id 关联 wms_partner）
		/// </summary>
		[Navigate(NavigateType.OneToOne, nameof(SupplierId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Supplier { get; set; }
	}
}
