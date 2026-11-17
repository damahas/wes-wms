using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_partner_address", "往来单位地址表")]
	public class WmsPartnerAddressEntity
	{
		/// <summary>
		/// 地址主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "address_id", IsPrimaryKey = true, IsNullable = false)]
		public long AddressId { get; set; }

		/// <summary>
		/// 关联往来单位ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "partner_id", IsNullable = false)]
		public long PartnerId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(PartnerId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Partner { get; set; }

		/// <summary>
		/// 地址类型（字典：收货/发货/注册）
		/// </summary>
		[SugarColumn(ColumnName = "address_type", IsNullable = true, Length = 20)]
		public string AddressType { get; set; }

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
		/// 区/县
		/// </summary>
		[SugarColumn(ColumnName = "district", IsNullable = true, Length = 50, ColumnDescription = "区县")]
		public string District { get; set; }

		/// <summary>
		/// 详细地址
		/// </summary>
		[SugarColumn(ColumnName = "address", IsNullable = false, Length = 255, ColumnDescription = "详细地址")]
		public string Address { get; set; }

		/// <summary>
		/// 联系人姓名
		/// </summary>
		[SugarColumn(ColumnName = "contact_name", IsNullable = true, Length = 50)]
		public string ContactName { get; set; }

		/// <summary>
		/// 联系电话
		/// </summary>
		[SugarColumn(ColumnName = "contact_phone", IsNullable = true, Length = 20)]
		public string ContactPhone { get; set; }

		/// <summary>
		/// 是否默认地址（0=否 1=是）
		/// </summary>
		[SugarColumn(ColumnName = "is_default", IsNullable = true, Length = 1, ColumnDescription = "是否默认")]
		public string IsDefault { get; set; }

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

	}
}
