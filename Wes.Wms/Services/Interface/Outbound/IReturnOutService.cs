using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 出库退货 数据访问接口
	/// </summary>
	public interface IReturnOutService
	{
		List<WmsReturnOutEntity> GetList(ParamData<WmsReturnOutParam> param, out int total);
		WmsReturnOutEntity? GetById(long id);
		bool Save(WmsReturnOutEntity model);
		bool Delete(List<long> ids);
	}
}
