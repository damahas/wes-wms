using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_pack_spec", "包装规格表")]
	public class WmsPackSpecEntity
	{
		/// <summary>
		/// 包装规格层级ID（雪花主键）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "spec_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "规格id")]
		public long SpecId { get; set; }

		/// <summary>
		/// 所属包装规格根节点ID（即最顶层层级的 spec_id）。
		/// 同一套包装（托盘→箱→盒）的所有层级共享同一个 root_id，用于归组整棵层级树。
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "root_id", IsNullable = true, ColumnDescription = "根节点ID")]
		public long? RootId { get; set; }

		/// <summary>
		/// 上级层级ID（自关联，指向同表 spec_id）。最顶层为 NULL。
		/// 用于构建层级树：子层的 parent_id 指向父层的 spec_id。
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "parent_id", IsNullable = true, ColumnDescription = "父级ID")]
		public long? ParentId { get; set; }

		/// <summary>
		/// 层级名称（如 托盘/箱/盒/卷/米），人类可读标识，替代原 level_code。
		/// </summary>
		[SugarColumn(ColumnName = "level_name", IsNullable = false, Length = 50, ColumnDescription = "层级名称")]
		public string LevelName { get; set; }

		/// <summary>
		/// 本层级计量单位（如 托盘/箱/个/卷/米/千克）。
		/// </summary>
		[SugarColumn(ColumnName = "uom", IsNullable = true, Length = 20, ColumnDescription = "计量单位")]
		public string Uom { get; set; }

		/// <summary>
		/// 上层包含本层的数量：1 个上级层级含有几个本级（如箱 qty_per_parent=10 表示 1 托盘含 10 箱）。
		/// 用于层级间数量换算，沿树路径相乘得到基本单位数量。
		/// </summary>
		[SugarColumn(ColumnName = "qty_per_parent", IsNullable = true, ColumnDescription = "上级层级包含的本层级数量")]
		public decimal? QtyPerParent { get; set; }

		/// <summary>
		/// 是否连续量：1=连续量（米/千克，可为小数）；0=离散量（盒/箱/托盘，整数）。
		/// 决定本层级数量是否允许小数。
		/// </summary>
		[SugarColumn(ColumnName = "is_continuous", IsNullable = true, ColumnDescription = "是否连续量")]
		public int? IsContinuous { get; set; }

		/// <summary>
		/// 是否基本单位：1=基本单位（库存记账单位，每套包装唯一）；0=非基本单位。
		/// </summary>
		[SugarColumn(ColumnName = "is_base", IsNullable = true, ColumnDescription = "是否最小基本单位")]
		public int? IsBase { get; set; }

		/// <summary>
		/// 层级排序（同父下展示顺序）。
		/// </summary>
		[SugarColumn(ColumnName = "sort_order", IsNullable = true, ColumnDescription = "层级排序")]
		public int? SortOrder { get; set; }

		/// <summary>
		/// 生效日期（支持包装规格版本变更：同一物料不同时间用不同包装）。
		/// </summary>
		[SugarColumn(ColumnName = "effective_date", IsNullable = true, ColumnDescription = "生效日期")]
		public DateTime? EffectiveDate { get; set; }

		/// <summary>
		/// 逻辑删除：0=未删，1=已删。
		/// </summary>
		[SugarColumn(ColumnName = "is_del", DefaultValue = "0", IsNullable = true, ColumnDescription = "删除标志")]
		public int IsDel { get; set; }

		/// <summary>
		/// 创建人账号。
		/// </summary>
		[SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建者")]
		public string CreateBy { get; set; }

		/// <summary>
		/// 创建时间。
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
		public DateTime? CreateTime { get; set; }

		/// <summary>
		/// 更新人账号。
		/// </summary>
		[SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64, ColumnDescription = "更新者")]
		public string UpdateBy { get; set; }

		/// <summary>
		/// 更新时间。
		/// </summary>
		[SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
		public DateTime? UpdateTime { get; set; }
	}
}
