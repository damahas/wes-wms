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
	/// 出库退货 业务逻辑
	/// </summary>
	public class ReturnOutBiz : IReturnOutBiz
	{
		private readonly IReturnOutService _returnOutService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;

		public ReturnOutBiz(IReturnOutService returnOutService, ISysCodeRuleBiz codeRuleBiz)
		{
			_returnOutService = returnOutService;
			_codeRuleBiz = codeRuleBiz;
		}

		public RowData<WmsReturnOutEntity> GetList(ParamData<WmsReturnOutParam> param)
		{
			int total = 0;
			var result = new RowData<WmsReturnOutEntity>(_returnOutService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsReturnOutEntity> GetById(long id)
		{
			return new ResultData<WmsReturnOutEntity>(_returnOutService.GetById(id));
		}

		public ReturnData Save(WmsReturnOutEntity model)
		{
			// 新增且未传编码时，按规则自动生成单据编码
			if (model.ReturnId == 0 && string.IsNullOrWhiteSpace(model.ReturnNo))
			{
				string err;
				model.ReturnNo = _codeRuleBiz.GetCode("WMS_RETURN_OUT", out err);
				if (!string.IsNullOrWhiteSpace(err))
					return new ReturnData(500, err);
			}
			// 新建时默认待审核（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
			if (string.IsNullOrWhiteSpace(model.ReturnStatus))
				model.ReturnStatus = "pending";

			return _returnOutService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");
			return _returnOutService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}
	}
}
