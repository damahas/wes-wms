namespace Wes.Model
{
	/// <summary>
	/// 检验项 查询参数
	/// </summary>
	public class WmsIqcItemParam
	{
		/// <summary>
		/// 关键字(模糊检索编码/名称)
		/// </summary>
		public string Keywords { get; set; }
		/// <summary>
		/// 物料分类
		/// </summary>
		public string CategoryId { get; set; }
		/// <summary>
		/// 状态
		/// </summary>
		public string Status { get; set; }
	}
}
