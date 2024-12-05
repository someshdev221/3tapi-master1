using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class PerformanceRepository : IPerformanceRepository
{
    private readonly ExecuteProcedure _exec;
    public PerformanceRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }
    public async Task<List<EmployeePerformanceSummaryViewModel>> GetEmployeeWorkedProjectSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetEmployeePerformanceReport",
            new string[] { "@OptId", "@EmployeeId", "@StartDate", "@EndDate", "@DepartmentId" },
            new string[] { "1", Convert.ToString(employeeProjectRequestModel.EmployeeId), Convert.ToString(employeeProjectRequestModel.From),
                Convert.ToString(employeeProjectRequestModel.To), Convert.ToString(employeeProjectRequestModel.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToListAsync<EmployeePerformanceSummaryViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<List<ProjectPerformanceDetailsViewModel>> GetEmployeeWorkedProjectWithBillingDetailSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetEmployeePerformanceReport",
            new string[] { "@OptId", "@EmployeeId", "@StartDate", "@EndDate", "@DepartmentId" },
            new string[] { "2", Convert.ToString(employeeProjectRequestModel.EmployeeId), Convert.ToString(employeeProjectRequestModel.From),
                Convert.ToString(employeeProjectRequestModel.To),Convert.ToString(employeeProjectRequestModel.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToListAsync<ProjectPerformanceDetailsViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<List<MonthlyBillingSummaryViewModel>> GetEmployeeMonthlyBillingSummary(string employeeId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetEmployeePerformanceReport",
            new string[] { "@OptId", "@EmployeeId", "@DepartmentId" },
            new string[] { "3", Convert.ToString(employeeId), Convert.ToString(departmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToListAsync<MonthlyBillingSummaryViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<EmployeeAttendanceSummaryViewModel> GetEmployeeAttendanceSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetEmployeePerformanceReport",
            new string[] { "@OptId", "@EmployeeId", "@StartDate", "@EndDate", "@DepartmentId" },
            new string[] { "4", Convert.ToString(employeeProjectRequestModel.EmployeeId), Convert.ToString(employeeProjectRequestModel.From),
                Convert.ToString(employeeProjectRequestModel.To), Convert.ToString(employeeProjectRequestModel.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToModelAsync<EmployeeAttendanceSummaryViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<List<EmployeePerformanceSummaryViewModel>> GetTeamWorkedProjectSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetTeamPerformanceReport",
                new string[] { "@OptId", "@EmployeeId", "@StartDate", "@EndDate", "@DepartmentId" },
                new string[] { "1", Convert.ToString(employeeProjectRequestModel.EmployeeId), Convert.ToString(employeeProjectRequestModel.From),
                Convert.ToString(employeeProjectRequestModel.To), Convert.ToString(employeeProjectRequestModel.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                var performanceList = await _exec.DataTableToListAsync<EmployeePerformanceSummaryViewModel>(obj);

                // Calculate ProductivityPercentage for each item in the list
                performanceList.ForEach(performance =>
                {
                    var totalHours = performance.UpworkHours + performance.FixedBillingHours + performance.NonBillableHours;
                    performance.ProductivityPercentage = totalHours > 0
                        ? ((performance.UpworkHours + performance.FixedBillingHours) / totalHours) * 100
                        : 0;
                });

                return performanceList;
            }

            return null;
        }
        catch (Exception ex)
        {
            // Log exception as needed
            return null;
        }
    }

    public async Task<List<ProjectPerformanceDetailsViewModel>> GetTeamWorkedProjectWithBillingDetailSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetTeamPerformanceReport",
            new string[] { "@OptId", "@EmployeeId", "@StartDate", "@EndDate", "@DepartmentId" },
            new string[] { "2", Convert.ToString(employeeProjectRequestModel.EmployeeId), Convert.ToString(employeeProjectRequestModel.From),
                Convert.ToString(employeeProjectRequestModel.To),Convert.ToString(employeeProjectRequestModel.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToListAsync<ProjectPerformanceDetailsViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<List<MonthlyBillingSummaryViewModel>> GetTeamMonthlyBillingSummary(string employeeId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetTeamPerformanceReport",
            new string[] { "@OptId", "@EmployeeId", "@DepartmentId" },
            new string[] { "3", Convert.ToString(employeeId), Convert.ToString(departmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToListAsync<MonthlyBillingSummaryViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<EmployeeAttendanceSummaryViewModel> GetTeamAttendanceSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetTeamPerformanceReport",
            new string[] { "@OptId", "@EmployeeId", "@StartDate", "@EndDate", "@DepartmentId" },
            new string[] { "4", Convert.ToString(employeeProjectRequestModel.EmployeeId), Convert.ToString(employeeProjectRequestModel.From),
                Convert.ToString(employeeProjectRequestModel.To), Convert.ToString(employeeProjectRequestModel.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToModelAsync<EmployeeAttendanceSummaryViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<List<TeamProductivityPerformanceReportViewModel>> GetTeamProductivitySummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetTeamPerformanceReport",
            new string[] { "@OptId", "@EmployeeId", "@StartDate", "@EndDate", "@DepartmentId" },
            new string[] { "5", Convert.ToString(employeeProjectRequestModel.EmployeeId), Convert.ToString(employeeProjectRequestModel.From),
                Convert.ToString(employeeProjectRequestModel.To), Convert.ToString(employeeProjectRequestModel.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToListAsync<TeamProductivityPerformanceReportViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
