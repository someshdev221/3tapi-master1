using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Controller;

[Route("api/[controller]")]
[ApiController]
public class PerformanceReportController : ControllerBase
{
    private readonly IPerformanceService _performanceService;
    public PerformanceReportController(IPerformanceService performanceService)
    {
        _performanceService = performanceService;
    }
    [HttpGet("GetEmployeeWorkedProjectSummary")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHODOrHR")]
    public async Task<IActionResult> GetEmployeeWorkedProjectSummary([FromQuery] DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var employeePerformanceSummary = await _performanceService.GetEmployeeWorkedProjectSummary(employeeProjectRequestModel);
        return Ok(employeePerformanceSummary);
    }
    [HttpGet("GetEmployeeWorkedProjectWithBillingDetailSummary")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHODOrHR")]
    public async Task<IActionResult> GetEmployeeWorkedProjectWithBillingDetailSummary([FromQuery] DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var employeeWorkedProjectWithBillingDetail = await _performanceService.GetEmployeeWorkedProjectWithBillingDetailSummary(employeeProjectRequestModel);
        return Ok(employeeWorkedProjectWithBillingDetail);
    }
    [HttpGet("GetEmployeeMonthlyBillingSummary")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHODOrHR")]
    public async Task<IActionResult> GetEmployeeMonthlyBillingSummary([Required]string employeeId,int departmentId)
    {
        var monthlyBillingSummary = await _performanceService.GetEmployeeMonthlyBillingSummary(employeeId, departmentId);
        return Ok(monthlyBillingSummary);
    }
    [HttpGet("GetEmployeeAttendanceSummary")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHODOrHR")]
    public async Task<IActionResult> GetEmployeeAttendanceSummary([FromQuery] DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var employeeAttendanceSummary = await _performanceService.GetEmployeeAttendanceSummary(employeeProjectRequestModel);
        return Ok(employeeAttendanceSummary);
    }
    [HttpGet("GetTeamWorkedProjectSummary")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHODOrHR")]
    public async Task<IActionResult> GetTeamWorkedProjectSummary([FromQuery] DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var teamPerformanceSummary = await _performanceService.GetTeamWorkedProjectSummary(employeeProjectRequestModel);
        return Ok(teamPerformanceSummary);
    }
    [HttpGet("GetTeamWorkedProjectWithBillingDetailSummary")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHODOrHR")]
    public async Task<IActionResult> GetTeamWorkedProjectWithBillingDetailSummary([FromQuery] DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var teamWorkedProjectWithBillingDetail = await _performanceService.GetTeamWorkedProjectWithBillingDetailSummary(employeeProjectRequestModel);
        return Ok(teamWorkedProjectWithBillingDetail);
    }
    [HttpGet("GetTeamProductivitySummary")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHODOrHR")]
    public async Task<IActionResult> GetTeamProductivitySummary([FromQuery] DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var teamProductivitySummary = await _performanceService.GetTeamProductivitySummary(employeeProjectRequestModel);
        return Ok(teamProductivitySummary);
    }
    [HttpGet("GetTeamMonthlyBillingSummary")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHODOrHR")]
    public async Task<IActionResult> GetTeamMonthlyBillingSummary([Required] string employeeId,int departmentId)
    {
        var monthlyBillingSummary = await _performanceService.GetTeamMonthlyBillingSummary(employeeId, departmentId);
        return Ok(monthlyBillingSummary);
    }
    [HttpGet("GetTeamAttendanceSummary")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHODOrHR")]
    public async Task<IActionResult> GetTeamAttendanceSummary([FromQuery] DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var teamAttendanceSummary = await _performanceService.GetTeamAttendanceSummary(employeeProjectRequestModel);
        return Ok(teamAttendanceSummary);
    }
}
