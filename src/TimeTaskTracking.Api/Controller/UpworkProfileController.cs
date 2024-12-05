
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
public class UpworkProfileController : ControllerBase
{
    private readonly IUpworkProfileService _upworkProfileService;
    private readonly IValidator<UpworkDetailsProfileDto> _validator;
    public UpworkProfileController(IUpworkProfileService upworkProfileService, IValidator<UpworkDetailsProfileDto> validator)
    {
        _upworkProfileService = upworkProfileService;
        _validator = validator;
    }
    /// <summary>
    /// Get Filterd Upwork Profile
    /// </summary>
    /// <param name="filterModel"></param>
    /// <returns></returns>
    [HttpGet("GetAllUpworkProfiles")]
    public async Task<IActionResult> GetUpworkProfiles([FromQuery] FilterViewModel filterModel, int ProfileType)
    {
        var response = await _upworkProfileService.GetUpworkProfiles(filterModel, ProfileType);     
        return Ok(response);
    }

    /// <summary>
    /// GetUpworkProfile by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetUpworkProfileById")]
    public async Task<IActionResult> GetProfileById([Required] int id)
    {
        var response = await _upworkProfileService.GetUpworkProfileById(id);        
        return Ok(response);
    }

    [HttpGet("GetProfileTypeList")]
    public IActionResult GetProfileType()
    {
        return Ok(SharedResources.GetEnumData<ProfileType, int>());
    }

    /// <summary>
    /// Add UpworkProfile
    /// </summary>
    /// <param name="upworkProfile"></param>
    /// <returns></returns>
    [HttpPost("AddUpworkProfile")]
    public async Task<IActionResult> AddUpworkProfile([FromBody] UpworkDetailsProfileDto upworkProfile)
    {
        var results = await _validator.ValidateAsync(upworkProfile, (options) =>
        {
            options.IncludeRuleSets(OperationType.Create.ToString(), "default");
        });
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        var response = await _upworkProfileService.AddUpworkProfile(upworkProfile);
        if (response.Model == 0)
            return StatusCode(500, response);
        return Ok(response);
    }

    /// <summary>
    /// UpdateUpworkProfile
    /// </summary>
    /// <param name="upworkProfile"></param>
    /// <returns></returns>
    [HttpPut("UpdateUpworkProfile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpworkDetailsProfileDto upworkProfile)
    {
        var results = await _validator.ValidateAsync(upworkProfile, (options) =>
        {
            options.IncludeRuleSets(OperationType.Update.ToString(), "default");
        });
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        var response = await _upworkProfileService.UpdateUpworkProfile(upworkProfile);
        if (response.Model == 0)
            return StatusCode(500, response);
        return Ok(response);
    }

    /// <summary>
    /// Delete UpworkProfile
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    [HttpDelete("DeleteUpworkProfile")]
    public async Task<IActionResult> DeleteProfile(int id)
    {
        var results = await _validator.ValidateAsync(new UpworkDetailsProfileDto { Id = id }, (options) =>
        {
            options.IncludeRuleSets(OperationType.Delete.ToString());
        });
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
        var response = await _upworkProfileService.DeleteUpworkProfile(id);
        if (!response.Model)
            return StatusCode(500, response);
        return Ok(response);
    }
}
