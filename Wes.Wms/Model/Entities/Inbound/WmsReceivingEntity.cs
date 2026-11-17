using System;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.Model
{
	[SugarTable("wms_receiving", "收货单表")]
	public class WmsReceivingEntity
	{
		/// <summary>
		/// 收货单id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "receiving_id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "收货单id")]
		public long ReceivingId { get; set; }
		/// <summary>
		/// 收货单号
		/// </summary>
		[SugarColumn(ColumnName = "receiving_no", IsNullable = false, Length = 50, ColumnDescription = "收货单号")]
		public string ReceivingNo { get; set; }
		/// <summary>
		/// 供应商id
		/// </summary>
		[JsonConverter(typeof(LongToStringConverter))]
		[SugarColumn(ColumnName = "supplier_id", IsNullable = true, ColumnDescription = "供应商id")]
		public long? SupplierId { get; set; }
		[Navigate(NavigateType.OneToOne, nameof(SupplierId), nameof(WmsPartnerEntity.PartnerId))]
		public WmsPartnerEntity Supplier { get; set; }
		/// <summary>
		/// 期望到货日期
		/// </summary>
		[SugarColumn(ColumnName = "expect_date", IsNullable = false, ColumnDescription = "期望到货日期")]
		public DateTime? ExpectDate { get; set; }
		/// <summary>
		/// 承运人
		/// </summary>
		[SugarColumn(ColumnName = "carrier_name", IsNullable = true, Length = 100, ColumnDescription = "承运人")]
		public string CarrierName { get; set; }
		/// <summary>
		/// 承运人电话
		/// </summary>
		[SugarColumn(ColumnName = "carrier_phone", IsNullable = true, Length = 20, ColumnDescription = "承运人电话")]
		public string CarrierPhone { get; set; }
		/// <summary>
		/// 收货总数量
		/// </summary>
		[SugarColumn(ColumnName = "total_quantity", IsNullable = true, ColumnDescription = "收货总数量")]
		public int? TotalQuantity { get; set; }
		/// <summary>
		/// 收货状态 pending-待收货 arriving-收货中 completed-已完成 cancelled-已取消
		/// </summary>
		[SugarColumn(ColumnName = "receiving_status", IsNullable = true, Length = 20, ColumnDescription = "收货状态 pending-待收货 arriving-收货中 completed-已完成 cancelled-已取消")]
		public string ReceivingStatus { get; set; }
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
