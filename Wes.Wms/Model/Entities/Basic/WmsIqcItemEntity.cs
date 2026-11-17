using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_iqc_item", "检验项表")]
	public class WmsIqcItemEntity
	{
		/// <summary>
		/// 检验项id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "item_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "检验项id")]
		public long ItemId { get; set; }
		/// <summary>
		/// 物料分类(字典 material_category, 存 dict_value)
		/// </summary>
		[SugarColumn(ColumnName = "category_id", IsNullable = false, Length = 50, ColumnDescription = "物料分类")]
		public string CategoryId { get; set; }
		/// <summary>
		/// 检验项编码
		/// </summary>
		[SugarColumn(ColumnName = "item_code", IsNullable = true, Length = 50, ColumnDescription = "检验项编码")]
		public string ItemCode { get; set; }
		/// <summary>
		/// 检验项名称
		/// </summary>
		[SugarColumn(ColumnName = "item_name", IsNullable = false, Length = 100, ColumnDescription = "检验项名称")]
		public string ItemName { get; set; }
		/// <summary>
		/// 检验类型(计量/定性/目视)
		/// </summary>
		[SugarColumn(ColumnName = "inspection_type", IsNullable = true, Length = 30, ColumnDescription = "检验类型")]
		public string InspectionType { get; set; }
		/// <summary>
		/// 规格/标准值
		/// </summary>
		[SugarColumn(ColumnName = "standard_value", IsNullable = true, Length = 100, ColumnDescription = "规格/标准值")]
		public string StandardValue { get; set; }
		/// <summary>
		/// 单位
		/// </summary>
		[SugarColumn(ColumnName = "unit", IsNullable = true, Length = 20, ColumnDescription = "单位")]
		public string Unit { get; set; }
		/// <summary>
		/// 是否关键项(0否 1是)
		/// </summary>
		[SugarColumn(ColumnName = "is_critical", IsNullable = true, ColumnDescription = "是否关键项")]
		public int? IsCritical { get; set; }
		/// <summary>
		/// 检测方法/工具
		/// </summary>
		[SugarColumn(ColumnName = "test_method", IsNullable = true, Length = 200, ColumnDescription = "检测方法/工具")]
		public string TestMethod { get; set; }
		/// <summary>
		/// 排序号
		/// </summary>
		[SugarColumn(ColumnName = "sort_no", IsNullable = true, ColumnDescription = "排序号")]
		public int? SortNo { get; set; }
		/// <summary>
		/// 状态(0正常 1停用)
		/// </summary>
		[SugarColumn(ColumnName = "status", IsNullable = true, Length = 5, ColumnDescription = "状态")]
		public string Status { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }
		/// <summary>
		/// 软删除标志(0未删 1已删)
		/// </summary>
		[SugarColumn(ColumnName = "is_del", IsNullable = false, ColumnDescription = "软删除标志")]
		public int IsDel { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 50, ColumnDescription = "创建人")]
		public string CreateBy { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
		public DateTime? CreateTime { get; set; }
		/// <summary>
		/// 修改人
		/// </summary>
		[SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 50, ColumnDescription = "修改人")]
		public string UpdateBy { get; set; }
		/// <summary>
		/// 修改时间
		/// </summary>
		[SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "修改时间")]
		public DateTime? UpdateTime { get; set; }
	}
}
