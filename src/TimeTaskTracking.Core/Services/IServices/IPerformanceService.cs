using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices;

public interface IPerformanceService
{
    Task<ResponseModel<List<EmployeePerformanceSummaryViewModel>>> GetEmployeeWorkedProjectSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<ResponseModel<List<ProjectPerformanceDetailsViewModel>>> GetEmployeeWorkedProjectWithBillingDetailSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<ResponseModel<List<MonthlyBillingSummaryViewModel>>> GetEmployeeMonthlyBillingSummary(string employeeId,int departmentId);
    Task<ResponseModel<EmployeeAttendanceSummaryViewModel>> GetEmployeeAttendanceSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<ResponseModel<List<EmployeePerformanceSummaryViewModel>>> GetTeamWorkedProjectSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<ResponseModel<List<ProjectPerformanceDetailsViewModel>>> GetTeamWorkedProjectWithBillingDetailSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<ResponseModel<List<MonthlyBillingSummaryViewModel>>> GetTeamMonthlyBillingSummary(string employeeId, int departmentId);
    Task<ResponseModel<EmployeeAttendanceSummaryViewModel>> GetTeamAttendanceSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
    Task<ResponseModel<List<TeamProductivityPerformanceReportViewModel>>> GetTeamProductivitySummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel);
}
