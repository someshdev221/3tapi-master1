using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface IDashboardRepository
    {
        Task<List<TeamPerformanceViewModel>> TeamPerformance(string teamLeadId, int departmentId, DateFilterViewModel model);
        Task<List<ProjectsDetailViewModel>> ProjectDetailsByTeamLead(string teamLeadId, int departmentId, DateFilterViewModel model);
        Task<List<CompleteModuleDetailsViewModel>> ModuleDetailsByTeamLead(ModuleBillingRequestViewModel model, int departmentId);
        Task<List<ProjectDetailsModuleAndEmployeeViewModel>> GetProjectDetailsModuleAndEmployeeWiseByTeamLeadAsync(ProjectRequestViewModel model, int departmentId);
        Task<List<TeamAttendanceViewModel>> AttendanceDetails(int departmentId, TeamLeadRequestViewModel model);
        Task<List<TeamLeadPerformanceViewModel>> PerformanceDetails(string teamLeadId, int departmentId, TeamLeadRequestViewModel model);
        Task<List<UserBadgeViewModel>> GetAssignedUserBadges(string teamLeadId, int departmentId);
        Task<TeamProductivityViewModel> GetTeamProductivity(string teamLeadId, TeamLeadProductivityViewModel teamsProductivityRequest, int departmentId);
        Task<List<EmployeeProjectModuleBillingDetailsViewModel>> BillingDetailsModuleWise(EmployeeRequestViewModel model, int departmentId);
        Task<ResponseModel<List<TraineeViewModel>>> GetTraineeListAsync(string teamLeadId);

    }
}
