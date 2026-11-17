using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 调拨单 业务逻辑接口
	///
	/// 调拨单只管跨仓：库内搬货请用移库单，两者分开是因为跨仓有在途账、两个阶段、两个责任人，
	/// 硬塞进一张表会让状态机与字段互相打架。
	/// </summary>
	public interface ITransferBiz
	{
		RowData<WmsTransferEntity> GetList(ParamData<WmsTransferParam> param);
		ResultData<WmsTransferEntity> GetById(long id);
		ReturnData Save(WmsTransferEntity model);
		ReturnData Delete(string ids);
		/// <summary>移出：调出仓扣库存、写在途账，由调出仓责任人操作</summary>
		ReturnData TransferOut(long transferId);
		/// <summary>移入：调入仓按实收数量入账，由调入仓责任人操作</summary>
		ReturnData TransferIn(TransferInDto dto);
	}
}
