using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 检验单 数据访问接口
	/// </summary>
	public interface IIqcOrderService
	{
		List<WmsIqcOrderEntity> GetList(ParamData<WmsIqcOrderParam> param, out int total);

		/// <summary>来料不良台账（已完成且存在不良的检验单）</summary>
		List<WmsIqcOrderEntity> GetDefectList(ParamData<WmsIqcDefectParam> param, out int total);

		/// <summary>某检验单的不合格(NG)检验项</summary>
		List<WmsIqcOrderItemEntity> GetNgItems(long inspectionId);
		WmsIqcOrderEntity? GetById(long id);
		bool Save(WmsIqcOrderEntity model);
		bool Delete(List<long> ids);
		bool SaveItems(long inspectionId, List<WmsIqcOrderItemEntity> items);
		void BatchInsertItems(List<WmsIqcOrderItemEntity> items);
		bool DeleteItems(List<long> ids);
		List<WmsIqcOrderItemEntity> GetItems(long inspectionId);
		List<WmsIqcOrderEntity> GetUncompletedByReceivingId(long receivingId);
	}
}
