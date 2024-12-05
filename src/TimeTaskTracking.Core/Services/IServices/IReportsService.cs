using MailKit.Search;
using System.Threading.Tasks;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices;

public interface IReportsService
{
    Task<ResponseModel<List<ReportsResponseViewModel>>> GetReportsListByTeamLeaderAsync(ReportsViewModel model);
    Task<ResponseModel<List<MonthlyHoursViewModel>>> GetMonthlyHoursReportsAsync(ReportsViewModel model);
    Task<ResponseModel<PaginationResponseViewModel<List<ProjectBillingReportViewModel>>>> GetProjectReportAsync(ProjectReportRequestModel projectReportViewModel);
    Task<ResponseModel<PaginationResponseViewModel<List<EmployeeReportViewModel>>>> GetEmployeeReportAsync(EmployeeReportRequestModel employeeReportViewModel);
    Task<ResponseModel<List<ProjectReportViewModel>>> GetEmployeeProjectReportAsync(EmployeeProjectRequestModel employeeReportViewModel);
    Task<ResponseModel<PaginationResponseViewModel<List<EmployeeAttendanceReportViewModel>>>> GetEmployeesAttendanceReport(EmployeeAttendanceRequestViewModel employeeAttendanceRequestViewModel);
    Task<ResponseModel<PaginationResponseViewModel<List<EmployeeMonthlyLeaveReportsViewModel>>>> GetEmployeesMonthlyLeaveReportByHR(EmployeesMonthlyLeaveRequestViewModel employeeMonthlyLeaveReportsViewModel);
    Task<ResponseModel<List<TeamReportResponseModel>>> GetTeamReportByDepartment(TeamReportRequestModel teamReportRequestModel);
    Task<ResponseModel<PaginationResponseViewModel<List<EmployeeReportResponseModel>>>> GetEmployeeReportByDepartmentManager(EmployeeReportRequestViewModel employeeReportRequestViewModel);
    Task<ResponseModel<List<ProjectPaymentStatus>>> GetPaymentStatusReportAsync(string teamAdminId, int departmentId, string searchText);
    Task<ResponseModel<List<ClientsReportViewModel>>> GetClientsBillingReport (ClientReportRequestViewModel clientReportRequestModel);
    Task<ResponseModel<List<WorkInHandReportViewModel>>> GetWorkInHandReportAsync(string? teamAdminId, int departmentId, string? searchText);
    Task<ResponseModel<List<GroupFullReportViewModel>>> GetFullReport(FullReportRequestViewModel model);
    Task<ResponseModel<List<DropDownResponse<string>>>> GetEmployeeListForFullReport(int departmentId, string? teamAdminId);
    Task<ResponseModel<List<DropDownResponse<int>>>> GetProjectListForFullReport(int departmentId, string? teamAdminId);
    Task<ResponseModel<List<DropDownResponse<int>>>> GetClientListForFullReport(int departmentId, string? teamAdminId);
    Task<ResponseModel<List<MonthlyHourFullReportViewModel>>> GetMonthlyHoursListForFullReport(MonthlyHourFullReportRequestViewModel model);
    Task<ResponseModel<PaginationResponseViewModel<List<ProfileReportViewModel>>>> GetProfileReport(DateTime? fromDate, DateTime? toDate, int? pageSize, int? pageNumber);
}

