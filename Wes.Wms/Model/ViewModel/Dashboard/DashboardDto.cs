using System;
using System.Collections.Generic;

namespace Wes.ViewModel.Dashboard
{
    /// <summary>
    /// 首页仪表盘数据
    /// </summary>
    public class DashboardDto
    {
        /// <summary>仓库数量</summary>
        public int WarehouseCount { get; set; }

        /// <summary>库区数量</summary>
        public int ZoneCount { get; set; }

        /// <summary>库位数量</summary>
        public int LocationCount { get; set; }

        /// <summary>空闲库位数</summary>
        public int FreeLocationCount { get; set; }

        /// <summary>物料种类数</summary>
        public int MaterialCount { get; set; }

        /// <summary>库存总数量</summary>
        public decimal TotalStockQuantity { get; set; }

        /// <summary>今日入库单数</summary>
        public int TodayReceiptCount { get; set; }

        /// <summary>今日入库数量</summary>
        public decimal TodayReceiptQuantity { get; set; }

        /// <summary>今日出库单数</summary>
        public int TodayDeliveryCount { get; set; }

        /// <summary>今日出库数量</summary>
        public decimal TodayDeliveryQuantity { get; set; }

        /// <summary>库存预警数</summary>
        public int StockWarningCount { get; set; }

        /// <summary>待发料出库申请数</summary>
        public int PendingIssueCount { get; set; }

        /// <summary>待上架任务数</summary>
        public int PendingShelfCount { get; set; }

        /// <summary>往来单位数</summary>
        public int PartnerCount { get; set; }

        /// <summary>库位使用率</summary>
        public decimal LocationUsageRate { get; set; }

        /// <summary>近7天入库趋势</summary>
        public List<ChartDataItem> ReceiptTrend { get; set; }

        /// <summary>近7天出库趋势</summary>
        public List<ChartDataItem> DeliveryTrend { get; set; }

        /// <summary>最近库存流水</summary>
        public List<RecentRecordItem> RecentRecords { get; set; }
    }

    /// <summary>
    /// 最近库存流水项
    /// </summary>
    public class RecentRecordItem
    {
        /// <summary>流水号</summary>
        public string RecordNo { get; set; }

        /// <summary>物料名称</summary>
        public string MaterialName { get; set; }

        /// <summary>批次号</summary>
        public string BatchNo { get; set; }

        /// <summary>方向 in-入 out-出</summary>
        public string Direction { get; set; }

        /// <summary>数量</summary>
        public decimal Quantity { get; set; }

        /// <summary>业务类型</summary>
        public string BizType { get; set; }

        /// <summary>操作时间</summary>
        public DateTime? OperateTime { get; set; }
    }

    /// <summary>
    /// 图表数据项
    /// </summary>
    public class ChartDataItem
    {
        /// <summary>日期</summary>
        public string Date { get; set; }

        /// <summary>数量</summary>
        public decimal Quantity { get; set; }

        /// <summary>单数</summary>
        public int Count { get; set; }
    }
}
