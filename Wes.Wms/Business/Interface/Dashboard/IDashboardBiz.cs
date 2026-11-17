using Wes.Utils.Model;
using Wes.ViewModel.Dashboard;

namespace Wes.Business
{
    public interface IDashboardBiz
    {
        ResultData<DashboardDto> GetDashboard();
    }
}
