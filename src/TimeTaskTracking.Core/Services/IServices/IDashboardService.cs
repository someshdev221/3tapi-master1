using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;


namespace TimeTaskTracking.Core.Services.IServices
{
    public interface IDashboardService
    {
        Task<ResponseModel<List<TeamPerformanceViewModel>>> PerformanceDetails(DateFilterViewModel model, string? teamLeadId);
        Task<ResponseModel<List<ProjectsDetailViewModel>>> ProjectDetails(DateFilterViewModel model, string? teamLeadId, int departmentId);
        Task<ResponseModel<List<CompleteModuleDetailsViewModel>>> ModuleBillingDetails(ModuleBillingRequestViewModel model, int departmentId);
        Task<ResponseModel<List<ProjectDetailsModuleAndEmployeeViewModel>>> GetProjectDetailsModuleAndEmployeeWiseByTeamLead( ProjectRequestViewModel model, int departmentId);
        Task<ResponseModel<List<TeamAttendanceViewModel>>> TeamAttendanceDetails(TeamLeadRequestViewModel model);
        Task<ResponseModel<List<UserBadgeViewModel>>> GetAssignedBadges();
        Task<ResponseModel<List<TeamLeadPerformanceViewModel>>> TeamLeadPerformaceDetails(TeamLeadRequestViewModel model);
        Task<ResponseModel<TeamProductivityViewModel>> GetTeamProductivity(TeamLeadProductivityViewModel teamsProductivityRequest);
        Task<ResponseModel<List<EmployeeProjectModuleBillingDetailsViewModel>>> EmployeeProjectBillingByModule(EmployeeRequestViewModel model, int departmentId);
        Task<ResponseModel<List<TraineeDto>>> GetTraineeListAsync();
    }
}   
