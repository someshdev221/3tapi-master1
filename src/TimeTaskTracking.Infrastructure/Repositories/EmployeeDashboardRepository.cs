using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class EmployeeDashboardRepository :IEmployeeDashboardRepository
{
    private readonly ExecuteProcedure _exec;
    public EmployeeDashboardRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }

    public async Task<EmployeeDashboardViewModel> GetEmployeeDashboardDetails(string employeeId,int month, int year)
    {
        try
        {
            var obj = await _exec.Get_DataSetAsync("GetEmployeeDashboardDetails",
            new string[] { "@EmployeeId", "@Month", "@Year" },
            new string[] { Convert.ToString(employeeId), month.ToString(),year.ToString() });

            if (obj?.Tables?.Count > 0)
            {
                EmployeeDashboardViewModel employeeDashboardViewModel = new();
                employeeDashboardViewModel.EmployeeName = obj.Tables[2].Rows[0]["EmployeeName"].ToString();
                employeeDashboardViewModel.EmployeeNumber = obj.Tables[2].Rows[0]["EmployeeNumber"]?.ToString();
                employeeDashboardViewModel.AttendanceList = await _exec.DataTableToListAsync<EmployeeDashboardAttendanceViewModel>(obj?.Tables[0]);
                employeeDashboardViewModel.UserProjects = await _exec.DataTableToListAsync<DropDownResponse<int>>(obj?.Tables[1]);
                employeeDashboardViewModel.EmployeeBadges = await _exec.DataTableToListAsync<EmployeeDashboardBadgesViewModel>(obj?.Tables[3]);
                return employeeDashboardViewModel;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
