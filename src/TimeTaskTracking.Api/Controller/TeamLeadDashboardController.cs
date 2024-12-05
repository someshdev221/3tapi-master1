using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public class TeamLeadDashboardController : ControllerBase
    {
        public readonly IDashboardService _dashboardService;
        public TeamLeadDashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("GetTeamsProductivity")]
        public async Task<IActionResult> GetTeamsProductivity([FromQuery] TeamLeadProductivityViewModel model)
        {
            var productivityModel = await _dashboardService.GetTeamProductivity(model);
            if (productivityModel.Model == null)
            {
                return BadRequest(productivityModel);
            }
            if (productivityModel.Model != null && productivityModel.Model.TeamLeadId == "ERROR")
            {
                productivityModel.Model = null;
                return NotFound(productivityModel);
            }

            if (productivityModel.Model != null && productivityModel.Model.TeamLeadId == "BadRequest")
            {
                productivityModel.Model = null;
                return BadRequest(productivityModel);
            }
            return Ok(productivityModel);
        }
        [HttpGet("GetTeamPerformanceDetail")]
        public async Task<IActionResult> TeamPerformanceDetail([FromQuery] DateFilterViewModel model, string? TeamLeadId)
        {
            if (ModelState.IsValid)
            {
                var performanceDetail = await _dashboardService.PerformanceDetails(model, TeamLeadId);

                if (performanceDetail.Model != null && performanceDetail.Model.Any() && performanceDetail.Model.First().EmployeeId == "ERROR")
                {
                    performanceDetail.Model = null;
                    return BadRequest(performanceDetail);
                }
                //if (performanceDetail.Model == null || !performanceDetail.Model.Any())
                //{
                //    return NotFound(performanceDetail);
                //}
                return Ok(performanceDetail);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetEmployeeProjectBillingDetailsByModule")]
        public async Task<IActionResult> GetEmployeeProjectBillingDetailsModuleWise([FromQuery] EmployeeRequestViewModel model, int departmentId)
        {
            if (ModelState.IsValid)
            {
                var productivityModel = await _dashboardService.EmployeeProjectBillingByModule(model, departmentId);

                if (productivityModel.Model != null && productivityModel.Model.Any() && productivityModel.Model.First().ProjectId == -1)
                {
                    productivityModel.Model = null;
                    return BadRequest(productivityModel);
                }
                //if (productivityModel.Model == null || !productivityModel.Model.Any())
                //{
                //    return NotFound(productivityModel);
                //}               
                return Ok(productivityModel);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetTeamLeadTotalProjectsDetail")]
        public async Task<IActionResult> ProjectDetailsByTeamLead([FromQuery] DateFilterViewModel model, string? TeamLeadId, int departmentId)
        {

            if (ModelState.IsValid)
            {
                var projectDetail = await _dashboardService.ProjectDetails(model, TeamLeadId, departmentId);

                if (projectDetail.Model != null && projectDetail.Model.Any() && projectDetail.Model.First().ProjectId == -1)
                {
                    projectDetail.Model = null;
                }
                return Ok(projectDetail);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetProjectModuleBillingDetailsByProjectID")]
        public async Task<IActionResult> GetModuleBillingDetailsByTeamLead([FromQuery] ModuleBillingRequestViewModel model, int departmentId)
        {
            if (ModelState.IsValid)
            {
                // Check if ModuleStatus is valid
                var response = new ResponseModel<CompleteModuleDetailsViewModel>();

                if (model.ModuleStatus != null)
                {
                    if (!Enum.TryParse(typeof(ModuleStatus), model.ModuleStatus, true, out var _))
                    {
                        response.Message.Add(SharedResources.ModuleStatus);
                        return BadRequest(response);
                    }
                }
                // Check if PaymentStatus is valid
                if (model.PaymentStatus != null)
                {
                    if (!Enum.TryParse(typeof(PaymentStatus), model.PaymentStatus, true, out var _))
                    {
                        response.Message.Add(SharedResources.ModuleStatus);
                        return BadRequest(response);
                    }
                }
                var moduleDetail = await _dashboardService.ModuleBillingDetails(model, departmentId);

                if (moduleDetail.Model != null && moduleDetail.Model.Any() && moduleDetail.Model.First().ModuleId == "ERROR")
                {
                    moduleDetail.Model = null;
                    return BadRequest(moduleDetail);
                }
                //if (moduleDetail.Model == null || !moduleDetail.Model.Any())
                //{
                //    return NotFound(moduleDetail);
                //}
                return Ok(moduleDetail);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetDeveloperProgressReport")]
        public async Task<IActionResult> GetProjectDetailsModuleAndEmployeeWiseByTeamLead([FromQuery] ProjectRequestViewModel model, int departmentId)
        {

            if (ModelState.IsValid)
            {
                var moduleDetail = await _dashboardService.GetProjectDetailsModuleAndEmployeeWiseByTeamLead(model, departmentId);

                if (moduleDetail.Model != null && moduleDetail.Model.Any() && moduleDetail.Model.First().EmployeeId == "ERROR")
                {
                    moduleDetail.Model = null;
                    return BadRequest(moduleDetail);
                }
                //if (moduleDetail.Model == null || !moduleDetail.Model.Any())
                //{
                //    return NotFound(moduleDetail);

                //}
                return Ok(moduleDetail);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetEmployeePerformanceDetails")]
        public async Task<IActionResult> GetEmployeePerformanceDetails([FromQuery] TeamLeadRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var teamLeadPerfoamceDetails = await _dashboardService.TeamLeadPerformaceDetails(model);
                return Ok(teamLeadPerfoamceDetails);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetTeamAttendance")]
        public async Task<IActionResult> GetEmployeeAttendanceByTeamLead([FromQuery] TeamLeadRequestViewModel model)
        {

            if (ModelState.IsValid)
            {
                var attendanceDetails = await _dashboardService.TeamAttendanceDetails(model);
                if (attendanceDetails.Model != null && attendanceDetails.Model.Any() && attendanceDetails.Model.First().EmployeeName == "ERROR")
                {
                    attendanceDetails.Model = null;
                    return BadRequest(attendanceDetails);
                }
                return Ok(attendanceDetails);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetTeamLeadBadges")]
        public async Task<IActionResult> GetTeamLeadBadges()
        {
            if (ModelState.IsValid)
            {
                var attendanceDetails = await _dashboardService.GetAssignedBadges();
                return Ok(attendanceDetails);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetTraineeList")]
        public async Task<IActionResult> GetTraineeList()
        {
            if (ModelState.IsValid)
            {
                var traineeList = await _dashboardService.GetTraineeListAsync();
                return Ok(traineeList);
            }
            return BadRequest(ModelState);
        }
    }
}
