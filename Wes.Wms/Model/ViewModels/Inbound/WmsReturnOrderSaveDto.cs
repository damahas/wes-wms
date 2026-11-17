using System.Collections.Generic;

namespace Wes.Model
{
	/// <summary>
	/// 退货单 主子表保存DTO
	/// </summary>
	public class WmsReturnOrderSaveDto
	{
		/// <summary>退货单主表</summary>
		public WmsReturnOrderEntity ReturnOrder { get; set; }

		/// <summary>退货明细列表</summary>
		public List<WmsReturnOrderDetailEntity> Details { get; set; }
	}
}
