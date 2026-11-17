using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_zone", "库区表")]
	public class WmsZoneEntity
	{
		/// <summary>
		/// 库区id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "zone_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "库区id")]
		public long ZoneId { get; set; }
		/// <summary>
		/// 仓库id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = false, ColumnDescription = "仓库id")]
		public long WarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(WarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity Warehouse { get; set; }
		/// <summary>
		/// 库区编码(格式: 仓库编码-类型代码-序号，如WH01-STO-A)
		/// </summary>
		[SugarColumn(ColumnName = "zone_code", IsNullable = false, Length = 50, ColumnDescription = "库区编码")]
		public string ZoneCode { get; set; }
		/// <summary>
		/// 库区名称(例：存储A区、存储B区、冷藏一区)
		/// </summary>
		[SugarColumn(ColumnName = "zone_name", IsNullable = false, Length = 100, ColumnDescription = "库区名称")]
		public string ZoneName { get; set; }
		/// <summary>
		/// 库区类型 receive-收货区 storage-存储区 picking-拣货区 ship-发货区 return-退货区 temp-暂存区 reject-不合格品区
		/// </summary>
		[SugarColumn(ColumnName = "zone_type", IsNullable = true, Length = 20, ColumnDescription = "库区类型 receive-收货区 storage-存储区 picking-拣货区 ship-发货区 return-退货区 temp-暂存区 reject-不合格品区")]
		public string ZoneType { get; set; }
		/// <summary>
		/// 库区属性 normal-常温 cold-低温 freezer-冷冻 damp-防潮 dangerous-危险品
		/// </summary>
		[SugarColumn(ColumnName = "zone_attr", IsNullable = true, Length = 50, ColumnDescription = "库区属性 normal-常温 cold-低温 freezer-冷冻 damp-防潮 dangerous-危险品")]
		public string ZoneAttr { get; set; }
		/// <summary>
		/// 所在楼层
		/// </summary>
		[SugarColumn(ColumnName = "floor", IsNullable = true, ColumnDescription = "所在楼层")]
		public int? Floor { get; set; }
		/// <summary>
		/// 库区面积(平方米)
		/// </summary>
		[SugarColumn(ColumnName = "zone_area", IsNullable = true, ColumnDescription = "库区面积")]
		public decimal? ZoneArea { get; set; }
		/// <summary>
		/// 货架排数
		/// </summary>
		[SugarColumn(ColumnName = "shelf_rows", IsNullable = true, ColumnDescription = "货架排数")]
		public int? ShelfRows { get; set; }
		/// <summary>
		/// 货架列数
		/// </summary>
		[SugarColumn(ColumnName = "shelf_cols", IsNullable = true, ColumnDescription = "货架列数")]
		public int? ShelfCols { get; set; }
		/// <summary>
		/// 货架层数
		/// </summary>
		[SugarColumn(ColumnName = "shelf_levels", IsNullable = true, ColumnDescription = "货架层数")]
		public int? ShelfLevels { get; set; }
		/// <summary>
		/// 库区状态(0正常 1停用 2维护中)
		/// </summary>
		[SugarColumn(ColumnName = "zone_status", IsNullable = true, Length = 1, ColumnDescription = "库区状态")]
		public string ZoneStatus { get; set; }
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
