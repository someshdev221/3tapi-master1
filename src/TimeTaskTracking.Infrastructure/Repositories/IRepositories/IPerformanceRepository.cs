using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface IPerformanceRepository
{
    Task<List<EmployeePerformanceSummaryViewModel>> GetEmployeeWorkedProjectSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<List<ProjectPerformanceDetailsViewModel>> GetEmployeeWorkedProjectWithBillingDetailSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<List<MonthlyBillingSummaryViewModel>> GetEmployeeMonthlyBillingSummary(string employeeId,int departmentId);
    Task<EmployeeAttendanceSummaryViewModel> GetEmployeeAttendanceSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<List<EmployeePerformanceSummaryViewModel>> GetTeamWorkedProjectSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<List<ProjectPerformanceDetailsViewModel>> GetTeamWorkedProjectWithBillingDetailSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<List<MonthlyBillingSummaryViewModel>> GetTeamMonthlyBillingSummary(string employeeId, int departmentId);
    Task<EmployeeAttendanceSummaryViewModel> GetTeamAttendanceSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<List<TeamProductivityPerformanceReportViewModel>> GetTeamProductivitySummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
}
