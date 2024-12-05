
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Validators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeeStatusController : ControllerBase
    {
        private readonly IEmployeeStatusServices _employeeStatusServices;
        private readonly IValidator<EmployeeStatusDto> _validator;
        private readonly IUpworkProfileService _upworkProfileService;
        private readonly IProjectService _projectService;
        private readonly IProjectModuleService _projectModuleService;
        public EmployeeStatusController(IEmployeeStatusServices employeeStatusServices,
                                        IValidator<EmployeeStatusDto> validator,
                                        IUpworkProfileService upworkProfileService,
                                        IProjectService projectService,
                                        IProjectModuleService projectModuleService)
        {
            _employeeStatusServices = employeeStatusServices;
            _validator = validator;
            _upworkProfileService = upworkProfileService;
            _projectService = projectService;
            _projectModuleService = projectModuleService;
        }

        [HttpGet("GetEmployeeStatus")]
        [Authorize(Policy = "EmployeeOrTeamLeadOrBDMOrAdmin")]
        public async Task<IActionResult> GetEmployeesStatus([FromQuery] EmployeeStatusPaginationViewModel employeeStatusPaginationViewModel)
        {
            var empStatusList = await _employeeStatusServices.GetEmployeeStatusListAsync(employeeStatusPaginationViewModel);     
            return Ok(empStatusList);
        }

        [HttpPost("AddEmployeeStatus")]
        [Authorize(Policy = "EmployeeOrTeamLeadOrBDMOrAdmin")]
        public async Task<IActionResult> AddEmployeeStatus([FromBody] List<EmployeeStatusDto> employeeStatusList)
        {
            try
            {
                var responseData = new ResponseModel<SuccessFailureResultViewModel<AddEmployeeStatusResponseViewModel>>();
                var assignmentResult = new SuccessFailureResultViewModel<AddEmployeeStatusResponseViewModel>();

                foreach (var status in employeeStatusList)
                {
                    var leaveStatus = await _employeeStatusServices.CheckIfUserAlreadyAppliedLeave(status);
                    if (leaveStatus.Model == 1)
                    {
                        assignmentResult.Success.Add(new AddEmployeeStatusResponseViewModel { Status = status, ErrorMessages = new List<string>(leaveStatus.Message) });
                        continue;
                    }
                    if (leaveStatus.Model == 2)
                    {
                        leaveStatus.Model = 0;
                        assignmentResult.Failure.Add(new AddEmployeeStatusResponseViewModel { Status = status, ErrorMessages = new List<string>(leaveStatus.Message) });
                        continue;
                    }
                    var validationResults = new List<FluentValidation.Results.ValidationResult>();

                    var results = await _validator.ValidateAsync(status);

                    if (!results.IsValid)
                    {
                        var errors = await SharedResources.FluentValidationResponse(results.Errors);
                        assignmentResult.Failure.Add(new AddEmployeeStatusResponseViewModel { Status = status, ErrorMessages = errors.Message });
                        continue;
                    }                  
                    var response = await _employeeStatusServices.AddNewEmployeeStatusAsync(status);
                    if (response.Model == null)
                    {
                        assignmentResult.Failure.Add(new AddEmployeeStatusResponseViewModel { Status = status, ErrorMessages = new List<string>(response.Message) });
                    }
                    else
                    {
                        assignmentResult.Success.Add(new AddEmployeeStatusResponseViewModel { Status = response.Model });
                    }
                }
                responseData.Model = assignmentResult;
                if(assignmentResult.Success.Any())
                {
                    responseData.Message.Add(SharedResources.StatusAddedSuccessfully);
                    return Ok(responseData);
                }
                else
                {
                    responseData.Message.Add(SharedResources.ErrorWhileAddStatus);
                    return BadRequest(responseData);
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, SharedResources.InternalServerError + ex.Message);
            }
        }

        [HttpPut("UpdateEmployeeStatus")]
        [Authorize(Policy = "EmployeeOrTeamLeadOrBDMOrAdmin")]
        public async Task<IActionResult> UpdateEmployeeStatus([FromBody] EmployeeStatusDto editStatus)
        {
            var leaveStatus = await _employeeStatusServices.CheckIfUserAlreadyAppliedLeave(editStatus);
            if (leaveStatus.Model == 1)
                return Ok(leaveStatus);
            if (leaveStatus.Model == 2)
            {
                leaveStatus.Model = 0;
                return BadRequest(leaveStatus);
            }
            var results = await _validator.ValidateAsync(editStatus, (options) =>
            {
                options.IncludeRuleSets(OperationType.Update.ToString(), "default");
            });
            if (!results.IsValid)
                return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
            var result = await _employeeStatusServices.UpdateEmployeeStatusAsync(editStatus);
            if (result.Model == 0)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteEmployeeStatusbyId")]
        [Authorize(Policy = "EmployeeOrTeamLeadOrBDMOrAdmin")]
        public async Task<IActionResult> DeleteById([Required] int id)
        {
            var results = await _validator.ValidateAsync(new EmployeeStatusDto { Id = id }, (options) =>
            {
                options.IncludeRuleSets(OperationType.Delete.ToString());
            });
            if (!results.IsValid)
                return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
            var response = await _employeeStatusServices.DeleteStatusAsync(id);
            if (!response.Model)
            {
                return NotFound(response);
            }
            return Ok(response);

        }

        [HttpDelete("DeleteEmployeeStatus")]
        public async Task<IActionResult> DeleteEmployeeStatus([FromQuery] EmployeeStatusFilterViewModel getAndDeleteEmployeeStatusViewModel)
        {
            var response = await _employeeStatusServices.DeleteEmployeeStatusByIDAsync(getAndDeleteEmployeeStatusViewModel);
            if (!response.Model)
                return BadRequest(response);
            return Ok(response);
        }
        [HttpGet("GetUpworkProfilesListByDepartment")]
        [Authorize]
        public async Task<IActionResult> GetUpworkProfilesByDepartment()
        {
            var response = await _upworkProfileService.GetUpworkProfilesByDepartment();
            return Ok(response);
        }
        [HttpGet("GetProjectsListByDepartment")]
        [Authorize]
        public async Task<IActionResult> GetProjectsByDepartment(int departmentId)
        {
            var response = await _projectService.GetProjectsByDepartment(departmentId);
            return Ok(response);
        }
        [HttpGet("GetProjectModulesListByProject")]
        [Authorize]
        public async Task<IActionResult> GetProjectModulesByProject([Required] int projectId)
        {
            var response = await _employeeStatusServices.GetOpenProjectModulesListByProjectId(projectId);
            return Ok(response);
        }

        [HttpGet("GetEmployeeStatusByManager")]
        [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetEmployeesStatusByManager([Required][Range(1, 12)] int Month, [Required][Range(2000, 2024)] int Year, [Required] string EmployeeId, int departmentId)
        {
            var empStatusList = await _employeeStatusServices.GetEmployeeStatusListByManager(Month, Year, EmployeeId, departmentId);
            return Ok(empStatusList);
        }

    }
}



