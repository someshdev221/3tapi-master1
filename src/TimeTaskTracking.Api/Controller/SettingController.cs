using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Validators;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class SettingController : ControllerBase
    {
        private readonly ISettingService _settingService;
        private readonly IValidator<AssignBadgeToEmployeesViewModel> _validator;
        public SettingController(ISettingService settingService, IValidator<AssignBadgeToEmployeesViewModel> validator)
        {
            _settingService = settingService;
            _validator = validator;
        }
        //[HttpGet("GetUsersByRole")]
        //public async Task<IActionResult> GetUsersByRole(int departmentId, [Required]string roleId, string? searchKeyword)
        //{
        //    var getUserByRole = await _settingService.GetUsersByRole(departmentId, roleId, searchKeyword);
        //    return Ok(getUserByRole);
        //}
        [HttpPost("AddTeamMemberToTeam")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> AddTeamMemberToTeam(TeamAssignmentViewModel<List<string>> model)
        {
            var teamMemberDetail = await _settingService.AddTeamMemberToTeam(model);
            if (teamMemberDetail.Model == null || teamMemberDetail.Model.Failure != null && teamMemberDetail.Model.Failure.Any())
                return BadRequest(teamMemberDetail);
            return Ok(teamMemberDetail);
        }

        [HttpGet("GetTeamMembersInProject")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> GetTeamMembersInProject([Required] int projectId, int departmentId)
        {
            var teamMembersDetail = await _settingService.GetProjectMembersByProjectIdAsync(projectId, departmentId);
            return Ok(teamMembersDetail);
        }

        [HttpPost("AddTeamMembersInProject")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> AddTeamMemberInProject(ProjectAssignmentViewModel<List<string>, List<int>> projectAssignmentViewModel)
        {
            var projectAssignmentDetail = await _settingService.AddTeamMemberInProject(projectAssignmentViewModel);
            if (projectAssignmentDetail.Model == null || projectAssignmentDetail.Model.Failure != null && projectAssignmentDetail.Model.Failure.Any())
                return BadRequest(projectAssignmentDetail);
            return Ok(projectAssignmentDetail);
        }

        [HttpGet("GetAssignedUsersByEmployeeId")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetAssignedUserByName([Required] string employeeId)
        {
            var getEmployeeDetails = await _settingService.GetAssignedUsersByName(employeeId);          
            return Ok(getEmployeeDetails);
        }

        [HttpGet("GetEmployeeListByDepartmentId")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetEmployeeByDepartmentId(string? teamAdminId, int departmentId)
        {
            var getEmployeeDepartment = await _settingService.GetEmployeeByDepartmentIdAsync(teamAdminId,departmentId);        
            return Ok(getEmployeeDepartment);
        }

        [HttpGet("GetTeamLeadAndBDMListByDepartment")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> GetTeamLeadBAAndBDListByDepartmentId(string? teamAdminId, [Required] int departmentId)
        {
            var getEmployeeDepartment = await _settingService.GetTeamLeadBAAndBDListByDepartmentId(teamAdminId, departmentId);    
            return Ok(getEmployeeDepartment);
        }

        [HttpPost("AssignBadgeToEmployees")]
        [Authorize(Policy = "ManagerOrAdminOrHOD")]
        public async Task<IActionResult> AssignAwardsToEmployeesWhoseProductivityIsHigh([FromBody] AssignBadgeToEmployeesViewModel model)
        {
            AssignBadgeValidator validator = new AssignBadgeValidator();
            FluentValidation.Results.ValidationResult results = validator.Validate(model);

            if (!results.IsValid)
                return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

            var getProductiveHourData = await _settingService.GetProductiveHoursByDepartmentIdAsync(model);
            if (getProductiveHourData.Model == null)
                return NotFound(getProductiveHourData);
            return Ok(getProductiveHourData);
        }

        [HttpPut("UpdateCanEditStatus")]
        [Authorize(Policy = "AdminOrHOD")]
        public async Task<IActionResult> UpdateCanEditStatus([FromBody] UpdateCanEditStatusViewModel updateCanEditStatus)
        {
            var updatedEditStatus = await _settingService.UpdateCanEditStatusByTeamAdminIdOrDepartmentId(updateCanEditStatus);
            if (!updatedEditStatus.Model)
                return NotFound(updatedEditStatus);
            return Ok(updatedEditStatus);
        }
    }
}