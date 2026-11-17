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
	/// 出库申请单 业务逻辑（原领料单泛化，outbound_type 区分生产领料 / 销售发货 / 其他）
	///
	/// 与原销售出库订单合并为一张单据：共用同一套底层能力——库存分配器（FEFO/FIFO）、
	/// 出库单执行、库存扣减与流水。通过 wms_delivery.source_type + source_id 泛化回指来源，
	/// 一张出库申请单可以对应多个出库单（部分发料）。
	/// </summary>
	public class IssueOrderBiz : IIssueOrderBiz
	{
		private readonly IIssueOrderService _issueService;
		private readonly ISysCodeRuleBiz _codeRuleBiz;
		private readonly ISqlSugarClient _client;
		private readonly IStockAllocaterBiz _allocaterBiz;
		private readonly IDeliveryBiz _deliveryBiz;

		public IssueOrderBiz(IIssueOrderService issueService, ISysCodeRuleBiz codeRuleBiz,
			ISqlSugarClient sqlSugarClient, IStockAllocaterBiz allocaterBiz, IDeliveryBiz deliveryBiz)
		{
			_issueService = issueService;
			_codeRuleBiz = codeRuleBiz;
			_client = sqlSugarClient;
			_allocaterBiz = allocaterBiz;
			_deliveryBiz = deliveryBiz;
		}

		public RowData<WmsIssueOrderEntity> GetList(ParamData<WmsIssueOrderParam> param)
		{
			int total = 0;
			var result = new RowData<WmsIssueOrderEntity>(_issueService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsIssueOrderEntity> GetById(long id)
		{
			return new ResultData<WmsIssueOrderEntity>(_issueService.GetById(id));
		}

		public ReturnData Save(WmsIssueOrderEntity model)
		{
			// 已发过料的单据不允许改：明细变了会让已出库的数量对不上账
			if (model.IssueId > 0)
			{
				var exist = _issueService.GetById(model.IssueId);
				if (exist == null) return new ReturnData(500, "出库申请单不存在！");
				if ((exist.IssuedQuantity ?? 0) > 0)
					return new ReturnData(500, "该出库申请单已发料，不能再修改！");
				if (exist.IssueStatus == "cancelled")
					return new ReturnData(500, "该出库申请单已取消！");
			}

			// 出库类型：领料/销售发货/其他
			if (string.IsNullOrWhiteSpace(model.OutboundType))
				model.OutboundType = "issue";
			if (model.OutboundType == "sales" && string.IsNullOrWhiteSpace(model.CustomerName))
				return new ReturnData(500, "销售发货请填写客户名称！");

			if (!model.WarehouseId.HasValue || model.WarehouseId.Value <= 0)
				return new ReturnData(500, "请选择仓库！");

			if (model.Details == null || model.Details.Count == 0)
				return new ReturnData(500, "请至少添加一条出库明细！");

			foreach (var d in model.Details)
			{
				if (d.MaterialId <= 0)
					return new ReturnData(500, "请选择出库物料！");
				if (d.RequireQuantity <= 0)
					return new ReturnData(500, "需求数量必须大于0！");
			}

			// 新增且未传编码时，按规则自动生成单据编码
			if (model.IssueId == 0 && string.IsNullOrWhiteSpace(model.IssueNo))
			{
				string err;
				model.IssueNo = _codeRuleBiz.GetCode("WMS_ISSUE", out err);
				if (!string.IsNullOrWhiteSpace(err))
					return new ReturnData(500, err);
			}

			var warehouse = _client.Queryable<WmsWarehouseEntity>()
				.Where(p => p.WarehouseId == model.WarehouseId.Value && p.IsDel == 0).First();
			model.WarehouseName = warehouse?.WarehouseName;

			// 补全物料冗余信息与初始欠料数量
			foreach (var d in model.Details)
			{
				var material = _client.Queryable<WmsMaterialEntity>()
					.Where(p => p.MaterialId == d.MaterialId && p.IsDel == 0).First();
				if (material == null)
					return new ReturnData(500, "选择的物料不存在！");

				d.MaterialCode = material.MaterialCode;
				d.MaterialName = material.MaterialName;
				if (string.IsNullOrWhiteSpace(d.Spec)) d.Spec = material.Spec;
				if (string.IsNullOrWhiteSpace(d.Unit)) d.Unit = material.Unit;
				if (d.IssuedQuantity == null) d.IssuedQuantity = 0;
				d.ShortageQuantity = (d.RequireQuantity ?? 0) - (d.IssuedQuantity ?? 0);
			}

			model.TotalQuantity = model.Details.Sum(p => p.RequireQuantity ?? 0);
			if (model.IssuedQuantity == null) model.IssuedQuantity = 0;
			model.ShortageQuantity = (model.TotalQuantity ?? 0) - (model.IssuedQuantity ?? 0);
			// 新建时默认待发料（SqlSugar 会显式写入所有列，数据库默认值不生效，必须在这里给）
			if (string.IsNullOrWhiteSpace(model.IssueStatus)) model.IssueStatus = "pending";

			_client.Ado.BeginTran();
			try
			{
				if (!_issueService.Save(model))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存领料单失败！");
				}

				// 编辑时先软删旧明细再重建，避免残留删除行
				if (model.IssueId > 0)
					_issueService.DeleteDetailsByIssueId(model.IssueId);

				if (!_issueService.BatchSaveDetails(model.IssueId, model.Details))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "保存领料明细失败！");
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

		/// <summary>
		/// 发料：对每条明细按 FEFO/FIFO 找可用批次，有多少发多少。
		/// 库存不足时同样按现有库存生成出库单（部分发料），剩余记为欠料，后续料到再发。
		/// </summary>
		public ReturnData Issue(long issueId)
		{
			var issue = _issueService.GetById(issueId);
			if (issue == null) return new ReturnData(500, "领料单不存在！");
			if (issue.IssueStatus == "issued") return new ReturnData(500, "该领料单已全部发完！");
			if (issue.IssueStatus == "cancelled") return new ReturnData(500, "该领料单已取消，不能发料！");
			if (!issue.WarehouseId.HasValue || issue.WarehouseId.Value <= 0)
				return new ReturnData(500, "领料单未选择仓库！");
			if (issue.Details == null || issue.Details.Count == 0)
				return new ReturnData(500, "领料单没有明细！");

			var lines = new List<WmsDeliveryDetailEntity>();
			foreach (var d in issue.Details)
			{
				var lack = (d.RequireQuantity ?? 0) - (d.IssuedQuantity ?? 0);
				if (lack <= 0) continue;

				// 挑货与销售出库共用同一个分配器：先过期先出，其次先进先出
				var items = _allocaterBiz.Allocate(issue.WarehouseId.Value, d.MaterialId, lack, null, d.BatchNo);
				foreach (var it in items)
				{
					lines.Add(new WmsDeliveryDetailEntity
					{
						MaterialId = d.MaterialId,
						Spec = d.Spec,
						Unit = d.Unit,
						BatchNo = it.Stock.BatchNo,
						ZoneId = it.Stock.ZoneId,
						LocationId = it.Stock.LocationId,
						DeliveryQuantity = it.Quantity,
					});
				}
			}

			if (lines.Count == 0)
				return new ReturnData(500, "当前没有可用库存，暂时无法发料（可用量可能已被冻结或锁定）！");

			var delivery = new WmsDeliveryEntity
			{
				WarehouseId = issue.WarehouseId.Value,
				DeliveryDate = DateTime.Now,
				// 泛化回指来源：Execute 执行完会按这个把数量回写到领料单
				SourceType = "issue",
				SourceId = issueId,
				SourceNo = issue.IssueNo,
				TotalQuantity = lines.Sum(p => p.DeliveryQuantity),
				Details = lines,
			};

			var saveRet = _deliveryBiz.Save(delivery);
			if (saveRet.Code != 200) return saveRet;

			var execRet = _deliveryBiz.Execute(delivery.DeliveryId);
			if (execRet.Code != 200) return execRet;

			// 重新取一次，告诉操作者这次发了多少、还缺多少
			var after = _issueService.GetById(issueId);
			var issued = after?.IssuedQuantity ?? 0;
			var shortage = after?.ShortageQuantity ?? 0;

			return new ReturnData(200, shortage > 0
				? $"发料成功：已生成出库单 {delivery.DeliveryNo}，累计已发 {issued}，仍欠 {shortage}（料到后可再次发料）"
				: $"发料成功：已生成出库单 {delivery.DeliveryNo}，需求已全部发完");
		}

		public ReturnData Cancel(long issueId)
		{
			var issue = _issueService.GetById(issueId);
			if (issue == null) return new ReturnData(500, "领料单不存在！");
			if ((issue.IssuedQuantity ?? 0) > 0)
				return new ReturnData(500, "该领料单已发过料，不能取消！");
			if (issue.IssueStatus == "cancelled")
				return new ReturnData(500, "该领料单已取消！");

			var account = GlobalContext.CurrentUser?.Account;
			var ok = _client.Updateable<WmsIssueOrderEntity>()
				.SetColumns(p => new WmsIssueOrderEntity
				{
					IssueStatus = "cancelled",
					UpdateBy = account,
					UpdateTime = DateTime.Now,
				})
				.Where(p => p.IssueId == issueId)
				.ExecuteCommand() > 0;

			return ok ? new ReturnData() : new ReturnData(500, "取消失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			foreach (var id in delIds)
			{
				var exist = _issueService.GetById(id);
				if (exist != null && (exist.IssuedQuantity ?? 0) > 0)
					return new ReturnData(500, "已发料的领料单不能删除！");
			}

			return _issueService.Delete(delIds)
				? new ReturnData()
				: new ReturnData(500, "删除失败！");
		}
	}
}
