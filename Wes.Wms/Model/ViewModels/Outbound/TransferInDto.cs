using System.Collections.Generic;

namespace Wes.Model
{
	/// <summary>
	/// 调拨移入的明细项
	/// </summary>
	public class TransferInItemDto
	{
		/// <summary>调拨明细id</summary>
		public long DetailId { get; set; }

		/// <summary>本次实收数量（可少于在途，差额继续留在途上）</summary>
		public decimal Quantity { get; set; }

		/// <summary>收货库位id（跨仓调拨开单时不知道对方仓哪个货位空着，收货时才定）</summary>
		public long? ToLocationId { get; set; }
	}

	/// <summary>
	/// 调拨移入参数
	/// </summary>
	public class TransferInDto
	{
		/// <summary>调拨单id</summary>
		public long TransferId { get; set; }

		/// <summary>各明细的实收情况</summary>
		public List<TransferInItemDto> Items { get; set; }
	}
}
