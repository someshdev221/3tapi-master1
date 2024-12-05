
using Microsoft.AspNetCore.Mvc;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

namespace TimeTaskTracking.Core.Services.IServices;

public interface ITeamAdminDashboardService
{
    Task<ResponseModel<ManagerProjectSummaryViewModel>> GetProjectsSummaryByManager(TeamsProductivityRequestViewModel teamsProductivityRequest);
    Task<ResponseModel<List<TeamsSummaryViewModel>>> GetTeamsSummaryByManager(string? teamAdminId, int departmentId);
    Task<ResponseModel<List<TeamProductivityViewModel>>> GetTeamsProductivitySummaryByManager(TeamsProductivityRequestViewModel teamsProductivityRequest);
}
