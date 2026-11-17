using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_region", "省市区")]
	public class WmsRegionEntity
	{
		/// <summary>
		/// 行政区划编码（如 440300）
		/// </summary>
		[SugarColumn(ColumnName = "code", IsPrimaryKey = true, IsNullable = false, Length = 20, ColumnDescription = "行政区划编码")]
		public string Code { get; set; }

		/// <summary>
		/// 行政区划名称（省/市/区）
		/// </summary>
		[SugarColumn(ColumnName = "name", IsNullable = false, Length = 100, ColumnDescription = "名称")]
		public string Name { get; set; }

		/// <summary>
		/// 父级行政区划编码（省→市→区 三级联动）
		/// </summary>
		[SugarColumn(ColumnName = "parent_code", IsNullable = true, Length = 20, ColumnDescription = "父级行政区划编码")]
		public string ParentCode { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[SugarColumn(ColumnName = "update_date", IsNullable = true, ColumnDescription = "更新日期")]
		public DateTime? UpdateDate { get; set; }

	}
}
