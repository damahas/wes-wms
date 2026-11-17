using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_location", "库位表")]
	public class WmsLocationEntity
	{
		/// <summary>
		/// 库位id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "location_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "库位id")]
		public long LocationId { get; set; }
		/// <summary>
		/// 仓库id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsNullable = false, ColumnDescription = "仓库id")]
		public long WarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(WarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity Warehouse { get; set; }
		/// <summary>
		/// 库区id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "zone_id", IsNullable = false, ColumnDescription = "库区id")]
		public long ZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(ZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity Zone { get; set; }
		/// <summary>
		/// 库位编码
		/// </summary>
		[SugarColumn(ColumnName = "location_code", IsNullable = false, Length = 50, ColumnDescription = "库位编码")]
		public string LocationCode { get; set; }
		/// <summary>
		/// 库位名称
		/// </summary>
		[SugarColumn(ColumnName = "location_name", IsNullable = true, Length = 100, ColumnDescription = "库位名称")]
		public string LocationName { get; set; }
		/// <summary>
		/// 库位类型 storage-存储区 picking-拣货区 temp-暂存区 return-退货区
		/// </summary>
		[SugarColumn(ColumnName = "location_type", IsNullable = true, Length = 20, ColumnDescription = "库位类型 storage-存储区 picking-拣货区 temp-暂存区 return-退货区")]
		public string LocationType { get; set; }
		/// <summary>
		/// 库位属性 normal-常温 cold-低温 damp-防潮 dangerous-危险品
		/// </summary>
		[SugarColumn(ColumnName = "location_attr", IsNullable = true, Length = 50, ColumnDescription = "库位属性 normal-常温 cold-低温 damp-防潮 dangerous-危险品")]
		public string LocationAttr { get; set; }
		/// <summary>
		/// 货架编号
		/// </summary>
		[SugarColumn(ColumnName = "shelf_code", IsNullable = true, Length = 20, ColumnDescription = "货架编号")]
		public string ShelfCode { get; set; }
		/// <summary>
		/// 长度(米)
		/// </summary>
		[SugarColumn(ColumnName = "length", IsNullable = true, ColumnDescription = "长度")]
		public decimal? Length { get; set; }
		/// <summary>
		/// 宽度(米)
		/// </summary>
		[SugarColumn(ColumnName = "width", IsNullable = true, ColumnDescription = "宽度")]
		public decimal? Width { get; set; }
		/// <summary>
		/// 高度(米)
		/// </summary>
		[SugarColumn(ColumnName = "height", IsNullable = true, ColumnDescription = "高度")]
		public decimal? Height { get; set; }
		/// <summary>
		/// 最大承重(千克)
		/// </summary>
		[SugarColumn(ColumnName = "max_weight", IsNullable = true, ColumnDescription = "最大承重")]
		public decimal? MaxWeight { get; set; }
		/// <summary>
		/// 最大容量(立方米)
		/// </summary>
		[SugarColumn(ColumnName = "max_volume", IsNullable = true, ColumnDescription = "最大容量")]
		public decimal? MaxVolume { get; set; }
		/// <summary>
		/// 最大存储数量
		/// </summary>
		[SugarColumn(ColumnName = "max_quantity", IsNullable = true, ColumnDescription = "最大存储数量")]
		public int? MaxQuantity { get; set; }
		/// <summary>
		/// 库位状态(0空闲 1占用 2部分占用 3锁定 4禁用)
		/// </summary>
		[SugarColumn(ColumnName = "location_status", IsNullable = true, Length = 1, ColumnDescription = "库位状态")]
		public string LocationStatus { get; set; }
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
