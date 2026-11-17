using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
    /// <summary>
    /// 仪表盘
    /// </summary>
    [ApiController]
    [Route("wms/dashboard")]
    [ApiExplorerSettings(GroupName = "Dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardBiz _dashboardBiz;

        public DashboardController(IDashboardBiz dashboardBiz)
        {
            _dashboardBiz = dashboardBiz;
        }

        /// <summary>
        /// 获取仪表盘数据
        /// </summary>
        [HttpGet]
        public ReturnData Get()
        {
            return _dashboardBiz.GetDashboard();
        }
    }
}
