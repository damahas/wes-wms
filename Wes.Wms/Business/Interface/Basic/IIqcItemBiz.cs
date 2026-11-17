using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 检验项 业务逻辑接口
	/// </summary>
	public interface IIqcItemBiz
	{
		RowData<WmsIqcItemEntity> GetList(string categoryId);
		RowData<WmsIqcItemEntity> GetItemsByMaterial(long materialId);
		ReturnData BatchSave(string categoryId, List<WmsIqcItemEntity> items);
		ReturnData Delete(long id);
	}
}
