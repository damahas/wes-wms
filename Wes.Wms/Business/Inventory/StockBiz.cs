using System;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.Business
{
	/// <summary>
	/// 实时库存 业务逻辑
	/// </summary>
	public class StockBiz : IStockBiz
	{
		private readonly IStockService _stockService;

		public StockBiz(IStockService stockService)
		{
			_stockService = stockService;
		}

		public RowData<WmsStockEntity> GetList(ParamData<WmsStockParam> param)
		{
			int total = 0;
			var result = new RowData<WmsStockEntity>(_stockService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsStockEntity> GetById(long id)
		{
			return new ResultData<WmsStockEntity>(_stockService.GetById(id));
		}

		public ReturnData Save(WmsStockEntity model)
		{
			return _stockService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _stockService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}
	}
}
