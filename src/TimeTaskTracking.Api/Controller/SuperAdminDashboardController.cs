using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class SuperAdminDashboardController : ControllerBase
    {
        private readonly ISuperAdminDashboardService _superAdminDashboardService;
        public SuperAdminDashboardController(ISuperAdminDashboardService superAdminDashboardService)
        {
            _superAdminDashboardService = superAdminDashboardService;
        }

        [HttpGet("GetDepartmentWiseOverallDetails")]
        public async Task<IActionResult> GetDashboardResults()
        {
            var results = await _superAdminDashboardService.GetDashboardResultsAsync();
            return Ok(results);
        }

        [HttpGet("GetAllDepartmentProductivity")]
        public async Task<IActionResult> GetSuperAdminOverAllPerformance([Required] int month, [Required] int year)
        {
            var results = await _superAdminDashboardService.GetSuperAdminOverAllPerformance(month, year);
            return Ok(results);
        }

    }
}
