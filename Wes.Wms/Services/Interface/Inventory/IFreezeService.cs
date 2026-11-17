using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 库存冻结 数据访问接口
	/// </summary>
	public interface IFreezeService
	{
		List<WmsFreezeEntity> GetList(ParamData<WmsFreezeParam> param, out int total);
		WmsFreezeEntity? GetById(long id);
		bool Save(WmsFreezeEntity model);
		bool Delete(List<long> ids);
	}
}
