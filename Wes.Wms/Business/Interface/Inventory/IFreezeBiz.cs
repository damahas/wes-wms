using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 库存冻结 业务逻辑接口
	/// </summary>
	public interface IFreezeBiz
	{
		RowData<WmsFreezeEntity> GetList(ParamData<WmsFreezeParam> param);
		ResultData<WmsFreezeEntity> GetById(long id);
		ReturnData Save(WmsFreezeEntity model);

		/// <summary>执行冻结：库存冻结数量增加、可用数量减少</summary>
		ReturnData Execute(long freezeId);

		/// <summary>解冻：库存冻结数量减少、可用数量回补</summary>
		ReturnData Thaw(long freezeId, string thawReason);

		ReturnData Delete(string ids);
	}
}
