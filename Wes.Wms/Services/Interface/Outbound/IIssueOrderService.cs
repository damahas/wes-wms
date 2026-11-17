using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 领料单 数据访问接口
	/// </summary>
	public interface IIssueOrderService
	{
		List<WmsIssueOrderEntity> GetList(ParamData<WmsIssueOrderParam> param, out int total);
		WmsIssueOrderEntity? GetById(long id);
		bool Save(WmsIssueOrderEntity model);
		bool Delete(List<long> ids);
		List<WmsIssueOrderDetailEntity> GetDetailsByIssueId(long issueId);
		bool DeleteDetailsByIssueId(long issueId);
		bool BatchSaveDetails(long issueId, List<WmsIssueOrderDetailEntity> details);
		/// <summary>更新单条明细（发料回写数量用）</summary>
		bool SaveDetail(WmsIssueOrderDetailEntity detail);
	}
}
