using SqlSugar;
using Wes.Model;
using System.Collections.Generic;

namespace Wes.Service
{
    /// <summary>
    /// 收货单明细 数据访问接口
    /// </summary>
	public interface IReceivingDetailService
	{
		List<WmsReceivingDetailEntity> GetListByReceivingId(long receivingId);
		bool BatchSave(List<WmsReceivingDetailEntity> details);
		bool DeleteByReceivingId(long receivingId);
		bool UpdateInboundQuantities(long detailId, decimal returnQty, decimal stockInQty);
		bool UpdatePendingInboundQty(long detailId, decimal qty);
		bool UpdateReturnQty(long detailId, decimal qty);
		bool UpdateStockInQuantity(long detailId, decimal qty);
		List<WmsReceivingDetailEntity> GetIncompleteDetails(long receivingId);
	}
}
