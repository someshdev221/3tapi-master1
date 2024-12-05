
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Core.Services.IServices;

public interface IEmployeeDashboardService
{
    Task<ResponseModel<EmployeeDashboardViewModel>> GetEmployeeDashboardDetails(int month, int year);
}
