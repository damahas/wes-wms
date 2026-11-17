using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 退货单 业务逻辑
	/// </summary>
	public class ReturnOrderBiz : IReturnOrderBiz
	{
		private readonly IReturnOrderService _returnOrderService;
		private readonly IReturnOrderDetailService _returnOrderDetailService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;
		private readonly ISqlSugarClient _client;

		public ReturnOrderBiz(IReturnOrderService returnOrderService, IReturnOrderDetailService returnOrderDetailService,
			ISysCodeRuleBiz codeRuleBiz, ISqlSugarClient sqlSugarClient)
		{
			_returnOrderService = returnOrderService;
			_returnOrderDetailService = returnOrderDetailService;
			_codeRuleBiz = codeRuleBiz;
			_client = sqlSugarClient;
		}

		public RowData<WmsReturnOrderEntity> GetList(ParamData<WmsReturnOrderParam> param)
		{
			int total = 0;
			var result = new RowData<WmsReturnOrderEntity>(_returnOrderService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsReturnOrderEntity> GetById(long id)
		{
			return new ResultData<WmsReturnOrderEntity>(_returnOrderService.GetById(id));
		}

		public ResultData<List<WmsReturnOrderDetailEntity>> GetDetailList(long returnOrderId)
		{
			return new ResultData<List<WmsReturnOrderDetailEntity>>(
				_returnOrderDetailService.GetListByReturnOrderId(returnOrderId));
		}

		public ReturnData Save(WmsReturnOrderSaveDto dto)
		{
			if (dto == null || dto.ReturnOrder == null)
				return new ReturnData(500, "参数有误！");

			var model = dto.ReturnOrder;
			var details = dto.Details ?? new List<WmsReturnOrderDetailEntity>();

			// 校验明细
			if (details.Count == 0)
				return new ReturnData(500, "请至少添加一条退货明细！");

			foreach (var item in details)
			{
				if (item.MaterialId <= 0)
					return new ReturnData(500, "请选择退货物料！");
				if (item.PlanQuantity <= 0)
					return new ReturnData(500, "退货数量必须大于0！");
			}

			// 新增且未传编码时，按规则自动生成单据编码
			if (model.ReturnOrderId == 0 && string.IsNullOrWhiteSpace(model.ReturnOrderNo))
			{
				string err;
				model.ReturnOrderNo = _codeRuleBiz.GetCode("WMS_RETURN_ORDER", out err);
				if (!string.IsNullOrWhiteSpace(err))
					return new ReturnData(500, err);
			}

			// 新建时默认待处理（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
			if (string.IsNullOrWhiteSpace(model.ReturnStatus))
				model.ReturnStatus = "pending";

			// 汇总明细数据到主表
			model.TotalQuantity = details.Sum(d => d.PlanQuantity);

			_client.Ado.BeginTran();
			try
			{
				// 保存主表
				if (!_returnOrderService.Save(model))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存退货单失败！");
				}

				// 编辑时，先删除旧明细
				if (model.ReturnOrderId > 0)
				{
					_returnOrderDetailService.DeleteByReturnOrderId(model.ReturnOrderId);
				}

				// 保存明细
				if (details.Count > 0)
				{
					foreach (var item in details)
					{
						item.ReturnOrderId = model.ReturnOrderId;
						item.DetailId = 0; // 重新生成雪花ID
					}
					_returnOrderDetailService.BatchSave(details);
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"保存失败：{ex.Message}");
			}

			return new ReturnData();
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			_client.Ado.BeginTran();
			try
			{
				// 软删除主表
				if (!_returnOrderService.Delete(delIds))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "删除失败！");
				}

				// 软删除明细
				foreach (var id in delIds)
				{
					_returnOrderDetailService.DeleteByReturnOrderId(id);
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"删除失败：{ex.Message}");
			}

			return new ReturnData();
		}
	}
}
