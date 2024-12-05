using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface ITeamAdminDashboardRepository
{
    Task<ManagerProjectSummaryViewModel> GetProjectsSummaryByManager(TeamsProductivityRequestViewModel teamsProductivityRequest);
    Task<List<TeamsSummaryViewModel>> GetTeamsSummaryByManager(string? teamAdminId, int departmentId);
    Task<List<TeamProductivityViewModel>> GetTeamsProductivitySummaryByManager(TeamsProductivityRequestViewModel teamsProductivityRequest);
}
