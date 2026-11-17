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
	/// 库存流水 业务逻辑
	/// </summary>
	public class StockRecordBiz : IStockRecordBiz
	{
		private readonly IStockRecordService _stockRecordService;

		public StockRecordBiz(IStockRecordService stockRecordService)
		{
			_stockRecordService = stockRecordService;
		}

		public RowData<WmsStockRecordEntity> GetList(ParamData<WmsStockRecordParam> param)
		{
			int total = 0;
			var result = new RowData<WmsStockRecordEntity>(_stockRecordService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsStockRecordEntity> GetById(long id)
		{
			return new ResultData<WmsStockRecordEntity>(_stockRecordService.GetById(id));
		}

		public ReturnData Save(WmsStockRecordEntity model)
		{
			return _stockRecordService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _stockRecordService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}
	}
}
