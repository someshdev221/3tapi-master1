using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Validators;

namespace TimeTaskTracking.Controller;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    public readonly IEmployeeService _employeeService;
    private readonly IValidator<UpdateEmployeeProfileByManagerViewModel> _validator;
    private readonly IValidator<MonthlyFeedbackFormDto> _validate;
    private readonly IValidator<AddOnRollFeedbackFormDto> _validation;
    public EmployeeController(IEmployeeService employeeService, IValidator<UpdateEmployeeProfileByManagerViewModel> validator, IValidator<MonthlyFeedbackFormDto> validate, IValidator<AddOnRollFeedbackFormDto> validation)
    {
        _employeeService = employeeService;
        _validator = validator;
        _validate = validate;
        _validation = validation;
    }


    //[HttpGet("GetEmployeeById")]
    //[Authorize(Policy = "ManagerOnly")]
    //public async Task<IActionResult> GetEmployeeById([Required] string employeeId, int departmentId)
    //{
    //    var getEmployeeDetails = await _employeeService.GetEmployeesById(employeeId, departmentId);
    //    if (getEmployeeDetails.Model == null)
    //        return NotFound(getEmployeeDetails);
    //    return Ok(getEmployeeDetails);
    //}

    [HttpGet("GetEmployeeProfileDetails")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHROrHOD")]
    public async Task<IActionResult> GetEmployeeProfileDetails([Required] string employeeId, int departmentId)
    {
        var getEmployeeDetails = await _employeeService.GetEmployeeProfileDetails(employeeId, departmentId);
        return Ok(getEmployeeDetails);
    }


    [HttpGet("GetDesignationsList")]
    public async Task<IActionResult> GetAllDesignation([Required] int departmentId)
    {
        var designationList = await _employeeService.GetAllDesignations(departmentId);
        return Ok(designationList);
    }

    [HttpGet("GetProjectManagerOrTeamLeadOrBDMListByDepartment")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetProjectManagerOrTeamLeadOrBDMListByDepartment(int departmentId)
    {
        var responseList = await _employeeService.GetProjectManagerOrTeamLeadOrBDMListByDepartment(departmentId);
        return Ok(responseList);
    }

    [HttpGet("GetDepartmentsList")]
    public async Task<IActionResult> GetAllDepartments()
    {
        var departmentList = await _employeeService.GetAllDepartments();
        return Ok(departmentList);
    }

    [HttpGet("GetManagerList")]
    [Authorize(Policy = "ManagerOrHROrAdminOrHOD")]
    public async Task<IActionResult> GetManagerList(int departmentId)
    {
        var response = await _employeeService.GetManagerList(departmentId);
        return Ok(response);
    }

    [HttpGet("GetAllEmployees")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHROrHOD")]
    public async Task<IActionResult> GetEmployees([FromQuery] EmployeeModel employeeModel)
    {
        var filteredEmployees = await _employeeService.GetEmployeesByFilter(employeeModel);
        return Ok(filteredEmployees);
    }

    [HttpGet("GetAllUsers")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserViewModel model)
    {
        var filteredEmployees = await _employeeService.GetAllUsers(model);
        return Ok(filteredEmployees);
    }


    [HttpPost("AssignAward")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<IActionResult> AssignAward([FromBody] AssignAwardViewModel awardModel)
    {
        AssignAwardValidator validator = new AssignAwardValidator();
        FluentValidation.Results.ValidationResult results = validator.Validate(awardModel);

        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

        var response = await _employeeService.AssignEmployeeAward(awardModel);
        if (response.Model == 0)
            return BadRequest(response);
        return Ok(response);
    }

    //[HttpDelete("DeleteEmployee")]
    //[Authorize(Policy = "ManagerOrHROrAdmin")]
    //public async Task<IActionResult> DeleteEmployee([Required] string employeeId, int departmentId)
    //{
    //    var deleteEmployee = await _employeeService.DeleteEmployeeById(employeeId, departmentId);
    //    if (!deleteEmployee.Model)
    //        return NotFound(deleteEmployee);
    //    return Ok(deleteEmployee);
    //}

    //[HttpPut("UpdateEmployeeProfileStatus")]
    //[Authorize(Policy = "RetrievedSuccessfully")]
    //public async Task<IActionResult> UpdateEmployeeStatus([Required] string employeeId, [Required] int isActive )
    //{
    //    var statusUpdate = await _employeeService.UpdateEmployeeStatusById(employeeId, isActive );
    //    if (statusUpdate.Model == null)
    //        return NotFound(statusUpdate);
    //    return Ok(statusUpdate);
    //}

    [HttpPut("UpdateEmployeeManagerAndStatus")]
    [Authorize(Policy = "ManagerOrHROrAdminOrHOD")]
    public async Task<IActionResult> UpdateEmployeeManagerAndStatus(UpdateManagerandStatusDto updateManagerAndStatusModel)
    {
        var statusUpdate = await _employeeService.UpdateEmployeeManagerAndStatus(updateManagerAndStatusModel);
        if (statusUpdate.Model == null)
        {
            return NotFound(statusUpdate);
        }
        if (statusUpdate.Model != null && statusUpdate.Model == "Failed")
        {
            statusUpdate.Model = null;
            return BadRequest(statusUpdate);
        }   
        return Ok(statusUpdate);
    }

    [HttpPut("UpdateEmployeeProfileDetails")]
    [Authorize(Policy = "ManagerOrHROrAdminOrHOD")]
    public async Task<IActionResult> UpdateEmployeeProfileDetails([FromBody] UpdateEmployeeProfileByManagerViewModel profileModel)
    {
        var results = await _validator.ValidateAsync(profileModel);
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

        var statusUpdate = await _employeeService.UpdateEmployeeProfileByManager(profileModel);
        if (statusUpdate.Model == null)
            return NotFound(statusUpdate);
        return Ok(statusUpdate);
    }


    [HttpGet("GetEmployeeStatusFilter")]
    [Authorize(Policy = "ManagerOrHROrAdminOrHOD")]
    public IActionResult GetEmployeeStatusFilter()
    {
        return Ok(SharedResources.GetEnumData<EmployeeStatusFilter, int>());
    }


    [HttpGet("GetEmployeeStatusList")]
    [Authorize(Policy = "ManagerOrHROrAdminOrHOD")]
    public IActionResult GetEmployeeStatus()
    {
        return Ok(SharedResources.GetEnumData<EmployeeStatusList, int>());
    }

    [HttpGet("GetPerformanceList")]
    [Authorize]
    public IActionResult GetPerformance()
    {
        return Ok(SharedResources.GetEnumData<Performance, int>());
    }

    [HttpGet("GetSalaryList")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public IActionResult GetSalary()
    {
        return Ok(SharedResources.GetEnumData<StartSalary, int>());
    }


    [HttpGet("GetEmployeeListByTeamLead")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<IActionResult> GetEmployeeByTL([Required] string teamLeaderId)
    {
        var getEmployeeDetails = await _employeeService.GetEmployeeByTeamLeader(teamLeaderId);

        return Ok(getEmployeeDetails);
    }

    [HttpDelete("DeleteAssignAwards")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<IActionResult> DeleteAssignAwards([Required] string employeeId, [Required] int UserBadgeId)
    {

        var deleteAssignAwards = await _employeeService.DeleteAssignAwards(employeeId, UserBadgeId);
        if (deleteAssignAwards.Model == false)
        {
            return BadRequest(deleteAssignAwards);
        }
        return Ok(deleteAssignAwards);
    }

    [HttpPost("AddMonthlyFeedback")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<IActionResult> AddMonthlyFeedback([FromBody] MonthlyFeedbackFormDto feedbackFormDto)
    {
        var results = await _validate.ValidateAsync(feedbackFormDto, (options) =>
        {
            options.IncludeRuleSets(OperationType.Create.ToString(), "default");
        }); ;
        if (!results.IsValid)
        {
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        }

        var response = await _employeeService.AddMonthlyFeedbackForm(feedbackFormDto);
        if (response.Model == 0)
            return NotFound(response);
        return Ok(response);
    }

    [HttpGet("GetMonthlyTraineeFeedback")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
    public async Task<IActionResult> GetMonthlyTraineeFeedback([Required] string employeeId, int departmentId)
    {
        var getTraineeDetails = await _employeeService.GetMonthlyTraineeFeedback(employeeId, departmentId);
        return Ok(getTraineeDetails);
    }

    [HttpPut("UpdateTraineeMonthlyFeedback")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<IActionResult> UpdateTraineeMonthlyFeedback([FromBody] MonthlyFeedbackFormDto feedbackFormDto)
    {
        var results = await _validate.ValidateAsync(feedbackFormDto, (options) =>
        {
            options.IncludeRuleSets(OperationType.Update.ToString(), "default");
        });
        if (!results.IsValid)
        {
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        }
        var updateMonthlyFeedback = await _employeeService.UpdateMonthlyFeedbackForm(feedbackFormDto);

        if (updateMonthlyFeedback.Model == 0)
        {
            return NotFound(updateMonthlyFeedback);
        }
            
        if (updateMonthlyFeedback.Model == -1)
        {
            updateMonthlyFeedback.Model = 0;
            return BadRequest(updateMonthlyFeedback);
        }
        return Ok(updateMonthlyFeedback);
    }

    [HttpDelete("DeleteFeedbackForm")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<IActionResult> DeleteFeedbackForm([Required] int feedBackId)
    {

        var deleteFeedbackForm = await _employeeService.DeleteTraineeFeedback(feedBackId);
        if (deleteFeedbackForm.Model == false)
        {
            return NotFound(deleteFeedbackForm);
        }
        return Ok(deleteFeedbackForm);
    }

    [HttpPost("AddOnRollFeedbackForm")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<IActionResult> AddOnRollFeedbackForm([FromBody] AddOnRollFeedbackFormDto addOnRollFeedbackForm)
    {

        var results = await _validation.ValidateAsync(addOnRollFeedbackForm, (options) =>
        {
            options.IncludeRuleSets(OperationType.Create.ToString(), "default");
        }); ;
        if (!results.IsValid)
        {
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        }
        var response = await _employeeService.AddOnRollFeedbackForm(addOnRollFeedbackForm);
        if (response.Model == 0)
            return NotFound(response);
        return Ok(response);
    }

    [HttpPut("UpdateOnRollFeedback")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<IActionResult> UpdateOnRollFeedback([FromBody] AddOnRollFeedbackFormDto feedbackFormDto)
    {
        var results = await _validation.ValidateAsync(feedbackFormDto, (options) =>
        {
            options.IncludeRuleSets(OperationType.Update.ToString(), "default");
        });
        if (!results.IsValid)
        {
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        }
        var updateOnRollFeedback = await _employeeService.UpdateOnRollFeedbackForm(feedbackFormDto);
        if (updateOnRollFeedback.Model == 0)
            return NotFound(updateOnRollFeedback);
        return Ok(updateOnRollFeedback);
    }

    [HttpGet("GetAwardsList")]
    [Authorize(Policy = "TeamLeadOrManagerOrAdminOrHOD")]
    public async Task<ActionResult<List<UserBadgeDto>>> GetAllUserBadges()
    {
        {
            var userBadges = await _employeeService.GetAllAwardsAsync();
            return Ok(userBadges);
        }
    }
    [Authorize(Policy = "ManagerOrHROrAdminOrHOD")]
    [HttpGet("GetListOfNewRequest")]
    public async Task<IActionResult> GetListOfNewRequest([FromQuery] string? searchText, int depatmentId)
    {
        try
        {
            var employees = await _employeeService.GetListOfNewRequestAsync(searchText, depatmentId);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetMonthlyTraineeFeedbackById")]
    [Authorize]
    public async Task<IActionResult> GetMonthlyTraineeFeedbackById([Required] int feedbackId, [Required] string employeeId)
    {
        var getTraineeDetails = await _employeeService.GetMonthlyTraineeFeedbackById(feedbackId, employeeId);
        return Ok(getTraineeDetails);
    }

    [HttpPut("RemoveEmployeeByManager")]
    [Authorize(Policy = "ManagerOrAdminOrHOD")]//set authorize policy
    public async Task<IActionResult> RemoveEmployeeByManager([FromBody] RemoveEmployeeManagerViewModel model)
    {
        var result = await _employeeService.RemoveEmployeeByManagerFromManagerTeamList(model.EmployeeId);
        if (result.Model == "Error")
        {
            result.Model = null;
            return BadRequest(result);
        }
        if (result.Model == "Not Found")
        {
            result.Model = null;
            return NotFound(result);
        }
        return Ok(result);
    }
    [HttpGet("GetSalesPersonsList")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetSalesPersonsList([FromQuery] int departmentId)
    {
        var response = new ResponseModel<dynamic>();
        var salesPersons = await _employeeService.GetSalesPersonsList(departmentId);
        if (salesPersons != null)
        {
            var salesPersonByDepartment = salesPersons.Model
                .GroupBy(p => new { p.DepartmentName, p.DepartmentId })
                .Select(g => new
                {
                    DepartmentName = g.Key.DepartmentName,
                    DepartmentId = g.Key.DepartmentId,
                    SalesPersons = g.Select(p => new
                    {
                        p.Id,
                        p.Name
                    }).ToList()
                })
                .ToList();

            response.Model = salesPersonByDepartment;
        }
        return Ok(response);
    }
    [HttpGet("GetProjectManagerOrTeamLeadOrBDMListByDepartments")]
    [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetProjectManagerOrTeamLeadOrBDMListByDepartments([FromQuery] List<int> departmentId)
    {
        var responseList = await _employeeService.GetProjectManagerOrTeamLeadOrBDMListByDepartments(departmentId);
        return Ok(responseList);
    }
}