using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 包装规格 数据访问接口
	/// </summary>
	public interface IPackSpecService
	{
		List<WmsPackSpecEntity> GetList(ParamData<WmsPackSpecParam> param, out int total);
		List<WmsPackSpecEntity> GetTree(long rootId);
		WmsPackSpecEntity? GetById(long id);
		/// <summary>纯 CRUD：新增或修改（不处理 RootId 业务逻辑）</summary>
		bool Save(WmsPackSpecEntity model);
		/// <summary>纯 CRUD：按 rootId 软删除整棵树</summary>
		bool SoftDeleteByRootId(long rootId);
		/// <summary>纯 CRUD：按 specId 软删除自身及直接子节点</summary>
		bool SoftDeleteSelfAndChildren(long specId);
	}
}
