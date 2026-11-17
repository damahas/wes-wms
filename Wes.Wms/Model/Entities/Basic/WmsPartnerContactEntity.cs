using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_partner_contact", "往来单位联系人表")]
	public class WmsPartnerContactEntity
	{
		/// <summary>
		/// 联系人主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "contact_id", IsPrimaryKey = true, IsIdentity = true, IsNullable = false)]
		public long ContactId { get; set; }

		/// <summary>
		/// 关联往来单位ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "partner_id", IsNullable = false)]
		public long PartnerId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(PartnerId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Partner { get; set; }

		/// <summary>
		/// 联系人姓名
		/// </summary>
		[SugarColumn(ColumnName = "contact_name", IsNullable = true, Length = 50)]
		public string ContactName { get; set; }

		/// <summary>
		/// 职位/称谓
		/// </summary>
		[SugarColumn(ColumnName = "title", IsNullable = true, Length = 50)]
		public string Title { get; set; }

		/// <summary>
		/// 联系电话
		/// </summary>
		[SugarColumn(ColumnName = "phone", IsNullable = true, Length = 20, ColumnDescription = "联系电话")]
		public string Phone { get; set; }

		/// <summary>
		/// 电子邮箱
		/// </summary>
		[SugarColumn(ColumnName = "email", IsNullable = true, Length = 100, ColumnDescription = "邮箱")]
		public string Email { get; set; }

		/// <summary>
		/// 是否默认联系人（0=否 1=是）
		/// </summary>
		[SugarColumn(ColumnName = "is_default", IsNullable = true, Length = 1, ColumnDescription = "是否默认")]
		public string IsDefault { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

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
