using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 检验单 业务逻辑接口
	/// </summary>
	public interface IIqcOrderBiz
	{
		RowData<WmsIqcOrderEntity> GetList(ParamData<WmsIqcOrderParam> param);

		/// <summary>来料不良台账</summary>
		RowData<WmsIqcOrderEntity> GetDefectList(ParamData<WmsIqcDefectParam> param);

		/// <summary>某检验单的不合格(NG)检验项</summary>
		ResultData<List<WmsIqcOrderItemEntity>> GetNgItems(long inspectionId);
		ResultData<WmsIqcOrderEntity> GetById(long id);
		ResultData<List<WmsIqcOrderItemEntity>> GetItems(long inspectionId);
		ReturnData Save(WmsIqcOrderSaveDto dto);
		ReturnData Complete(long inspectionId);
		ReturnData Delete(string ids);

		/// <summary>从收货单明细创建检验单（一个收货单明细 → 一个检验单）</summary>
		WmsIqcOrderEntity CreateFromReceiving(string inspectionNo, long receivingId, long? supplierId,
			WmsReceivingDetailEntity inspectDetail);
	}
}
