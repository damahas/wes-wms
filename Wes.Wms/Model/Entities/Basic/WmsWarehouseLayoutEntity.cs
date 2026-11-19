using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_warehouse_layout", "仓库平面布局表")]
	public class WmsWarehouseLayoutEntity
	{
		/// <summary>
		/// 布局id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "layout_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "布局id")]
		public long LayoutId { get; set; }
		/// <summary>
		/// 仓库id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = false, ColumnDescription = "仓库id")]
		public long WarehouseId { get; set; }
		/// <summary>
		/// 版本号(每次保存递增)
		/// </summary>
		[SugarColumn(ColumnName = "version", IsNullable = false, ColumnDescription = "版本号")]
		public int Version { get; set; }
		/// <summary>
		/// 状态(0草稿 1已发布)
		/// </summary>
		[SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "状态(0草稿 1已发布)")]
		public string Status { get; set; }
		/// <summary>
		/// 画布JSON(网格配置与块定义树)
		/// </summary>
		[SugarColumn(ColumnName = "canvas_json", ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "画布JSON")]
		public string CanvasJson { get; set; }
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
