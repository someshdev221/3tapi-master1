
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;


namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface IReportsRepository
{
    Task<List<ReportsResponseViewModel>> ReportsDetailByTeamLeader(ReportsViewModel model);
    Task<List<MonthlyHoursViewModel>> MonthlyHoursReport(ReportsViewModel model);
    Task<PaginationResponseViewModel<List<ProjectBillingReportViewModel>>> GetProjectReport(ProjectReportRequestModel requestModel);
    Task<PaginationResponseViewModel<List<EmployeeReportViewModel>>> GetActiveEmployeeReport(EmployeeReportRequestModel requestModel);
    Task<List<ProjectReportViewModel>> GetEmployeeProjectReport(EmployeeProjectRequestModel requestModel);
    Task<PaginationResponseViewModel<List<EmployeeAttendanceReportViewModel>>> GetEmployeesAttendanceReport(EmployeeAttendanceRequestViewModel employeeAttendanceRequestViewModel);
    Task<PaginationResponseViewModel<List<EmployeeMonthlyLeaveReportsViewModel>>> GetEmployeesMonthlyLeaveReportByHR(EmployeesMonthlyLeaveRequestViewModel requestViewModel);
    Task<List<TeamReportResponseModel>> GetTeamReportByDepartment(TeamReportRequestModel requestModel);
    Task<PaginationResponseViewModel<List<EmployeeReportResponseModel>>> GetEmployeesReportByDepartmentManager(EmployeeReportRequestViewModel requestModel);
    Task<bool> CheckTeamAdminAndHodDepartment(int DepartmentId, string TeamAdminId);
    Task<List<ProjectPaymentStatus>> GetPaymentStatusReportAsync(string teamAdminId, int departmentId, string searchText);
    Task<List<ClientsReportViewModel>> GetClientsBillingReport(ClientReportRequestViewModel requestModel);
    Task<List<WorkInHandReportViewModel>> GetWorkInHandReportAsync(string? teamAdminId, int departmentId, string? searchText);
    Task<List<GroupFullReportViewModel>> GetFullReportAsync(FullReportRequestViewModel model);
    Task<List<DropDownResponse<string>>> GetEmployeeListForFullReportAsync(string teamAdminId, int departmentId);
    Task<List<DropDownResponse<int>>> GetProjectListForFullReportAsync(string teamAdminId, int departmentId);
    Task<List<DropDownResponse<int>>> GetClientListForFullReportAsync(string teamAdminId, int departmentId);
    Task<List<MonthlyHourFullReportViewModel>> GetMonthlyHoursListForFullReportAsync(MonthlyHourFullReportRequestViewModel model);
    Task<PaginationResponseViewModel<List<ProfileReportViewModel>>> GetProfileReportAsync(ProfileReportRequestViewModel model);
}
