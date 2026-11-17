using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 检验项
	/// </summary>
	[ApiController]
	[Route("wms/iqcItem")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class IqcItemController : ControllerBase
	{
		private readonly IIqcItemBiz _iqcItemBiz;

		public IqcItemController(IIqcItemBiz iqcItemBiz)
		{
			_iqcItemBiz = iqcItemBiz;
		}

		/// <summary>
		/// 获取分类下检验项列表(不分页)
		/// </summary>
		[HttpGet("list")]
		public ReturnData GetList([FromQuery] string categoryId)
		{
			return _iqcItemBiz.GetList(categoryId);
		}

		/// <summary>
		/// 按物料获取检验项(含物料分类及其所有祖先分类的检验项)
		/// </summary>
		[HttpGet("listByMaterial")]
		public ReturnData GetListByMaterial([FromQuery] long materialId)
		{
			return _iqcItemBiz.GetItemsByMaterial(materialId);
		}

		/// <summary>
		/// 批量保存(事务内先删后插)
		/// </summary>
		[HttpPost("batchSave")]
		public ReturnData BatchSave([FromQuery] string categoryId, [FromBody] List<WmsIqcItemEntity> items)
		{
			return _iqcItemBiz.BatchSave(categoryId, items);
		}

		/// <summary>
		/// 删除
		/// </summary>
		[HttpDelete("{id}")]
		public ReturnData Delete(long id)
		{
			return _iqcItemBiz.Delete(id);
		}
	}
}
