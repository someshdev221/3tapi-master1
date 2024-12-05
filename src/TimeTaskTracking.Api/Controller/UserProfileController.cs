using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Validators;

namespace TimeTaskTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IValidator<UpdateUserEmailViewModel> _validator;
        public UserProfileController(IUserProfileService userProfileService, IValidator<UpdateUserEmailViewModel> validator)
        {
            _userProfileService = userProfileService;
            _validator = validator;
        }

        //[HttpGet("GetUserProfileDetail")]
        //public async Task<IActionResult> GetUserProfileDetail([Required] string userProfileID)
        //{
        //    var response = await _userProfileService.GetUserProfileDetailAsync(userProfileID);
        //    if (response.Model == null)
        //        return NotFound(response);
        //    return Ok(response);
        //}

        [HttpGet("GetUserDetails")]
        [Authorize]
        public async Task<IActionResult> GetUserDetail()
        {

            var claims = SharedResources.GetUserIdFromToken(Request);

            if (string.IsNullOrEmpty(claims.UserId))
            {
                return BadRequest(SharedResources.UserNotFound);
            }
            var response = await _userProfileService.GetAllUserProfileAsync(claims.UserId);           
            return Ok(response);
        }

        [HttpGet("GetUserProjects")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> GetUserProjects()
        {
            var claims = SharedResources.GetUserIdFromToken(Request);

            if (string.IsNullOrEmpty(claims.UserId))
            {
                return BadRequest(SharedResources.UserNotFound);
            }
            var response = await _userProfileService.GetUserProjectsAsync(claims.UserId);       
            return Ok(response);
        }

        [HttpGet("GetUserTools")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> GetUserTools()
        {
            var claims = SharedResources.GetUserIdFromToken(Request);

            if (string.IsNullOrEmpty(claims.UserId))
            {
                return BadRequest(SharedResources.UserNotFound);
            }

            var response = await _userProfileService.GetUserToolsAsync(claims.UserId);
            return Ok(response);
        }

        //Getalluserprofiledetails
        [HttpGet("GetUserProfileById")]
        [Authorize]
        public async Task<IActionResult> GetUserProfileById()
        {
            var claims = SharedResources.GetUserIdFromToken(Request);

            if (string.IsNullOrEmpty(claims.UserId))
            {
                return BadRequest(SharedResources.UserNotFound);
            }
            var response = await _userProfileService.GetUserProfileByIdAsync(claims.UserId);         
            return Ok(response);
        }

        [HttpGet("GetUserProjectsById")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> GetUserProjectsByUserId(int id)
        {
            var response = await _userProfileService.GetUserProjectByUserIdAsync(id);    
            return Ok(response);
        }

     

        [HttpGet("GetUserToolsById")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> GetUserToolsById(int id)
        {
            var response = await _userProfileService.GetUserToolByUserIdAsync(id);
            return Ok(response); 
        }

        [HttpPut("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileAsync([FromForm] UpdateUserProfileDto userProfileDto)
        {
            UserProfileValidator validator = new UserProfileValidator();
            FluentValidation.Results.ValidationResult result = validator.Validate(userProfileDto);
            if (!result.IsValid)
            {
                return BadRequest(await SharedResources.FluentValidationResponse(result.Errors));
            }

            var response = await _userProfileService.UpdateProfileAsync(userProfileDto);
            if (response.Model == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateUserEmail")]
        [Authorize]
        public async Task<IActionResult> UpdateUserEmail([FromBody] UpdateUserEmailViewModel model)
        {
            EmailValidator validator = new EmailValidator();
            FluentValidation.Results.ValidationResult result = validator.Validate(model);
            if (!result.IsValid)
            {
                return BadRequest(await SharedResources.FluentValidationResponse(result.Errors));
            }

            var updateUserEmail = await _userProfileService.UpdateUserEmail(model);
            if(updateUserEmail.Model == null)
            {
                return BadRequest(updateUserEmail);
            }
            return Ok(updateUserEmail);
        }

        [HttpPost("AddUserWorkedProject")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> AddUserProject([FromBody] UsersProjectDto addUserProjectDto)
        {
            UserProjectValidator validator = new UserProjectValidator();
            FluentValidation.Results.ValidationResult result = validator.Validate(addUserProjectDto);
            if (!result.IsValid)
            {
                return BadRequest(await SharedResources.FluentValidationResponse(result.Errors));
            }
            var response = await _userProfileService.AddUserProjectAsync(addUserProjectDto);
            if (response.Model == null)
            {
                var errorResponse = new ResponseModel<UsersProjectDto>
                {
                    Model = null,
                    Message = response.Message
                };
                return BadRequest(errorResponse);
            }
            return Ok(response);
        }

        [HttpPut("UpdateUserWorkedProject")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> UpdateUserProjectAsync([FromBody] UsersProjectDto updateUserProjectDto)
        {

            UpdateUserProjectValidator validator = new UpdateUserProjectValidator();
            FluentValidation.Results.ValidationResult result = validator.Validate(updateUserProjectDto);
            if (!result.IsValid)
            {
                return BadRequest(await SharedResources.FluentValidationResponse(result.Errors));
            }
            var response = await _userProfileService.UpdateUserProjectAsync(updateUserProjectDto);
            if (response.Model == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteUserWorkedProject")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> DeleteUserProject(int userProjectID)
        {
            var response = await _userProfileService.DeleteUserProjectAsync(userProjectID);

            if (!response.Model)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost("AddUserTool")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> AddUserToolAsync([FromBody] UsersToolDto userToolDto)
        {
            UserToolValidator validator = new UserToolValidator();
            FluentValidation.Results.ValidationResult result = validator.Validate(userToolDto);

            if (!result.IsValid)
            {
                return BadRequest(await SharedResources.FluentValidationResponse(result.Errors));
            }

            var response = await _userProfileService.AddUserToolAsync(userToolDto);

            if (response.Model == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateUserTool")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> UpdateUserToolAsync([FromBody] UsersToolDto addUserToolDto)
        {
            UpdateUserToolValidator validator = new UpdateUserToolValidator();
            FluentValidation.Results.ValidationResult result = validator.Validate(addUserToolDto);
            if (!result.IsValid)
            {
                return BadRequest(await SharedResources.FluentValidationResponse(result.Errors));
            }
            var response = await _userProfileService.UpdateUserToolAsync(addUserToolDto);
            if (response.Model == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteUserTool")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD")]
        public async Task<IActionResult> DeleteUserToolAsync(int userToolId)
        {
            var response = await _userProfileService.DeleteUserToolAsync(userToolId);
            if (!response.Model)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        //[HttpPut("UpdateUserTeamAdmin")]
        //public async Task<IActionResult> UpdateUserTeamAdminAsync([Required] Guid userId, [Required] Guid teamAdminId)
        //{
        //    var response = await _userProfileService.UpdateUserTeamAdminAsync(userId, teamAdminId);

        //    if (response.Model == null)
        //    {
        //        return NotFound(response);
        //    }
        //    return Ok(response);
        //}

        //[HttpGet("GetDepTeamAdmin")]
        //public async Task<IActionResult> GetDepTeamAdminAsync([FromQuery] int depId)
        //{
        //    var response = await _userProfileService.GetDepTeamAdminAsync(depId);

        //    if (response.Model == null)
        //    {
        //        return NotFound(response);
        //    }
        //    return Ok(response);
        //}


        //[HttpGet("GetAllProjectsListByDepartment")]
        //public async Task<IActionResult> GetProjectsByDepartmentId(int departmentId)
        //{
        //    var response = await _userProfileService.GetProjectsByDepartmentIdAsync(departmentId);
        //    if (response.Model == null)
        //    {
        //        return NotFound(response);
        //    }
        //    return Ok(response);
        //}

        [HttpGet("GetTechnologyList")]
        [Authorize]
        public async Task<IActionResult> GetTags()
        {
            var response = await _userProfileService.GetTagsAsync();           
            return Ok(response);
        }

        [HttpPut("UpdateProfileImage")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileImageAsync([FromForm] UpdateUserProfileImageDto userProfileImageDto)
        {
            var claims = SharedResources.GetUserIdFromToken(Request);

            if (string.IsNullOrEmpty(claims.UserId))
            {
                return BadRequest(SharedResources.UserNotFound);
            }

            var response = await _userProfileService.UpdateProfileImageAsync(userProfileImageDto, claims.UserId);
            if (response.Model == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}


