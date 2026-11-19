using System.Collections.Generic;
using System.Text.Json.Serialization;
using Wes.Utils.Converter;

namespace Wes.Model
{
	/// <summary>
	/// 仓库平面布局（库区网格 + 库位坐标 + 库存占用）
	/// </summary>
	public class WarehouseLayoutDto
	{
		/// <summary>仓库id</summary>
		[JsonConverter(typeof(LongToStringConverter))]
		public long WarehouseId { get; set; }
		/// <summary>仓库编码</summary>
		public string WarehouseCode { get; set; }
		/// <summary>仓库名称</summary>
		public string WarehouseName { get; set; }
		/// <summary>库区列表</summary>
		public List<ZoneLayoutDto> Zones { get; set; } = new List<ZoneLayoutDto>();
	}

	/// <summary>
	/// 库区布局（网格尺寸由库位实际坐标计算，shelf_rows/cols 仅作排布参考）
	/// </summary>
	public class ZoneLayoutDto
	{
		/// <summary>库区id</summary>
		[JsonConverter(typeof(LongToStringConverter))]
		public long ZoneId { get; set; }
		/// <summary>库区编码</summary>
		public string ZoneCode { get; set; }
		/// <summary>库区名称</summary>
		public string ZoneName { get; set; }
		/// <summary>库区类型 receive-收货区 storage-存储区 picking-拣货区 ship-发货区 return-退货区 temp-暂存区 reject-不合格品区</summary>
		public string ZoneType { get; set; }
		/// <summary>库区属性 normal-常温 cold-低温 freezer-冷冻 damp-防潮 dangerous-危险品</summary>
		public string ZoneAttr { get; set; }
		/// <summary>库区状态(0正常 1停用 2维护中)</summary>
		public string ZoneStatus { get; set; }
		/// <summary>网格行数（库位最大行与货架排数取大）</summary>
		public int GridRows { get; set; }
		/// <summary>网格列数（库位最大列与货架列数取大）</summary>
		public int GridCols { get; set; }
		/// <summary>货架外框列跨度JSON：{"排号":列数}，空则按库位自动计算</summary>
		public string RowFrames { get; set; }
		/// <summary>库位列表（含无坐标的未排布库位）</summary>
		public List<LocationGridItem> Locations { get; set; } = new List<LocationGridItem>();
	}

	/// <summary>
	/// 库位网格项（着色数据从库存实时聚合，不用 location_status）
	/// </summary>
	public class LocationGridItem
	{
		/// <summary>库位id</summary>
		[JsonConverter(typeof(LongToStringConverter))]
		public long LocationId { get; set; }
		/// <summary>库位编码</summary>
		public string LocationCode { get; set; }
		/// <summary>库位名称</summary>
		public string LocationName { get; set; }
		/// <summary>库位类型 storage-存储区 picking-拣货区 temp-暂存区 return-退货区</summary>
		public string LocationType { get; set; }
		/// <summary>库位状态(0空闲 1占用 2部分占用 3锁定 4禁用)</summary>
		public string LocationStatus { get; set; }
		/// <summary>布局行号</summary>
		public int? GridRow { get; set; }
		/// <summary>布局列号</summary>
		public int? GridCol { get; set; }
		/// <summary>布局结束行号（矩形区域，空=单格）</summary>
		public int? GridEndRow { get; set; }
		/// <summary>布局结束列号（矩形区域，空=单格）</summary>
		public int? GridEndCol { get; set; }
		/// <summary>库存数量合计</summary>
		public decimal StockQuantity { get; set; }
		/// <summary>冻结数量合计</summary>
		public decimal FreezeQuantity { get; set; }
		/// <summary>锁定数量合计</summary>
		public decimal LockQuantity { get; set; }
		/// <summary>物料种类数</summary>
		public int MaterialCount { get; set; }
	}

	/// <summary>
	/// 保存布局参数（只提交变更项）
	/// </summary>
	public class SaveLayoutParam
	{
		[JsonConverter(typeof(LongToStringConverter))]
		public long ZoneId { get; set; }
		/// <summary>货架外框列跨度JSON：{"排号":列数}</summary>
		public string RowFrames { get; set; }
		/// <summary>库位坐标变更项</summary>
		public List<LocationGridParam> Items { get; set; } = new List<LocationGridParam>();
	}

	/// <summary>
	/// 库位坐标项
	/// </summary>
	public class LocationGridParam
	{
		[JsonConverter(typeof(LongToStringConverter))]
		public long LocationId { get; set; }
		/// <summary>布局行号</summary>
		public int? GridRow { get; set; }
		/// <summary>布局列号</summary>
		public int? GridCol { get; set; }
		/// <summary>布局结束行号（矩形区域，空=单格）</summary>
		public int? GridEndRow { get; set; }
		/// <summary>布局结束列号（矩形区域，空=单格）</summary>
		public int? GridEndCol { get; set; }
	}

	/// <summary>
	/// 一键排布参数
	/// </summary>
	public class AutoLayoutParam
	{
		[JsonConverter(typeof(LongToStringConverter))]
		public long WarehouseId { get; set; }
		[JsonConverter(typeof(LongToStringConverter))]
		public long ZoneId { get; set; }
		/// <summary>排布模式 rect-矩形顺序填充 shelf-按货架分排</summary>
		public string Mode { get; set; }
	}

	/// <summary>
	/// 库位库存占用聚合（平面布局着色用）
	/// </summary>
	public class LocationStockSummaryDto
	{
		[JsonConverter(typeof(LongToStringConverter))]
		public long LocationId { get; set; }
		/// <summary>库存数量合计</summary>
		public decimal StockQuantity { get; set; }
		/// <summary>冻结数量合计</summary>
		public decimal FreezeQuantity { get; set; }
		/// <summary>锁定数量合计</summary>
		public decimal LockQuantity { get; set; }
		/// <summary>物料种类数</summary>
		public int MaterialCount { get; set; }
	}
}
