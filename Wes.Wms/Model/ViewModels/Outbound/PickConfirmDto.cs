namespace Wes.Model
{
	/// <summary>
	/// 拣货确认参数
	/// </summary>
	public class PickConfirmDto
	{
		/// <summary>拣货任务id</summary>
		public long PickId { get; set; }

		/// <summary>本次拣货数量（累加到已拣数量上，支持分次拣）</summary>
		public int Quantity { get; set; }
	}
}
