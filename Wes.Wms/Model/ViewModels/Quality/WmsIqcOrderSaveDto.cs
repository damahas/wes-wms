using System.Collections.Generic;

namespace Wes.Model
{
	/// <summary>
	/// 检验单 保存DTO（一个检验单 = 一个物料 + 关联检验项）
	/// </summary>
	public class WmsIqcOrderSaveDto
	{
		/// <summary>检验单主表</summary>
		public WmsIqcOrderEntity Inspection { get; set; }

		/// <summary>检验项列表</summary>
		public List<WmsIqcOrderItemEntity> Items { get; set; }
	}
}
