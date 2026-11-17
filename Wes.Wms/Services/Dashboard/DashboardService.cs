using SqlSugar;
using Wes.Model;

namespace Wes.Service
{
    /// <summary>
    /// 仪表盘 数据访问（仅作 Repository 基类，业务逻辑在 DashboardBiz）
    /// </summary>
    public class DashboardService : Repository<WmsStockEntity>, IDashboardService
    {
        public DashboardService(ISqlSugarClient db) : base(db) { }
    }
}
