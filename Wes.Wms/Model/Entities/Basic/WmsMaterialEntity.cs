using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Wes.Model
{
	[SugarTable("wms_material", "物料表")]
	public class WmsMaterialEntity
	{
		/// <summary>
		/// 物料id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "material_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "物料id")]
		public long MaterialId { get; set; }
		/// <summary>
		/// 物料编码
		/// </summary>
		[SugarColumn(ColumnName = "material_code", IsNullable = false, Length = 50, ColumnDescription = "物料编码")]
		public string MaterialCode { get; set; }
		/// <summary>
		/// 物料名称
		/// </summary>
		[SugarColumn(ColumnName = "material_name", IsNullable = false, Length = 100, ColumnDescription = "物料名称")]
		public string MaterialName { get; set; }
		/// <summary>
		/// 物料简称
		/// </summary>
		[SugarColumn(ColumnName = "material_short_name", IsNullable = true, Length = 50, ColumnDescription = "物料简称")]
		public string MaterialShortName { get; set; }
		/// <summary>
		/// 物料分类(字典 material_category, 存 dict_value)
		/// </summary>
		[SugarColumn(ColumnName = "category_id", IsNullable = true, Length = 50, ColumnDescription = "物料分类")]
		public string CategoryId { get; set; }
		/// <summary>
		/// 规格型号
		/// </summary>
		[SugarColumn(ColumnName = "spec", IsNullable = true, Length = 100, ColumnDescription = "规格型号")]
		public string Spec { get; set; }
		/// <summary>
		/// 条码
		/// </summary>
		[SugarColumn(ColumnName = "barcode", IsNullable = true, Length = 100, ColumnDescription = "条码")]
		public string Barcode { get; set; }
		/// <summary>
		/// 计量单位
		/// </summary>
		[SugarColumn(ColumnName = "unit", IsNullable = true, Length = 20, ColumnDescription = "计量单位")]
		public string Unit { get; set; }
		/// <summary>
		/// 包装规格id（指向 wms_pack_spec.root_id，决定入库按什么层级拆条码）
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "pack_spec_id", IsNullable = true, ColumnDescription = "包装规格id")]
		public long? PackSpecId { get; set; }
		/// <summary>
		/// 物料类型(字典 material_type: material-原材料 semi-半成品 product-成品)
		/// </summary>
		[SugarColumn(ColumnName = "material_type", IsNullable = true, Length = 20, ColumnDescription = "物料类型")]
		public string MaterialType { get; set; }
		/// <summary>
		/// 默认仓库id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "default_warehouse_id", IsNullable = true, ColumnDescription = "默认仓库id")]
		public long? DefaultWarehouseId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(DefaultWarehouseId), nameof(WmsWarehouseEntity.WarehouseId))]
		public WmsWarehouseEntity DefaultWarehouse { get; set; }
		/// <summary>
		/// 默认库区id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "default_zone_id", IsNullable = true, ColumnDescription = "默认库区id")]
		public long? DefaultZoneId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(DefaultZoneId), nameof(WmsZoneEntity.ZoneId))]
		public WmsZoneEntity DefaultZone { get; set; }
		/// <summary>
		/// 默认库位id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "default_location_id", IsNullable = true, ColumnDescription = "默认库位id")]
		public long? DefaultLocationId { get; set; }

		[Navigate(NavigateType.OneToOne, nameof(DefaultLocationId), nameof(WmsLocationEntity.LocationId))]
		public WmsLocationEntity DefaultLocation { get; set; }
		/// <summary>
		/// 物料供应商列表（一物料多供应商）
		/// </summary>
		[Navigate(NavigateType.OneToMany, nameof(WmsMaterialSupplierEntity.MaterialId), nameof(MaterialId))]
		public List<WmsMaterialSupplierEntity> Suppliers { get; set; }
		/// <summary>
		/// 保质期天数
		/// </summary>
		[SugarColumn(ColumnName = "shelf_life", IsNullable = true, ColumnDescription = "保质期天数")]
		public int? ShelfLife { get; set; }
		/// <summary>
		/// 安全库存量
		/// </summary>
		[SugarColumn(ColumnName = "min_stock", IsNullable = true, ColumnDescription = "安全库存量")]
		public decimal? MinStock { get; set; }
		/// <summary>
		/// 最大库存量
		/// </summary>
		[SugarColumn(ColumnName = "max_stock", IsNullable = true, ColumnDescription = "最大库存量")]
		public decimal? MaxStock { get; set; }
		/// <summary>
		/// 采购周期(天)
		/// </summary>
		[SugarColumn(ColumnName = "purchase_cycle", IsNullable = true, ColumnDescription = "采购周期")]
		public int? PurchaseCycle { get; set; }
		/// <summary>
		/// 重量(千克)
		/// </summary>
		[SugarColumn(ColumnName = "weight", IsNullable = true, ColumnDescription = "重量")]
		public decimal? Weight { get; set; }
		/// <summary>
		/// 体积(立方米)
		/// </summary>
		[SugarColumn(ColumnName = "volume", IsNullable = true, ColumnDescription = "体积")]
		public decimal? Volume { get; set; }
		/// <summary>
		/// 物料图片
		/// </summary>
		[SugarColumn(ColumnName = "image_url", IsNullable = true, Length = 500, ColumnDescription = "物料图片")]
		public string ImageUrl { get; set; }
		/// <summary>
		/// 状态(0启用 1禁用)
		/// </summary>
		[SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "状态")]
		public string Status { get; set; }
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
