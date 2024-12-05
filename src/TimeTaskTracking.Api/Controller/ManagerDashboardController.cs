using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

namespace TimeTaskTracking.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "ManagerOrAdminOrHOD")]
public class ManagerDashboardController : ControllerBase
{
    private readonly ITeamAdminDashboardService _teamAdminDashboardService;
    public ManagerDashboardController(ITeamAdminDashboardService teamAdminDashboardService)
    {
        _teamAdminDashboardService = teamAdminDashboardService;
    }
    [HttpGet("GetProjectsSummaryByManager")]
    public async Task<IActionResult> GetProjectsSummaryByManager([FromQuery] TeamsProductivityRequestViewModel teamsProductivityRequest)
    {
        var projectsSummary = await _teamAdminDashboardService.GetProjectsSummaryByManager(teamsProductivityRequest);      
        return Ok(projectsSummary);
    }
    [HttpGet("GetTeamsSummaryByManager")]
    public async Task<IActionResult> GetTeamsSummaryByManager([FromQuery] string? teamAdminId, [FromQuery] int departmentId)
    {
        var teamsSummary = await _teamAdminDashboardService.GetTeamsSummaryByManager(teamAdminId, departmentId);      
        {
            var groupedTeams = teamsSummary.Model
             .GroupBy(t => new { t.TeamLeadId, t.TeamLeadName, t.TeamLeadProfilePicture })
             .Select(g => new
             {
                 TeamLeadId = g.Key.TeamLeadId,
                 TeamLeadName = g.Key.TeamLeadName,
                 TeamLeadProfilePicture = g.Key.TeamLeadProfilePicture,
                 TeamMembers = g.Select(x => new { x.TeamMemberId, x.TeamMemberName, x.TeamMemberExperienceOnJoining,x.TeamMemberJoiningDate, x.TeamMemberProfilePicture ,x.TeamMemberDesignation }).ToList()
             })
             .ToList();
            return Ok(new ResponseModel<dynamic> { Model = groupedTeams });
        }
    }
    [HttpGet("GetTeamsProductivitySummaryByManager")]
    public async Task<IActionResult> GetTeamsProductivitySummaryByManager([FromQuery] TeamsProductivityRequestViewModel teamsProductivityRequest)
    {
        var productivitySummary = await _teamAdminDashboardService.GetTeamsProductivitySummaryByManager(teamsProductivityRequest);   
        return Ok(productivitySummary);
    }
}
