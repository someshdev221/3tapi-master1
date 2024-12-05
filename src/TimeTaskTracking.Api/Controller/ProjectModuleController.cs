using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Controller;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
public class ProjectModuleController : ControllerBase
{
    private readonly IProjectModuleService _projectModuleService;
    private readonly IValidator<ProjectModuleDto> _validator;
    public ProjectModuleController(IProjectModuleService projectModuleService, IValidator<ProjectModuleDto> validator)
    {
        _projectModuleService = projectModuleService;
        _validator = validator;
    }

    //[HttpGet("GetProjectDetailsById")]
    //public async Task<IActionResult> GetProjectDetailsById([FromQuery]ProjectDetailFilterViewModel projectDetailFilterViewModel)
    //{
    //    var response = await _projectModuleService.GetProjectDetailsById(projectDetailFilterViewModel);
    //    if(response.Model == null)
    //        return NotFound(response);
    //    return Ok(response);
    //}

    [HttpGet("GetProjectBasicDetails")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetProjectDetailsById([FromQuery] [Required] int projectId, int departmentId)
    {
        var response = await _projectModuleService.GetProjectBasicDetails(projectId, departmentId);
        return Ok(response);
    }

    [HttpGet("GetProjectModuleDetailsByProjectId")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetProjectModuleDetailsByProjectId([FromQuery] ProjectModuleDetailsViewModel projectModuleModel)
    {
        var response = await _projectModuleService.GetProjectModuleDetailsByProjectId(projectModuleModel);
        return Ok(response);
    }


    [HttpGet("GetProjectBillingDetailsByProjectId")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetProjectBillingDetailsByProjectId([FromQuery] ProjectBillingDetailsViewModel projectBillingModel)
    {
        var response = await _projectModuleService.GetProjectBillingDetailsByProjectId(projectBillingModel);
        return Ok(response);
    }

    [HttpGet("GetProjectEmployeeStatusByProjectId")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetProjectEmployeeStatusByProjectId([FromQuery] ProjectBillingDetailsViewModel projectEmplyeeDetailsModel)
    {
        var response = await _projectModuleService.GetProjectEmployeeDetailsByProjectId(projectEmplyeeDetailsModel);
        return Ok(response);
    }

    //[HttpGet("GetProjectTeamMembersByProjectId")]
    //public async Task<IActionResult> GetProjectAssignedTeamMembersByProjectId([FromQuery] [Required] int projectId)
    //{
    //    var response = await _projectModuleService.GetProjectTeamMembersByProjectId(projectId);
    //    if (response.Model == null)
    //        return NotFound(response);
    //    return Ok(response);
    //}


    //[HttpGet("GetProjectModules")]
    //public async Task<IActionResult> GetProjectModules([Required]int projectId, string? status,int departmentId)
    //{
    //    var projectModule = await _projectModuleService.GetProjectModules(projectId, status, departmentId);
    //    if (projectModule.Model?.Any() != true)
    //        return NotFound(projectModule);
    //    return Ok(projectModule);
    //}

    [HttpGet("GetProjectModule")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
    public async Task<IActionResult> GetProjectModule([Required]string moduleId, int departmentId)
    {
        var projectModule = await _projectModuleService.GetProjectModule(moduleId, departmentId);
        return Ok(projectModule);
    }

    [HttpPut("UpdateProjectModule")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> UpdateProjectModule([FromBody]ProjectModuleDto projectModuleDto)
    {
        var results = await _validator.ValidateAsync(projectModuleDto, (options) =>
        {
            options.IncludeRuleSets(OperationType.Update.ToString(), "default");
        });
        if (!results.IsValid)
        {
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        }
        var result = await _projectModuleService.UpdateProjectModule(projectModuleDto);
        if (result.Model == null)
            return StatusCode(500,result);
        if(result.Model == SharedResources.UnAuthorizedAccess)
        {
            result.Model = null;
            return BadRequest (result);
        }
        return Ok(result);
    }

    [HttpPut("UpdateProjectModulePaymentAndModuleStatus")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> UpdateProjectModulePaymentAndModuleStatus([FromBody] UpdateModuleStatusViewModel model)
    {
        var response = new ResponseModel<string>();
        if (string.IsNullOrEmpty(model.ModuleId))
        {
            response.Message.Add(SharedResources.ModuleIdRequired);
            return BadRequest(response);
        }

        if (!string.IsNullOrEmpty(model.PaymentStatus) && !string.IsNullOrEmpty(model.ModuleStatus))
        {
            response.Message.Add(SharedResources.EitherModuleOrPaymentStatusCanBeUpdated);
            return BadRequest(response);
        }

        if (string.IsNullOrEmpty(model.PaymentStatus) && string.IsNullOrEmpty(model.ModuleStatus))
        {
            response.Message.Add(SharedResources.ModuleOrPaymentStatusIsRequired);
            return BadRequest(response);
        }

        if (!string.IsNullOrEmpty(model.PaymentStatus))
        {
            if (!Enum.TryParse(typeof(PaymentStatus), model.PaymentStatus, true, out var parsedPaymentStatus) || !Enum.IsDefined(typeof(PaymentStatus), parsedPaymentStatus))
            {
                response.Message.Add(SharedResources.PaymentStatus);
                return BadRequest(response);
            }
        }
        
        if (!string.IsNullOrEmpty(model.ModuleStatus))
        {
            if (!Enum.TryParse(typeof(ModuleStatus), model.ModuleStatus, true, out var parsedModuleStatus) || !Enum.IsDefined(typeof(ModuleStatus), parsedModuleStatus))
            {
                response.Message.Add(SharedResources.ModuleStatus);
                return BadRequest(response);
            }
        }
 
        var result = await _projectModuleService.UpdateProjectModulePaymentAndModuleStatus(model);

        if (result.Model == null)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpDelete("DeleteProjectModule")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> DeleteProjectModule(string moduleId)
    {
        var results = await _validator.ValidateAsync(new ProjectModuleDto { Id = moduleId }, (options) =>
        {
            options.IncludeRuleSets(OperationType.Delete.ToString());
        });
        if (!results.IsValid)
        {
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        }
        var result = await _projectModuleService.DeleteProjectModule(moduleId);
        if (!result.Model)
            return StatusCode(500, result);
        return Ok(result);
    }

    [HttpPost("AddProjectModule")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> AddProjectModule([FromBody] ProjectModuleDto projectModuleDto)
    {
        var results = await _validator.ValidateAsync(projectModuleDto, (options) =>
        {
            options.IncludeRuleSets(OperationType.Create.ToString(), "default");
        });
        if (!results.IsValid)
        {
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        }
        var result = await _projectModuleService.AddProjectModule(projectModuleDto);
        if (result.Model == null)
            return StatusCode(500, result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("GetPaymentStatusList")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public IActionResult GetPaymentStatus()
    {
        return Ok(SharedResources.GetEnumData<PaymentStatus, string>());
    }

    [AllowAnonymous]
    [HttpGet("GetModuleStatusList")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public IActionResult GetModuleStatus()
    {
        return Ok(SharedResources.GetEnumData<ModuleStatus, string>());
    }
}
