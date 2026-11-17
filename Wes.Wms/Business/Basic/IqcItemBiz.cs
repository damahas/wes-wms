using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 检验项 业务逻辑
	/// </summary>
	public class IqcItemBiz : IIqcItemBiz
	{
		private readonly IIqcItemService _iqcItemService;
		private readonly ISqlSugarClient _client;

		public IqcItemBiz(IIqcItemService iqcItemService, ISqlSugarClient sqlSugarClient)
		{
			_iqcItemService = iqcItemService;
			_client = sqlSugarClient;
		}

		public RowData<WmsIqcItemEntity> GetList(string categoryId)
		{
			return new RowData<WmsIqcItemEntity>(_iqcItemService.GetList(categoryId))
			{
				total = 0
			};
		}

		public ReturnData BatchSave(string categoryId, List<WmsIqcItemEntity> items)
		{
			_client.Ado.BeginTran();
			try
			{
				_iqcItemService.BatchSave(categoryId, items);
				_client.Ado.CommitTran();
				return new ReturnData();
			}
			catch (System.Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, ex.Message);
			}
		}

		public RowData<WmsIqcItemEntity> GetItemsByMaterial(long materialId)
		{
			return new RowData<WmsIqcItemEntity>(_iqcItemService.GetItemsByMaterial(materialId))
			{
				total = 0
			};
		}

		public ReturnData Delete(long id)
		{
			return _iqcItemService.Delete(id)
				? new ReturnData()
				: new ReturnData(500, "删除失败");
		}
	}
}
