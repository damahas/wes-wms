using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;
using Wes.ViewModel.Dashboard;

namespace Wes.Business
{
    /// <summary>
    /// 仪表盘 业务逻辑（跨模块聚合统计）
    /// </summary>
    public class DashboardBiz : IDashboardBiz
    {
        private readonly ISqlSugarClient _client;

        public DashboardBiz(ISqlSugarClient sqlSugarClient)
        {
            _client = sqlSugarClient;
        }

        public ResultData<DashboardDto> GetDashboard()
        {
            var today = DateTime.Today;
            var dto = new DashboardDto();

            // 仓库数量
            dto.WarehouseCount = _client.Queryable<WmsWarehouseEntity>()
                .Where(p => p.IsDel == 0)
                .Count();

            // 库区数量
            dto.ZoneCount = _client.Queryable<WmsZoneEntity>()
                .Where(p => p.IsDel == 0)
                .Count();

            // 库位数量 & 空闲库位数
            dto.LocationCount = _client.Queryable<WmsLocationEntity>()
                .Where(p => p.IsDel == 0)
                .Count();
            dto.FreeLocationCount = _client.Queryable<WmsLocationEntity>()
                .Where(p => p.IsDel == 0 && p.LocationStatus == "0")
                .Count();
            dto.LocationUsageRate = dto.LocationCount > 0
                ? Math.Round((decimal)(dto.LocationCount - dto.FreeLocationCount) / dto.LocationCount * 100, 1)
                : 0;

            // 物料种类数
            dto.MaterialCount = _client.Queryable<WmsMaterialEntity>()
                .Where(p => p.IsDel == 0 || p.IsDel == null)
                .Count();

            // 库存总数量
            dto.TotalStockQuantity = _client.Queryable<WmsStockEntity>()
                .Where(p => p.IsDel == 0)
                .Sum(p => p.StockQuantity) ?? 0;

            // 今日入库单数 & 入库数量
            dto.TodayReceiptCount = _client.Queryable<WmsReceiptEntity>()
                .Where(p => p.CreateTime >= today && p.IsDel == 0)
                .Count();
            dto.TodayReceiptQuantity = _client.Queryable<WmsReceiptEntity>()
                .Where(p => p.CreateTime >= today && p.IsDel == 0)
                .Sum(p => p.TotalQuantity) ?? 0;

            // 今日出库单数 & 出库数量
            dto.TodayDeliveryCount = _client.Queryable<WmsDeliveryEntity>()
                .Where(p => p.CreateTime >= today && p.IsDel == 0)
                .Count();
            dto.TodayDeliveryQuantity = _client.Queryable<WmsDeliveryEntity>()
                .Where(p => p.CreateTime >= today && p.IsDel == 0)
                .Sum(p => p.TotalQuantity) ?? 0;

            // 库存预警数
            dto.StockWarningCount = _client.Queryable<WmsStockWarningEntity>()
                .Where(p => p.IsDel == 0 && p.WarningStatus == "0")
                .Count();

            // 待上架任务数
            // 待上架：入库单里还没上架完成的明细条数
            dto.PendingShelfCount = _client.Queryable<WmsReceiptDetailEntity>()
                .Where(p => p.IsDel == 0 && (p.ShelfQuantity ?? 0) < p.ReceiptQuantity)
                .Count();

            // 往来单位数
            dto.PartnerCount = _client.Queryable<WmsPartnerEntity>()
                .Where(p => p.IsDel == 0 || p.IsDel == null)
                .Count();

            // 近7天入库趋势
            var receiptTrend = new List<ChartDataItem>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var nextDate = date.AddDays(1);
                var count = _client.Queryable<WmsReceiptEntity>()
                    .Where(p => p.CreateTime >= date && p.CreateTime < nextDate && p.IsDel == 0)
                    .Count();
                var qty = _client.Queryable<WmsReceiptEntity>()
                    .Where(p => p.CreateTime >= date && p.CreateTime < nextDate && p.IsDel == 0)
                    .Sum(p => p.TotalQuantity) ?? 0;
                receiptTrend.Add(new ChartDataItem
                {
                    Date = date.ToString("MM-dd"),
                    Count = count,
                    Quantity = qty
                });
            }
            dto.ReceiptTrend = receiptTrend;

            // 近7天出库趋势
            var deliveryTrend = new List<ChartDataItem>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var nextDate = date.AddDays(1);
                var count = _client.Queryable<WmsDeliveryEntity>()
                    .Where(p => p.CreateTime >= date && p.CreateTime < nextDate && p.IsDel == 0)
                    .Count();
                var qty = _client.Queryable<WmsDeliveryEntity>()
                    .Where(p => p.CreateTime >= date && p.CreateTime < nextDate && p.IsDel == 0)
                    .Sum(p => p.TotalQuantity) ?? 0;
                deliveryTrend.Add(new ChartDataItem
                {
                    Date = date.ToString("MM-dd"),
                    Count = count,
                    Quantity = qty
                });
            }
            dto.DeliveryTrend = deliveryTrend;

            // 待发料出库申请数（待发料 + 部分发料）
            dto.PendingIssueCount = _client.Queryable<WmsIssueOrderEntity>()
                .Where(p => p.IsDel == 0 && (p.IssueStatus == "pending" || p.IssueStatus == "partial"))
                .Count();

            // 最近 8 条库存流水
            dto.RecentRecords = _client.Queryable<WmsStockRecordEntity>()
                .Includes(p => p.Material)
                .Where(p => p.IsDel == 0)
                .OrderBy(p => p.RecordId, OrderByType.Desc)
                .Take(8)
                .ToList()
                .Select(r => new RecentRecordItem
                {
                    RecordNo = r.RecordNo,
                    MaterialName = r.Material?.MaterialName ?? "",
                    BatchNo = r.BatchNo,
                    Direction = r.Direction,
                    Quantity = r.ChangeQuantity,
                    BizType = r.BizType,
                    OperateTime = r.OperateTime,
                })
                .ToList();

            return new ResultData<DashboardDto>(dto);
        }
    }
}
