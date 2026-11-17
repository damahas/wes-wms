using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_partner", "往来单位表")]
	public class WmsPartnerEntity
	{
		/// <summary>
		/// 往来单位主键ID
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "partner_id", IsPrimaryKey = true, IsNullable = false)]
		public long PartnerId { get; set; }

		/// <summary>
		/// 往来单位编码
		/// </summary>
		[SugarColumn(ColumnName = "partner_code", IsNullable = false, Length = 50)]
		public string PartnerCode { get; set; }

		/// <summary>
		/// 往来单位名称
		/// </summary>
		[SugarColumn(ColumnName = "partner_name", IsNullable = false, Length = 100)]
		public string PartnerName { get; set; }

		/// <summary>
		/// 往来单位类型（字典：供应商/客户）
		/// </summary>
		[SugarColumn(ColumnName = "partner_type", IsNullable = false, Length = 20)]
		public string PartnerType { get; set; }

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
		[SugarColumn(ColumnName = "address", IsNullable = true, Length = 255, ColumnDescription = "详细地址")]
		public string Address { get; set; }

		/// <summary>
		/// 开户银行名称
		/// </summary>
		[SugarColumn(ColumnName = "bank_name", IsNullable = true, Length = 100)]
		public string BankName { get; set; }

		/// <summary>
		/// 银行账号
		/// </summary>
		[SugarColumn(ColumnName = "bank_account", IsNullable = true, Length = 50)]
		public string BankAccount { get; set; }

		/// <summary>
		/// 税号（纳税人识别号）
		/// </summary>
		[SugarColumn(ColumnName = "tax_no", IsNullable = true, Length = 50)]
		public string TaxNo { get; set; }

		/// <summary>
		/// 合作等级（字典：如 重要/普通）
		/// </summary>
		[SugarColumn(ColumnName = "partner_level", IsNullable = true, Length = 20)]
		public string PartnerLevel { get; set; }

		/// <summary>
		/// 结算方式（字典：如 月结/现结）
		/// </summary>
		[SugarColumn(ColumnName = "settle_type", IsNullable = true, Length = 20)]
		public string SettleType { get; set; }

		/// <summary>
		/// 信用额度
		/// </summary>
		[SugarColumn(ColumnName = "credit_limit", IsNullable = true)]
		public decimal? CreditLimit { get; set; }

		/// <summary>
		/// 合作开始日期
		/// </summary>
		[SugarColumn(ColumnName = "cooperate_start_date", IsNullable = true)]
		public DateTime? CooperateStartDate { get; set; }

		/// <summary>
		/// 状态（0=启用 1=停用）
		/// </summary>
		[SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "状态")]
		public string Status { get; set; }

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

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
		public string Remark { get; set; }

	}
}
