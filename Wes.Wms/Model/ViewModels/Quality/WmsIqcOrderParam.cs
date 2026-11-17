namespace Wes.Model
{
	/// <summary>
	/// 检验单查询参数
	/// </summary>
	public class WmsIqcOrderParam
	{
		public string Keywords { get; set; }
		public string InspectStatus { get; set; }
		public long? ReceivingId { get; set; }
		public long? SupplierId { get; set; }
	}
}
