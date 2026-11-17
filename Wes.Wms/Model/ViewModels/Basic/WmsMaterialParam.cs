namespace Wes.Model
{
	/// <summary>
	/// 物料 查询参数
	/// </summary>
	public class WmsMaterialParam
	{
		public string? Keywords { get; set; }
		public string? Status { get; set; }
		public string? MaterialType { get; set; }
		public string? CategoryId { get; set; }
	}
}
