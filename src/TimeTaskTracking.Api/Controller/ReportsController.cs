using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;
        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [HttpGet("GetReportsDetailsByTL")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> GetReportsDetails([FromQuery] ReportsViewModel model)
        {
            var reportsList = await _reportsService.GetReportsListByTeamLeaderAsync(model);
            return Ok(reportsList);
        }

        [HttpGet("GetMonthlyHoursReportByTL")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> GetMonthlyHoursReport([FromQuery] ReportsViewModel model)
        {
            var reportsList = await _reportsService.GetMonthlyHoursReportsAsync(model);
            return Ok(reportsList);
        }

        [HttpGet("GetProjectsReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetProjectBillingReport([FromQuery] ProjectReportRequestModel model)
        {
            var projectList = await _reportsService.GetProjectReportAsync(model);
            return Ok(projectList);
        }

        [HttpGet("GetDevelopersReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetActiveEmployeeReport([FromQuery] EmployeeReportRequestModel model)
        {
            var projectList = await _reportsService.GetEmployeeReportAsync(model);
            return Ok(projectList);
        }

        [HttpGet("GetProfileReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetProfileReport([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int? pageSize, [FromQuery] int? pageNumber)
        {
            var profileReportResult = await _reportsService.GetProfileReport(fromDate, toDate, pageSize, pageNumber);
            return Ok(profileReportResult);
        }


        [HttpGet("GetDeveloperProjectBillingReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetEmployeeProjectBillingReport([FromQuery] EmployeeProjectRequestModel model)
        {
            var projectList = await _reportsService.GetEmployeeProjectReportAsync(model);
            return Ok(projectList);
        }

        [HttpGet("GetEmployeesAttendanceReport")]
        [Authorize(Policy = "ManagerOrHROrAdminOrHOD")]
        public async Task<IActionResult> GetEmployeesAttendanceReport([FromQuery] EmployeeAttendanceRequestViewModel employeeAttendanceRequestViewModel)
        {
            var attendanceReports = await _reportsService.GetEmployeesAttendanceReport(employeeAttendanceRequestViewModel);
            return Ok(attendanceReports);
        }

        [HttpGet("GetEmployeesMonthlyLeaveReportByHR")]
        [Authorize(Policy = "HROnly")]
        public async Task<IActionResult> GetEmployeesMonthlyLeaveReportByHR([FromQuery] EmployeesMonthlyLeaveRequestViewModel employeeMonthlyLeaveReportsViewModel)
        {
            var attendanceReports = await _reportsService.GetEmployeesMonthlyLeaveReportByHR(employeeMonthlyLeaveReportsViewModel);
            return Ok(attendanceReports);
        }

        [HttpGet("GetTeamReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetTeamReportByDepartment([FromQuery] TeamReportRequestModel model)
        {
            var teamReports = await _reportsService.GetTeamReportByDepartment(model);
            return Ok(teamReports);
        }

        [HttpGet("GetEmployeesReport")]
        [Authorize(Policy = "ManagerOrHROrAdminOrHOD")]
        public async Task<IActionResult> GetEmployeeReportByDepartment([FromQuery] EmployeeReportRequestViewModel model)
        {
            var employeeReport = await _reportsService.GetEmployeeReportByDepartmentManager(model);
            return Ok(employeeReport);
        }

        [HttpGet("GetPaymentPendingReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetPaymentStatusReport(string? teamAdminId, int departmentId, string? searchText)
        {
            var result = await _reportsService.GetPaymentStatusReportAsync(teamAdminId, departmentId, searchText);
            return Ok(result);
        }

        [HttpGet("GetClientReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetClientBillingReport([FromQuery] ClientReportRequestViewModel model)
        {
            var clientsReport = await _reportsService.GetClientsBillingReport(model);
            return Ok(clientsReport);
        }

        [HttpGet("GetFullReportByManager")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetFullReport([FromQuery] FullReportRequestViewModel model)
        {
            var dateValidation = await SharedResources.ValidateReportDateFilter(model.FromDate, model.ToDate);
            if (!dateValidation.IsValid)
            {
                var response = new ResponseModel<string>();
                response.Message.Add(SharedResources.ToDateMustGreater);
                return BadRequest(response);
            }
            model.FromDate = dateValidation.FromDate;
            model.ToDate = dateValidation.ToDate;

            var fullReport = await _reportsService.GetFullReport(model);
            if (fullReport.Model != null && fullReport.Model.Any())
            {
                var report = fullReport.Model.FirstOrDefault()?.ReportViewModel?.FirstOrDefault();

                if (report != null)
                {
                    if (report.Employee == "Error")
                    {
                        fullReport.Model = null;
                        return BadRequest(fullReport);
                    }
                }
            }
            return Ok(fullReport);
        }

        [HttpGet("GetEmployeeListForFullReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetEmployeeListForFullReport(int departmentId, string? teamAdminId)
        {
            var employeeList = await _reportsService.GetEmployeeListForFullReport(departmentId, teamAdminId);
            if (employeeList.Model != null && employeeList.Model.Any() && employeeList.Model.FirstOrDefault().Id == "Error")
            {
                employeeList.Model = null;
                return BadRequest(employeeList);
            }
            return Ok(employeeList);
        }

        [HttpGet("GetProjectListForFullReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetProjectListForFullReport(int departmentId, string? teamAdminId)
        {
            var projectList = await _reportsService.GetProjectListForFullReport(departmentId, teamAdminId);
            return Ok(projectList);
        }

        [HttpGet("GetClientListForFullReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetClientListForFullReport(int departmentId, string? teamAdminId)
        {
            var clientList = await _reportsService.GetClientListForFullReport(departmentId, teamAdminId);
            return Ok(clientList);
        }

        [HttpGet("GetMonthlyHoursListForFullReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetMonthlyHoursListForFullReport([FromQuery] MonthlyHourFullReportRequestViewModel model)
        {
            var dateValidation = await SharedResources.ValidateReportDateFilter(model.FromDate, model.ToDate);
            if (!dateValidation.IsValid)
            {
                var response = new ResponseModel<MonthlyHourFullReportViewModel>();
                response.Message.Add(SharedResources.ToDateMustGreater);
                return BadRequest(response);
            }
            model.FromDate = dateValidation.FromDate;
            model.ToDate = dateValidation.ToDate;

            var monthlyHoursFullReportList = await _reportsService.GetMonthlyHoursListForFullReport(model);

            if (monthlyHoursFullReportList.Model != null && monthlyHoursFullReportList.Model.Any() && monthlyHoursFullReportList.Model.First().Month == "N/A")
            {
                monthlyHoursFullReportList.Model = null;
                return BadRequest(monthlyHoursFullReportList);
            }
            if (monthlyHoursFullReportList.Model == null)
            {
                return NotFound(monthlyHoursFullReportList);
            }
            return Ok(monthlyHoursFullReportList);
        }


        [HttpGet("GetWorkInHandReport")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetWorkInHandReport([FromQuery] string? teamAdminId, int departmentId, string? searchText)
        {
            var workInHandReport = await _reportsService.GetWorkInHandReportAsync(teamAdminId, departmentId, searchText);
            if (workInHandReport.Model != null && workInHandReport.Model.Any() && workInHandReport.Model.First().ProjectId == -1)
            {
                workInHandReport.Model = null;
                return BadRequest(workInHandReport);
            }
            else
            {
                var result = workInHandReport.Model.
                GroupBy(p => new { p.ProjectId, p.ProjectName }).
                Select(g => new
                {
                    ProjectId = g.Key.ProjectId,
                    ProjectName = g.Key.ProjectName,
                    Modules = g.ToList()
                }).ToList();
                return Ok(new ResponseModel<dynamic> { Model = result });
            }
        }

      
    }
}