namespace Wes.Model
{
	/// <summary>
	/// 物料供应商关系 查询参数
	/// </summary>
	public class WmsMaterialSupplierParam
	{
		/// <summary>
		/// 物料ID（按物料过滤供应商列表）
		/// </summary>
		public long? MaterialId { get; set; }

		public string? Keywords { get; set; }
	}
}
