using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 检验项 数据访问接口
	/// </summary>
	public interface IIqcItemService
	{
		List<WmsIqcItemEntity> GetList(string categoryId);
		WmsIqcItemEntity? GetById(long id);
		bool BatchSave(string categoryId, List<WmsIqcItemEntity> items);
		bool Delete(long id);
		List<WmsIqcItemEntity> GetItemsByMaterial(long materialId);
	}
}
