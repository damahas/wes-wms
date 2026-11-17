using System;
using System.Collections.Generic;
using System.Linq;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 检验单 业务逻辑（一个检验单 = 一个收货单明细/物料）
	/// </summary>
	public class IqcOrderBiz : IIqcOrderBiz
	{
		private readonly IIqcOrderService _iqcOrderService;
		private readonly IIqcItemService _iqcItemService;
		private readonly IReceivingDetailService _receivingDetailService;
		private readonly IReceiptBiz _receiptBiz;
		private readonly ISysCodeRuleBiz _codeRuleBiz;

		public IqcOrderBiz(IIqcOrderService iqcOrderService, IIqcItemService iqcItemService, IReceivingDetailService receivingDetailService,
			IReceiptBiz receiptBiz, ISysCodeRuleBiz codeRuleBiz)
		{
			_iqcOrderService = iqcOrderService;
			_iqcItemService = iqcItemService;
			_receivingDetailService = receivingDetailService;
			_receiptBiz = receiptBiz;
			_codeRuleBiz = codeRuleBiz;
		}

		public RowData<WmsIqcOrderEntity> GetList(ParamData<WmsIqcOrderParam> param)
		{
			int total = 0;
			var list = _iqcOrderService.GetList(param, out total);
			var result = new RowData<WmsIqcOrderEntity>(list) { total = total };
			return result;
		}

		public RowData<WmsIqcOrderEntity> GetDefectList(ParamData<WmsIqcDefectParam> param)
		{
			int total = 0;
			var list = _iqcOrderService.GetDefectList(param, out total);
			return new RowData<WmsIqcOrderEntity>(list) { total = total };
		}

		public ResultData<List<WmsIqcOrderItemEntity>> GetNgItems(long inspectionId)
		{
			return new ResultData<List<WmsIqcOrderItemEntity>>(_iqcOrderService.GetNgItems(inspectionId));
		}

		public ResultData<WmsIqcOrderEntity> GetById(long id)
		{
			var data = _iqcOrderService.GetById(id);
			if (data == null)
				return new ResultData<WmsIqcOrderEntity>(500, "数据不存在");
			return new ResultData<WmsIqcOrderEntity>(data);
		}

		/// <summary>
		/// 获取检验项：已保存项 + 模板中未检项合并（未检项 ItemId=0，前端据此识别）
		/// </summary>
		public ResultData<List<WmsIqcOrderItemEntity>> GetItems(long inspectionId)
		{
			var saved = _iqcOrderService.GetItems(inspectionId);

			var order = _iqcOrderService.GetById(inspectionId);
			if (order != null && order.MaterialId > 0)
			{
				var templates = _iqcItemService.GetItemsByMaterial(order.MaterialId);
				foreach (var t in templates)
				{
					if (!saved.Any(s => s.ItemCode == t.ItemCode))
					{
						saved.Add(new WmsIqcOrderItemEntity
						{
							InspectionId = inspectionId,
							ItemCode = t.ItemCode,
							ItemName = t.ItemName,
							InspectionType = t.InspectionType,
							StandardValue = t.StandardValue,
							Unit = t.Unit,
							IsCritical = t.IsCritical,
							TestMethod = t.TestMethod,
							SortNo = t.SortNo,
						});
					}
				}
			}

			return new ResultData<List<WmsIqcOrderItemEntity>>(saved.OrderBy(x => x.SortNo).ToList());
		}

		public ReturnData Save(WmsIqcOrderSaveDto dto)
		{
			if (dto?.Inspection == null) return new ReturnData(500, "检验单主表不能为空");

			var order = dto.Inspection;

			// 新建时默认待检验（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
			if (string.IsNullOrWhiteSpace(order.InspectStatus))
				order.InspectStatus = "pending";

			// 保存检验单主表
			bool saved = _iqcOrderService.Save(order);
			if (!saved) return new ReturnData(500, "保存检验单失败");

			// 保存检验项
			if (dto.Items != null && dto.Items.Count > 0)
			{
				_iqcOrderService.SaveItems(order.InspectionId, dto.Items);
			}

			return new ReturnData();
		}

		public ReturnData Complete(long inspectionId)
		{
			var order = _iqcOrderService.GetById(inspectionId);
			if (order == null) return new ReturnData(500, "检验单不存在");
			if (order.InspectStatus == "completed") return new ReturnData(500, "检验单已完成");

			// 1. 更新检验单状态
			order.InspectStatus = "completed";
			order.InspectDate = DateTime.Now;
			if (!_iqcOrderService.Save(order))
				return new ReturnData(500, "更新检验单状态失败");

			// 2. 创建入库单（待入库状态），合格数量 = 允许入库量
			//    同一收货单已存在待入库的入库单时，明细直接合并过去，不重复建单
			var allowQty = order.AllowQuantity ?? 0;
			if (allowQty > 0 && order.ReceivingId > 0)
			{
				var receiptDetails = new List<WmsReceiptDetailEntity>
				{
					new WmsReceiptDetailEntity
					{
						MaterialId = order.MaterialId,
						Spec = order.Spec,
						Unit = order.Unit,
						BatchNo = order.BatchNo,
						ReceiptQuantity = (int)allowQty,
						SupplierId = order.SupplierId,
						LocationId = order.Material?.DefaultLocationId,
					}
				};

				_receiptBiz.CreateFromInspection(order.ReceivingId,
					order.SupplierId > 0 ? order.SupplierId : null, receiptDetails);
			}

			// 3. 回写收货单明细：待入库数量 += 允许入库数，退货数量 += 退货数
			if (order.ReceivingDetailId > 0)
			{
				if (allowQty > 0)
					_receivingDetailService.UpdatePendingInboundQty(order.ReceivingDetailId, allowQty);

				var returnQty = order.ReturnQuantity ?? 0;
				if (returnQty > 0)
					_receivingDetailService.UpdateReturnQty(order.ReceivingDetailId, returnQty);
			}

			return new ReturnData();
		}

		public ReturnData Delete(string ids)
		{
			var idList = ids.Split(',').Where(s => long.TryParse(s, out _)).Select(long.Parse).ToList();
			if (idList.Count == 0) return new ReturnData(500, "参数错误");

			// 删除检验项
			_iqcOrderService.DeleteItems(idList);
			// 删除检验单
			_iqcOrderService.Delete(idList);

			return new ReturnData();
		}

		/// <summary>
		/// 从收货单明细创建检验单（一个收货单明细 → 一个检验单）
		/// </summary>
		public WmsIqcOrderEntity CreateFromReceiving(string inspectionNo, long receivingId, long? supplierId,
			WmsReceivingDetailEntity inspectDetail)
		{
			return new WmsIqcOrderEntity
			{
				InspectionNo = inspectionNo,
				ReceivingId = receivingId,
				ReceivingDetailId = inspectDetail.DetailId,
				SupplierId = supplierId ?? 0L,
				MaterialId = inspectDetail.MaterialId,
				Spec = inspectDetail.Spec,
				Unit = inspectDetail.Unit,
				BatchNo = inspectDetail.BatchNo,
				DeliveryQuantity = inspectDetail.PlanQuantity,
				InspectStatus = "pending"
			};
		}

		/// <summary>
		/// 为检验单生成检验项（从物料供应商关联的检验项模板复制）
		/// </summary>
		public WmsIqcOrderEntity CreateFromInspection(string inspectionNo, long receivingId, string receivingNo, long? supplierId,
			List<WmsReceivingDetailEntity> details, List<WmsIqcOrderItemEntity> templateItems)
		{
			// 此方法保留兼容性，不再使用
			throw new NotSupportedException("请使用 CreateFromReceiving(inspectionNo, receivingId, receivingNo, supplierId, inspectDetail) 方法");
		}
	}
}
