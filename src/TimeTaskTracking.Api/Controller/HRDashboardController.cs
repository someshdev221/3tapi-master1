using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "HROnly")]
    public class HRDashboardController : ControllerBase
    {
        public readonly IHRDashboardService _hrDashboardService;
        private readonly IValidator<UpdateEmployeeProfileByManagerViewModel> _validator;

        public HRDashboardController(IEmployeeService employeeService, IValidator<UpdateEmployeeProfileByManagerViewModel> validator, IHRDashboardService hrDashboardService)
        {
            _validator = validator;
            _hrDashboardService = hrDashboardService;

        }

        [HttpGet("GetALLTeamListByDepartment")]
        public async Task<IActionResult> GetTeamsByHR([FromQuery] int departmentId, string? teamAdminId)
        {
            var teamsData = await _hrDashboardService.GetTeamsByHR(departmentId, teamAdminId);         
            return Ok(teamsData);
        }
    }
}
