using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 领料单 业务逻辑接口
	/// </summary>
	public interface IIssueOrderBiz
	{
		RowData<WmsIssueOrderEntity> GetList(ParamData<WmsIssueOrderParam> param);
		ResultData<WmsIssueOrderEntity> GetById(long id);
		ReturnData Save(WmsIssueOrderEntity model);
		ReturnData Delete(string ids);
		/// <summary>发料：按 FEFO/FIFO 能发多少发多少（部分发料），生成出库单并扣库存</summary>
		ReturnData Issue(long issueId);
		/// <summary>取消（未发过料的才能取消）</summary>
		ReturnData Cancel(long issueId);
	}
}
