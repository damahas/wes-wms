namespace Wes.Model
{
	/// <summary>
	/// 来料不良台账查询参数
	/// </summary>
	public class WmsIqcDefectParam
	{
		/// <summary>关键词：检验单号/收货单号/物料编码/物料名称</summary>
		public string Keywords { get; set; }

		/// <summary>供应商</summary>
		public long? SupplierId { get; set; }

		/// <summary>物料</summary>
		public long? MaterialId { get; set; }

		/// <summary>不良品处置方式</summary>
		public string DisposeType { get; set; }

		/// <summary>检验日期起（yyyy-MM-dd）</summary>
		public string BeginDate { get; set; }

		/// <summary>检验日期止（yyyy-MM-dd）</summary>
		public string EndDate { get; set; }
	}
}
