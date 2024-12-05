using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Dtos;
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

public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IValidator<ProjectDto> _validator;
    public ProjectController(IProjectService projectService, IValidator<ProjectDto> validator)
    {
        _projectService = projectService;
        _validator = validator;
    }

    [HttpGet("GetAllProjects")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetAllProjects([FromQuery] ProjectFilterViewModel projectFilterViewModel)
    {
        var project = await _projectService.GetAllProjects(projectFilterViewModel);
        return Ok(project);
    }

    [HttpGet("GetListOfOpenAssignedProjectsByTeamLeadId")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetListOfOpenAssignedProjectsByTeamLeadId([FromQuery][Required] string teamLeadId, [Required] int departmentId)
    {
        var projectList = await _projectService.GetListOfOpenAssignedProjectsToTeamLead(teamLeadId, departmentId);
        if (projectList.Model == null)
        {
            return NotFound(projectList);
        }

        if (projectList.Model != null && projectList.Model.Any(item => item.Id == "ERROR"))
        {
            projectList.Model = null;
            return BadRequest(projectList);
        }
        return Ok(projectList);
    }

    [HttpGet("GetListOfProjectsAssignedToTeamLead")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetListOfProjectsAssignedToTeamLead([FromQuery] TeamLeadProjectListViewModel model)
    {
        var project = await _projectService.GetListOfProjectsAssignedToTeamLead(model);
        return Ok(project);
    }


    [HttpGet("GetApplicationDomain")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetApplicationDomain()
    {
        var result = await _projectService.GetApplicationDomainsAsync();
        return Ok(result);
    }


    [HttpGet("GetProjectByID")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetProjectByID([Required] int id, int departmentId)
    {
        var response = await _projectService.GetProject(id, departmentId);
        return Ok(response);
    }

    [HttpGet("GetProjectBillingHistory")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetProjectBillingHistory([Required] int projectId)
    {
        var response = await _projectService.GetProjectBillingHistory(projectId);
        if (response.Model == null)
        {
            return BadRequest(response);
        }
        //if (response.Model != null && response.Model.Any() && response.Model.First().BillingHours == 0)
        //{
        //    response.Model = null;
        //    return NotFound(response);
        //}
        return Ok(response);
    }

    [HttpPost("AddProject")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> AddProject([FromBody] AddAssignProjectDto projectDto)
    {
        projectDto.Id = 0;
        var results = await _validator.ValidateAsync(projectDto);
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

        var updatedProject = await _projectService.AddProject(projectDto);
        if (updatedProject.Model == 0)
            return StatusCode(500, updatedProject);
        return Ok(updatedProject);
    }

    [HttpPut("UpdateProject")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> UpdateProject([FromBody] AddAssignProjectDto projectDto)
    {
        var results = await _validator.ValidateAsync(projectDto, (options) =>
        {
            options.IncludeRuleSets(OperationType.Update.ToString(), "default");
        });
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

        var updatedProject = await _projectService.UpdateProject(projectDto);
        if (updatedProject.Model == 0)
            return StatusCode(500, updatedProject);
        return Ok(updatedProject);
    }

    [HttpPut("UpdateProjectStatus")]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> UpdateProjectStatus([FromBody] UpdateProjectStatusViewModel model)
    {
        if (model.ProjectId <= 0)
        {
            var response = new ResponseModel<int>
            {
                Message = new List<string> {SharedResources.ProjectIdMustBeGreaterThan0},
                Model = 0 
            };
            return BadRequest(response);
        }
        else
        {
            var updatedProjectStatus = await _projectService.UpdateProjectStatus(model);
            if (updatedProjectStatus.Model == -1)
            {
                updatedProjectStatus.Model = 0;
                return BadRequest(updatedProjectStatus);
            }
            if (updatedProjectStatus.Model == 0)
            {
                updatedProjectStatus.Model = 0;
                return NotFound(updatedProjectStatus);

            }

            if (updatedProjectStatus.Model == 2)
            {
                updatedProjectStatus.Model = 0;
                return StatusCode(500, updatedProjectStatus);
            }
            return Ok(updatedProjectStatus);
        }
    }

    [HttpDelete("DeleteProject")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var results = await _validator.ValidateAsync(new ProjectDto { Id = id }, (options) =>
        {
            options.IncludeRuleSets(OperationType.Delete.ToString());
        });
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        var response = await _projectService.DeleteProject(id);
        if (!response.Model)
            return StatusCode(500, response);
        return Ok(response);
    }

    //[HttpGet("GetAllProjectsByTL")]
    //[Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdmin")]
    //public async Task<IActionResult> GetAllProjectsByTL([Required]string teamLeadId)
    //{
    //    var response = await _projectService.GetAllProjectsByTL(teamLeadId);
    //    if (response.Model == null)
    //        return NotFound(response);
    //    return Ok(response);
    //}

    [AllowAnonymous]
    [HttpGet("GetHiringStatusList")]
    public IActionResult GetHiringStatus()
    {
        return Ok(SharedResources.GetEnumData<HiringStatus, int>());
    }

    [HttpGet("GetHiringTypeFilter")]
    public IActionResult GetHiringTypeStatuses()
    {
        return Ok(SharedResources.GetEnumData<HiringTypeFilter, int>());
    }

    [AllowAnonymous]
    [HttpGet("GetBillingTypeList")]
    public IActionResult GetBilingType()
    {
        return Ok(SharedResources.GetEnumData<BillingType, int>());
    }

    [HttpGet("GetBilingTypeFilter")]
    public IActionResult GetBilingTypeStatuses()
    {
        return Ok(SharedResources.GetEnumData<BillingTypeStatus, int>());
    }

    [AllowAnonymous]
    [HttpGet("GeProjectStatusFilter")]
    public IActionResult GetProjectStatuses()
    {
        return Ok(SharedResources.GetEnumData<ProjectStatusFilter, int>());
    }

    [AllowAnonymous]
    [HttpGet("GetProjectStatusList")]
    public IActionResult GetProjectStatus()
    {
        return Ok(SharedResources.GetEnumData<ProjectStatus, int>());
    }

    [HttpGet("GetProjectListByTeamLead")]
    [Authorize(Policy = "TeamLeadOrAdmin")]
    public async Task<IActionResult> GetProjectByTeamLead([Required] string teamLeaderId, int departmentId)
    {
        var getEmployeeDetails = await _projectService.GetProjectByTeamLeader(teamLeaderId, departmentId);
        return Ok(getEmployeeDetails);
    }

    [HttpPost("UploadDocument")]
    public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadDto documentUploadDto)
    {
        var response = await _projectService.UploadDocumentAsync(documentUploadDto);

        if (response.Model == null)
            return NotFound(response);

        return Ok(response);
    }


    [HttpGet("GetUploadedDocuments")]
    public async Task<IActionResult> GetUploadedDocuments(int projectId)
    {
        var response = await _projectService.GetUploadedDocumentsAsync(projectId);
        return Ok(response);
    }

    [HttpGet("GetProjectProductivity")]
    public async Task<IActionResult> GetProjectProductivityByProjectId([Required] int projectId, int departmentId)
    {
        var result = await _projectService.GetProjectProductivityByProjectIdAsync(projectId, departmentId);
        return Ok(result);
    }

}
