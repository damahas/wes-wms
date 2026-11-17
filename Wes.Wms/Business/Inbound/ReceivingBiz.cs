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
    /// 收货单 业务逻辑
    /// </summary>
    public class ReceivingBiz : IReceivingBiz
    {
		private readonly IReceivingService _receivingService;
		private readonly IReceivingDetailService _receivingDetailService;
		private readonly IReceiptBiz _receiptBiz;
		private readonly IIqcItemService _iqcItemService;
		private readonly IIqcOrderBiz _iqcOrderBiz;
		private readonly IIqcOrderService _iqcOrderService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;
		private readonly ISqlSugarClient _client;
		private readonly IMaterialSupplierService _materialSupplierService;

		public ReceivingBiz(IReceivingService receivingService, IReceivingDetailService receivingDetailService,
			IReceiptBiz receiptBiz, IIqcItemService iqcItemService,
			IIqcOrderBiz iqcOrderBiz,
			IIqcOrderService iqcOrderService,
			ISysCodeRuleBiz codeRuleBiz, ISqlSugarClient sqlSugarClient,
			IMaterialSupplierService materialSupplierService)
		{
			_receivingService = receivingService;
			_receivingDetailService = receivingDetailService;
			_receiptBiz = receiptBiz;
			_iqcItemService = iqcItemService;
			_iqcOrderBiz = iqcOrderBiz;
			_iqcOrderService = iqcOrderService;
			_codeRuleBiz = codeRuleBiz;
			_client = sqlSugarClient;
			_materialSupplierService = materialSupplierService;
		}

        public RowData<WmsReceivingEntity> GetList(ParamData<WmsReceivingDetailParam> param)
        {
            int total = 0;
            var result = new RowData<WmsReceivingEntity>(_receivingService.GetList(param, out total))
            {
                total = total
            };
            return result;
        }

        public ResultData<WmsReceivingEntity> GetById(long id)
        {
            return new ResultData<WmsReceivingEntity>(_receivingService.GetById(id));
        }

        public ResultData<List<WmsReceivingDetailEntity>> GetDetailList(long receivingId)
        {
            return new ResultData<List<WmsReceivingDetailEntity>>(
                _receivingDetailService.GetListByReceivingId(receivingId));
        }

		public ReturnData Save(WmsReceivingSaveDto dto)
		{
			if (dto == null || dto.Receiving == null)
				return new ReturnData(500, "参数有误！");

			var model = dto.Receiving;
			var details = dto.Details ?? new List<WmsReceivingDetailEntity>();

			// 校验明细
			if (details.Count == 0)
				return new ReturnData(500, "请至少添加一条收货明细！");

			foreach (var item in details)
			{
				if (item.MaterialId <= 0)
					return new ReturnData(500, "请选择收货物料！");
				if (item.PlanQuantity <= 0)
					return new ReturnData(500, "收货数量必须大于0！");
			}

			// 新增且未传编码时，按规则自动生成单据编码
            if (model.ReceivingId == 0 && string.IsNullOrWhiteSpace(model.ReceivingNo))
            {
                string err;
                model.ReceivingNo = _codeRuleBiz.GetCode("WMS_RECEIVING", out err);
                if (!string.IsNullOrWhiteSpace(err))
                    return new ReturnData(500, err);
            }

            // 新建时默认待收货（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
            if (string.IsNullOrWhiteSpace(model.ReceivingStatus))
                model.ReceivingStatus = "pending";

            // 汇总明细数据到主表
            model.TotalQuantity = details.Sum(d => d.PlanQuantity);

            _client.Ado.BeginTran();
            try
            {
                // 保存主表
                if (!_receivingService.Save(model))
                {
                    _client.Ado.RollbackTran();
                    return new ReturnData(500, "保存收货单失败！");
                }

                // 编辑时，先删除旧明细
                if (dto.Receiving.ReceivingId > 0)
                {
                    _receivingDetailService.DeleteByReceivingId(model.ReceivingId);
                }

                // 保存明细
                if (details.Count > 0)
                {
                    foreach (var item in details)
                    {
                        item.ReceivingId = model.ReceivingId;
                        item.DetailId = 0; // 重新生成雪花ID
                    }
                    _receivingDetailService.BatchSave(details);
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

            // 只允许删除"待收货"状态的收货单
            var receivings = _receivingService.GetList(delIds);
            if (receivings == null || receivings.Count == 0)
                return new ReturnData(500, "未找到收货单！");
            var notPending = receivings.Where(a => a.ReceivingStatus != "pending").ToList();
            if (notPending.Count > 0)
            {
                var nos = string.Join("、", notPending.Select(a => a.ReceivingNo));
                return new ReturnData(500, $"以下收货单不是待收货状态，无法删除：{nos}");
            }

            _client.Ado.BeginTran();
            try
            {
                // 软删除主表
                if (!_receivingService.Delete(delIds))
                {
                    _client.Ado.RollbackTran();
                    return new ReturnData(500, "删除失败！");
                }

                // 软删除明细
                foreach (var id in delIds)
                {
                    _receivingDetailService.DeleteByReceivingId(id);
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

        /// <summary>
        /// 确认收货：根据物料情况，免检直接生成入库单，需检生成检验单
        /// </summary>
        public ReturnData ConfirmReceipt(long receivingId)
        {
            var receiving = _receivingService.GetById(receivingId);
            if (receiving == null)
                return new ReturnData(500, "收货单不存在！");
            if (receiving.ReceivingStatus != "pending")
                return new ReturnData(500, "收货单不是待收货状态，无法确认收货！");

            var details = _receivingDetailService.GetListByReceivingId(receivingId);
            if (details == null || details.Count == 0)
                return new ReturnData(500, "收货单没有明细数据！");

            // 分类：免检 / 需检
            // 优先看物料供应商的 inspection_mode 配置，未配置则按检验项模板是否存在判断
            var exemptDetails = new List<WmsReceivingDetailEntity>();
            var inspectDetails = new List<WmsReceivingDetailEntity>();
            foreach (var d in details)
            {
                var inspectionMode = _materialSupplierService.GetInspectionMode(d.MaterialId, receiving.SupplierId);
                bool needInspection;
                if (!string.IsNullOrWhiteSpace(inspectionMode))
                {
                    // exempt-免检, sampling-抽检, all-全检
                    needInspection = inspectionMode != "exempt";
                }
                else
                {
                    // 未配置时回退原逻辑：分类下有检验项模板 → 需检
                    var categoryId = d.Material?.CategoryId;
                    needInspection = !string.IsNullOrWhiteSpace(categoryId)
                        && _iqcItemService.GetList(categoryId).Count > 0;
                }
                (needInspection ? inspectDetails : exemptDetails).Add(d);
            }

            _client.Ado.BeginTran();
            try
            {
                // 1. 更新收货单状态为"收货中"（所有物料待检验/入库完成后才变"已完成"）
                if (!_receivingService.UpdateStatus(receivingId, "arriving"))
                {
                    _client.Ado.RollbackTran();
                    return new ReturnData(500, "更新收货单状态失败！");
                }

                // 2. 免检 → 生成入库单（待入库状态），设置待入库数量
                if (exemptDetails.Count > 0)
                {
                    string codeErr;
                    var receiptNo = _codeRuleBiz.GetCode("WMS_RECEIPT", out codeErr);
                    if (!string.IsNullOrWhiteSpace(codeErr))
                    {
                        _client.Ado.RollbackTran();
                        return new ReturnData(500, $"生成入库单号失败：{codeErr}");
                    }
                    _receiptBiz.CreateFromReceiving(receiptNo, receivingId, receiving.SupplierId, exemptDetails);
                    foreach (var d in exemptDetails)
                    {
                        _receivingDetailService.UpdatePendingInboundQty(d.DetailId, d.PlanQuantity);
                    }
                }

				// 3. 需检 → 每个收货单明细生成一个检验单
				if (inspectDetails.Count > 0)
				{
					for (int i = 0; i < inspectDetails.Count; i++)
					{
						// 每张检验单独立取号，保证一单一号（不再使用 单号-序号 的形式）
						string codeErr;
						var inspectionNo = _codeRuleBiz.GetCode("WMS_INSPECT", out codeErr);
						if (!string.IsNullOrWhiteSpace(codeErr))
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, $"生成检验单号失败：{codeErr}");
						}

						var detail = inspectDetails[i];
						var order = _iqcOrderBiz.CreateFromReceiving(inspectionNo, receivingId,
							receiving.SupplierId, detail);

						// 先保存检验单主表，获得 InspectionId
						if (!_iqcOrderService.Save(order))
						{
							_client.Ado.RollbackTran();
							return new ReturnData(500, "保存检验单失败！");
						}
					}
				}

                _client.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                _client.Ado.RollbackTran();
                return new ReturnData(500, $"确认收货失败：{ex.Message}");
            }

            return new ReturnData();
        }
    }
}
