using Microsoft.AspNetCore.Http;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

namespace TimeTaskTracking.Core.Services;

public class TeamAdminDashboardService : ITeamAdminDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _contextAccessor;
    public TeamAdminDashboardService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<ResponseModel<ManagerProjectSummaryViewModel>> GetProjectsSummaryByManager(TeamsProductivityRequestViewModel teamsProductivityRequest)
    {
        var response = new ResponseModel<ManagerProjectSummaryViewModel>();
        try
        {
            if (teamsProductivityRequest.Month == 0 || teamsProductivityRequest.Year == 0)
            {
                teamsProductivityRequest.Month = DateTime.UtcNow.Month;
                teamsProductivityRequest.Year = DateTime.UtcNow.Year;
            }
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, teamsProductivityRequest.DepartmentId, teamsProductivityRequest.TeamAdminId);
            teamsProductivityRequest.TeamAdminId = claims.UserId;
            teamsProductivityRequest.DepartmentId = claims.DepartmentId;
            var projectsSummary = await _unitOfWork.TeamAdminDashboard.GetProjectsSummaryByManager(teamsProductivityRequest);
            if (projectsSummary == null)
                response.Message.Add(SharedResources.ProjectSummaryNotFound);
            else
                response.Model = projectsSummary;
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<List<TeamsSummaryViewModel>>> GetTeamsSummaryByManager(string? teamAdminId, int departmentId)
    {
        var response = new ResponseModel<List<TeamsSummaryViewModel>>();
        try
        {
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, teamAdminId);
            var teamsSummary = await _unitOfWork.TeamAdminDashboard.GetTeamsSummaryByManager(claims.UserId, claims.DepartmentId);
            if (teamsSummary == null)
                response.Message.Add(SharedResources.TeamsSummaryNotFound);
            else
                response.Model = teamsSummary;
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }
    public async Task<ResponseModel<List<TeamProductivityViewModel>>> GetTeamsProductivitySummaryByManager(TeamsProductivityRequestViewModel teamsProductivityRequest)
    {
        var response = new ResponseModel<List<TeamProductivityViewModel>>();
        try
        {
            if(teamsProductivityRequest.Month == 0 || teamsProductivityRequest.Year == 0)
            {
                teamsProductivityRequest.Month = DateTime.UtcNow.Month;
                teamsProductivityRequest.Year = DateTime.UtcNow.Year;
            }
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, teamsProductivityRequest.DepartmentId, teamsProductivityRequest.TeamAdminId);
            teamsProductivityRequest.TeamAdminId = claims.UserId;
            teamsProductivityRequest.DepartmentId = claims.DepartmentId;
            var teamsSummary = await _unitOfWork.TeamAdminDashboard.GetTeamsProductivitySummaryByManager(teamsProductivityRequest);
            if (teamsSummary == null)
                response.Message.Add(SharedResources.TeamsProductivitySummaryNotFound);
            else
                response.Model = teamsSummary;
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }
}
