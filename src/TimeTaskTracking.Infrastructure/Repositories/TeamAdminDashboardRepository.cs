using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class TeamAdminDashboardRepository : ITeamAdminDashboardRepository
{
    private readonly ExecuteProcedure _exec;
    public TeamAdminDashboardRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }

    public async Task<ManagerProjectSummaryViewModel> GetProjectsSummaryByManager(TeamsProductivityRequestViewModel teamsProductivityRequest)
    {
        var reportsList = await _exec.Get_DataTableAsync("TeamAdminDashboard",
                 new string[] { "@TeamAdminId", "@DepartmentId", "@Month", "@Year", "@OptId" },
        new string[] { Convert.ToString(teamsProductivityRequest.TeamAdminId), Convert.ToString(teamsProductivityRequest.DepartmentId),
         Convert.ToString(teamsProductivityRequest.Month),Convert.ToString(teamsProductivityRequest.Year), "1" });
        if (reportsList != null && reportsList.Rows.Count > 0)
        {
            return await _exec.DataTableToModelAsync<ManagerProjectSummaryViewModel>(reportsList);
        }
        return null;
    }

    public async Task<List<TeamsSummaryViewModel>> GetTeamsSummaryByManager(string? teamAdminId, int departmentId)
    {
        var reportsList = await _exec.Get_DataTableAsync("TeamAdminDashboard",
                 new string[] { "@TeamAdminId", "@DepartmentId", "@OptId" },
        new string[] { Convert.ToString(teamAdminId), Convert.ToString(departmentId), "2" });
        if (reportsList != null && reportsList.Rows.Count > 0)
        {
            return await _exec.DataTableToListAsync<TeamsSummaryViewModel>(reportsList);
        }
        return null;
    }
    public async Task<List<TeamProductivityViewModel>> GetTeamsProductivitySummaryByManager(TeamsProductivityRequestViewModel teamsProductivityRequest)
    {
        var reportsList = await _exec.Get_DataTableAsync("TeamAdminDashboard",
                 new string[] { "@TeamAdminId", "@DepartmentId", "@Month", "@Year", "@OptId" },
        new string[] { Convert.ToString(teamsProductivityRequest.TeamAdminId), Convert.ToString(teamsProductivityRequest.DepartmentId),
                       Convert.ToString(teamsProductivityRequest.Month),Convert.ToString(teamsProductivityRequest.Year),"3" });
        if (reportsList != null && reportsList.Rows.Count > 0)
        {
            return await _exec.DataTableToListAsync<TeamProductivityViewModel>(reportsList);
        }
        return null;
    }
}
