using System;
using SqlSugar;
using Wes.Entity;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_warehouse", "仓库表")]
	public class WmsWarehouseEntity
	{
		/// <summary>
		/// 仓库id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "warehouse_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "仓库id")]
		public long WarehouseId { get; set; }
		/// <summary>
		/// 仓库编码
		/// </summary>
		[SugarColumn(ColumnName = "warehouse_code", IsNullable = false, Length = 50, ColumnDescription = "仓库编码")]
		public string WarehouseCode { get; set; }
		/// <summary>
		/// 仓库名称
		/// </summary>
		[SugarColumn(ColumnName = "warehouse_name", IsNullable = false, Length = 100, ColumnDescription = "仓库名称")]
		public string WarehouseName { get; set; }
		/// <summary>
		/// 仓库类型 warehouse-普通仓 cold-冷库 freezer-冷冻库
		/// </summary>
		[SugarColumn(ColumnName = "warehouse_type", IsNullable = true, Length = 20, ColumnDescription = "仓库类型 warehouse-普通仓 cold-冷库 freezer-冷冻库")]
		public string WarehouseType { get; set; }
		/// <summary>
		/// 省份
		/// </summary>
		[SugarColumn(ColumnName = "province", IsNullable = true, Length = 50, ColumnDescription = "省份")]
		public string Province { get; set; }
		/// <summary>
		/// 城市
		/// </summary>
		[SugarColumn(ColumnName = "city", IsNullable = true, Length = 50, ColumnDescription = "城市")]
		public string City { get; set; }
		/// <summary>
		/// 区县
		/// </summary>
		[SugarColumn(ColumnName = "district", IsNullable = true, Length = 50, ColumnDescription = "区县")]
		public string District { get; set; }
		/// <summary>
		/// 详细地址
		/// </summary>
		[SugarColumn(ColumnName = "address", IsNullable = true, Length = 255, ColumnDescription = "详细地址")]
		public string Address { get; set; }
		/// <summary>
		/// 仓库负责人id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "manager_id", IsNullable = true, ColumnDescription = "仓库负责人id")]
		public long? ManagerId { get; set; }

		/// <summary>
		/// 负责人名称（关联 sys_user 表）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string ManagerName
		{
			get
			{
				return ManagerUser?.UserName ?? "";
			}
		}

		/// <summary>
		/// 负责人（导航属性）
		/// </summary>
		[Navigate(NavigateType.OneToOne, nameof(ManagerId))]
		public SysUserEntity ManagerUser { get; set; }
		/// <summary>
		/// 联系电话
		/// </summary>
		[SugarColumn(ColumnName = "phone", IsNullable = true, Length = 20, ColumnDescription = "联系电话")]
		public string Phone { get; set; }
		/// <summary>
		/// 总面积(平方米)
		/// </summary>
		[SugarColumn(ColumnName = "total_area", IsNullable = true, ColumnDescription = "总面积")]
		public decimal? TotalArea { get; set; }
		/// <summary>
		/// 可用面积
		/// </summary>
		[SugarColumn(ColumnName = "usable_area", IsNullable = true, ColumnDescription = "可用面积")]
		public decimal? UsableArea { get; set; }
		/// <summary>
		/// 状态(0正常 1停用)
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
