using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Services.IServices;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDashboardController : ControllerBase
    {
        private readonly IEmployeeDashboardService _employeeDashboardService;
        public EmployeeDashboardController(IEmployeeDashboardService employeeDashboardService)
        {
            _employeeDashboardService = employeeDashboardService;
        }

        [HttpGet("GetEmployeeDashboardDetails")]
        [Authorize(Policy = "EmployeeOrTeamLeadOrBDM")]
        public async Task<IActionResult> GetEmployeeDashboardDetails([Required]int month,[Required]int year)
        {
            var employeeDashboardDetails = await _employeeDashboardService.GetEmployeeDashboardDetails(month,year);
            return Ok(employeeDashboardDetails);
        }
    }
}
