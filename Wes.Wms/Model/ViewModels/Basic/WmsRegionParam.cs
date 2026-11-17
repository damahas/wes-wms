namespace Wes.Model
{
	/// <summary>
	/// 省市区 查询参数（平铺，配合 query-form）
	/// </summary>
	public class WmsRegionParam
	{
		public string? Code { get; set; }
		public string? Name { get; set; }
		public int PageNum { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}
