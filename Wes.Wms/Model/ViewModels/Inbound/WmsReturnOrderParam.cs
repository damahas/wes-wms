namespace Wes.Model
{
	/// <summary>
	/// 退货单 查询参数
	/// </summary>
	public class WmsReturnOrderParam
	{
		public string? Keywords { get; set; }
		public long? SupplierId { get; set; }
		public string? ReturnType { get; set; }
		public string? ReturnStatus { get; set; }
	}
}
