using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(IConfiguration configuration, IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ILogger<UserProfileService> logger, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public async Task<ResponseModel<UserProfileDto>> GetUserProfileDetailAsync(string userProfileId)
        {
            var response = new ResponseModel<UserProfileDto>();
            var userProfile = await _unitOfWork.UserProfile.GetUserProfileDetailAsync(userProfileId);

            if (userProfile == null)
                response.Message.Add(SharedResources.UserProfileNotFound);
            else
                response.Model = _mapper.Map<UserProfileDto>(userProfile);
            return response;
        }
        public async Task<ResponseModel<UserProfileView>> GetUserProfileByIdAsync(string userId)
        {
            var response = new ResponseModel<UserProfileView>();
            // Retrieve claims
            //var claims = await SharedResources.GetDepartmentFromClaims(httpContext, 0, string.Empty);

            //// Check if UserId from claims matches the provided userProfileId
            //if (string.IsNullOrEmpty(claims.UserId))
            //{
            //    response.Message.Add(SharedResources.UserNotFoundPleaseCheckYourDetailsAndTryAgain);
            //    return response;
            //}

            // Fetch user profile data
            var userProfile = await _unitOfWork.UserProfile.GetUserProfileByIdAsync(userId);

            if (userProfile == null)
            {
                response.Message.Add(SharedResources.UserNotFoundPleaseCheckYourDetailsAndTryAgain);
            }
            else
            {
                //var userProfileDto = _mapper.Map<UserProfileDto>(userProfile);
                response.Model = userProfile;
            }

            return response;
        }
        public async Task<ResponseModel<UpdateUserProfileDto>> UpdateProfileAsync(UpdateUserProfileDto updateProfileDto)
        {
            var response = new ResponseModel<UpdateUserProfileDto>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var allowedRoles = new[] { "HOD", "HR", "Admin" };
            var claims_HOD = new UserIdentityModel();
            try
            {
                if (allowedRoles.Contains(claims.Role))
                {
                    claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                    claims.UserId = claims_HOD.UserId;
                }
                if (string.IsNullOrEmpty(claims.UserId))
                {
                    response.Message.Add(SharedResources.ErrorWhileUpdatingUserProfile);
                    return response;
                }
                var existingUserProfile = await _unitOfWork.UserProfile.GetUserProfileDetailAsync(updateProfileDto.Id);
                if (existingUserProfile == null)
                {
                    response.Message.Add(SharedResources.UserNotFound);
                    return response;
                }
                if(claims.UserId != existingUserProfile.Id)
                {
                    response.Message.Add(SharedResources.ErrorWhileUpdatingUserProfile);
                    return response;
                }
                string? newImageName = null;
                if (updateProfileDto.ProfileImage != null && updateProfileDto.ProfileImage.Length > 0)
                {
                    const int maxFileSize = 5 * 1024 * 1024;
                    if (updateProfileDto.ProfileImage.Length > maxFileSize)
                    {
                        response.Message.Add(SharedResources.FileSizeExceedsThe5MBLimit);
                        return response;
                    }
                    var allowedFormats = new[] { ".jpeg", ".jpg", ".png" };
                    var fileExtension = Path.GetExtension(updateProfileDto.ProfileImage.FileName).ToLower();
                    if (!allowedFormats.Contains(fileExtension))
                    {
                        response.Message.Add(SharedResources.OnlyJpegJpgOrPngFormatPhotosAreAllowed);
                        return response;
                    }
                    if (!string.IsNullOrEmpty(existingUserProfile.ProfileImage))
                    {
                        SharedResources.DeleteProfileImage(existingUserProfile.ProfileImage, _webHostEnvironment, _logger);
                    }
                    newImageName = await SharedResources.SaveProfileImage(updateProfileDto.ProfileImage, _webHostEnvironment, _logger);
                }
                var updateUserProfile = _mapper.Map<UpdateUserProject>(updateProfileDto);
                updateUserProfile.Email = existingUserProfile.Email;
                updateUserProfile.DepartmentId = existingUserProfile.DepartmentId;
                updateUserProfile.EmployeeNumber = existingUserProfile.EmployeeNumber;
                if (!string.IsNullOrEmpty(newImageName))
                {
                    updateUserProfile.ProfileImageName = newImageName;
                }
                else
                {
                    updateUserProfile.ProfileImageName = existingUserProfile.ProfileImage;
                }

                if (updateProfileDto.DepartmentId != existingUserProfile.DepartmentId)
                {
                    response.Message.Add(SharedResources.UnableToChangeDepartment);
                    return response;
                }
                if (updateProfileDto.EmployeeNumber != existingUserProfile.EmployeeNumber)
                {
                    response.Message.Add(SharedResources.UnableToChangeDepartment);
                    return response;
                }
                if (!string.IsNullOrEmpty(updateProfileDto.Skills))
                {
                    var skillList = updateProfileDto.Skills.Split(',').Select(s => s.Trim()).ToList();
                    foreach (var skill in skillList)
                    {
                        await _unitOfWork.UserProfile.AddNewSkill(skill);
                    }
                }
                else
                {
                    updateUserProfile.Skills = null; 
                }
                var updatedUserProfile = await _unitOfWork.UserProfile.UpdateProfileAsync(updateUserProfile);
                response.Model = _mapper.Map<UpdateUserProfileDto>(updatedUserProfile);
                response.Message.Add(SharedResources.ProfileUpdatedSuccessfully);
            }
            catch (Exception)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }
        public async Task<ResponseModel<UserProjectDto>> GetUserProjectByUserIdAsync(int id)
        {
            var response = new ResponseModel<UserProjectDto>();
            try
            {
                var userProject = await _unitOfWork.UserProfile.GetUserProjectByUserIdAsync(id);

                if (userProject != null)
                {
                    var userProjectDto = _mapper.Map<UserProjectDto>(userProject);
                    response.Model = userProjectDto;
                }
                else
                {
                    response.Message.Add(SharedResources.UserProjectNotFound);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<string>> UpdateUserEmail(UpdateUserEmailViewModel model)
        {
            var response = new ResponseModel<string>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var allowedRoles = new[] { "HOD", "HR", "Admin" };

            if (allowedRoles.Contains(claims.Role))
            {
                var claims_HOD = new UserIdentityModel();
                claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                claims.UserId = claims_HOD.UserId;
            }

            
                var userInfo = await _unitOfWork.Account.EmailExists(model.Email);
                if (userInfo != null)
                {
                    if (userInfo.IsActive == 0 || userInfo.IsActive == 2)
                        response.Message.Add(SharedResources.EmailExistUserInActive);
                    else if (userInfo.IsActive == 1)
                        response.Message.Add(SharedResources.EmailAlreadyExist);
                    return response;
                }

                var updateUserEmailAsync = await _unitOfWork.UserProfile.UpdateUserEmailAsync(model, claims.UserId);
                if(updateUserEmailAsync !=null )
                {
                    response.Message.Add(SharedResources.EmailUpdatedSuccessfully);
                    response.Model = updateUserEmailAsync;
                }
                else
                {
                    response.Message.Add(SharedResources.FailedToUpdateEmail);
                }           
            return response;
        }


        public async Task<ResponseModel<UsersProjectDto>> AddUserProjectAsync(UsersProjectDto userProjectDto)
        {
            var response = new ResponseModel<UsersProjectDto>();
            try
            {
                if (string.IsNullOrEmpty(userProjectDto.ApplicationUsersId) || !Guid.TryParse(userProjectDto.ApplicationUsersId, out _))
                {
                    response.Message.Add(SharedResources.UserNotFound);
                    return response;
                }

                // Check if the project is already assigned to the user
                var projectExists = await _unitOfWork.UserProfile.UserProjectExistsAsync(userProjectDto.ApplicationUsersId, userProjectDto.ProjectsId,0);
                if (projectExists)
                {
                    response.Message.Add(SharedResources.ThisProjectIsAlreadyAssignedToTheUser);
                    return response;
                }

                var userProject = _mapper.Map<AddUserProject>(userProjectDto);
                userProject.CreatedAt = DateTime.UtcNow;
                var addedUserProject = await _unitOfWork.UserProfile.AddUserProjectAsync(userProject);

                if (addedUserProject == null)
                {
                    response.Message.Add(SharedResources.FailedToAddUserProject);
                }
                else
                {
                    response.Model = _mapper.Map<UsersProjectDto>(addedUserProject);
                    response.Message.Add(SharedResources.UserProjectAddedSuccessfully);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }
        public async Task<ResponseModel<UsersProjectDto>> UpdateUserProjectAsync(UsersProjectDto updateUserProjectDto)
        {
            var response = new ResponseModel<UsersProjectDto>();
            try
            {
                var updateUserProject = _mapper.Map<AddUserProject>(updateUserProjectDto);


                if (string.IsNullOrEmpty(updateUserProjectDto.ApplicationUsersId) || !Guid.TryParse(updateUserProjectDto.ApplicationUsersId, out _))
                {
                    response.Message.Add(SharedResources.UserNotFound);

                    return response;
                }

                var projectExists = await _unitOfWork.UserProfile.UserProjectExistsAsync(updateUserProjectDto.ApplicationUsersId, updateUserProjectDto.ProjectsId, updateUserProjectDto.Id);
                if (projectExists)
                {
                    response.Message.Add(SharedResources.ThisProjectIsAlreadyAssignedToTheUser);
                    return response;
                }
                var userExists = await _unitOfWork.UserProfile.GetUserProfileDetailAsync(updateUserProjectDto.ApplicationUsersId);
                if (userExists == null)
                {
                    response.Message.Add(SharedResources.UserNotFound);

                    return response;
                }
                var updatedUserProject = await _unitOfWork.UserProfile.UpdateUserProjectAsync(updateUserProject);

                if (updatedUserProject == null)
                    response.Message.Add(SharedResources.FailedToUpdateUserProject);
                else
                {
                    response.Model = _mapper.Map<UsersProjectDto>(updatedUserProject);
                    response.Message.Add(SharedResources.UserProjectUpdatedSuccessfully);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);

            }
            return response;
        }
        public async Task<ResponseModel<bool>> DeleteUserProjectAsync(int userProjectID)
        {
            var response = new ResponseModel<bool>();
            try
            {
                var deletedUserProject = await _unitOfWork.UserProfile.DeleteUserProjectAsync(userProjectID);
                if (!deletedUserProject)
                    response.Message.Add(SharedResources.UserProjectNotFound);
                else
                {
                    response.Message.Add(SharedResources.DeleteUserProjectSuccessfully);
                    response.Model = true;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<UserToolsDto>> GetUserToolByUserIdAsync(int id)
        {
            var response = new ResponseModel<UserToolsDto>();
            try
            {
                var userTool = await _unitOfWork.UserProfile.GetUserToolByUserIdAsync(id);

                if (userTool != null)
                {
                    var userToolDto = _mapper.Map<UserToolsDto>(userTool);
                    response.Model = userToolDto;
                }
                else
                {
                    response.Message.Add(SharedResources.UserNotFound);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<UsersToolDto>> AddUserToolAsync(UsersToolDto userToolDto)
        {
            var response = new ResponseModel<UsersToolDto>();
            try
            {
                if (string.IsNullOrEmpty(userToolDto.ApplicationUsersId) || !Guid.TryParse(userToolDto.ApplicationUsersId, out _))
                {

                    response.Message.Add(SharedResources.UserNotFound);
                    return response;
                }
                var existingTool = await _unitOfWork.UserProfile.GetUserToolByDescriptionAsync(userToolDto.ApplicationUsersId, userToolDto.Description);
                if (existingTool != null)
                {
                    response.Message.Add(SharedResources.ToolAlreadyExists); 
                    return response;
                }
                var userTool = await _unitOfWork.UserProfile.AddUserToolAsync(_mapper.Map<UserTools>(userToolDto));
                if (userTool == null)
                    response.Message.Add(SharedResources.FailedToAddUserTool);
                else
                {
                    response.Model = _mapper.Map<UsersToolDto>(userTool);
                    response.Message.Add(SharedResources.ToolAddedSuccessfully);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }
        public async Task<ResponseModel<UsersToolDto>> UpdateUserToolAsync(UsersToolDto userToolDto)
        {
            var response = new ResponseModel<UsersToolDto>();
            try
            {
                var userTool = await _unitOfWork.UserProfile.UpdateUserToolAsync(_mapper.Map<UserTools>(userToolDto));

                if (userTool != null)
                {
                    response.Model = _mapper.Map<UsersToolDto>(userTool);
                    response.Message.Add(SharedResources.ToolUpdatedSuccessfully);
                }
                else
                    response.Message.Add(SharedResources.FailedToUpdateUserTool);
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<bool>> DeleteUserToolAsync(int userToolId)
        {
            var response = new ResponseModel<bool>();
            try
            {
                var userTool = await _unitOfWork.UserProfile.DeleteUserToolAsync(userToolId);
                if (userTool)
                {
                    response.Message.Add(SharedResources.DeletedUserToolSuccessfully);
                    response.Model = userTool;
                }
                else
                {
                    response.Message.Add(SharedResources.UserNotFound);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }
        public async Task<ResponseModel<UserProfileDto>> UpdateUserTeamAdminAsync(Guid userId, Guid teamAdminId)
        {
            var response = new ResponseModel<UserProfileDto>();
            try
            {
                var userProfile = await _unitOfWork.UserProfile.UpdateUserTeamAdminAsync(userId, teamAdminId);

                if (userProfile != null)
                {
                    response.Model = _mapper.Map<UserProfileDto>(userProfile);
                    response.Message.Add(SharedResources.UpdatedUserSuccessfully);
                }
                else
                {
                    response.Message.Add(SharedResources.UserNotFound);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }
        //public async Task<ResponseModel<List<TeamAdminDto>>> GetDepTeamAdminAsync(int depId)
        //{
        //    var response = new ResponseModel<List<TeamAdminDto>>();
        //    try
        //    {
        //        var teamAdmins = await _unitOfWork.UserProfile.GetDepTeamAdminAsync(depId);

        //        if (teamAdmins != null && teamAdmins.Count > 0)
        //        {
        //            response.Model = _mapper.Map<List<TeamAdminDto>>(teamAdmins);
        //            response.Message.Add(SharedResources.RetrievedSuccessfully);
        //        }
        //        else
        //        {
        //            response.Message.Add(SharedResources.NoDataFound);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add(SharedResources.InternalServerError);
        //    }
        //    return response;
        //}


        //Get User Profile
        public async Task<ResponseModel<CombinedUserProfile>> GetAllUserProfileAsync(string userId)
        {
            var response = new ResponseModel<CombinedUserProfile>();

            try
            {

                // Get claims
                //var claims = await SharedResources.GetDepartmentFromClaims(httpContext, 0, string.Empty);
                var combinedUserProfile = await _unitOfWork.UserProfile.GetAllUserProfileAsync(userId);

                if (combinedUserProfile.UserProfile == null &&
                    (combinedUserProfile.UserBadges == null || combinedUserProfile.UserBadges.Count == 0))

                {
                    response.Message.Add(SharedResources.UserNotFound);
                }
                else
                    response.Model = _mapper.Map<CombinedUserProfile>(combinedUserProfile);
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<List<UserProject>>> GetUserProjectsAsync(string userProfileId)
        {
            var response = new ResponseModel<List<UserProject>>();

            try
            {

                var userProjects = await _unitOfWork.UserProfile.GetUserProjectsAsync(userProfileId);

                if (userProjects == null || userProjects.Count == 0)
                {
                    response.Message.Add(SharedResources.UserProjectNotFound);
                }
                else
                    response.Model = userProjects;
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }


        public async Task<ResponseModel<List<UserTools>>> GetUserToolsAsync(string userProfileId)
        {
            var response = new ResponseModel<List<UserTools>>();

            try
            {
                var userTools = await _unitOfWork.UserProfile.GetUserToolsAsync(userProfileId);

                if (userTools == null || userTools.Count == 0)
                {
                    response.Message.Add(SharedResources.UserToolsNotFound);
                }
                else
                    response.Model = userTools;
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<List<DropDownResponse<int>>>> GetProjectsByDepartmentIdAsync(int departmentId)
        {
            var response = new ResponseModel<List<DropDownResponse<int>>>();
            try
            {
                var projects = await _unitOfWork.UserProfile.GetProjectsByDepartmentIdAsync(departmentId);

                if (projects == null || projects.Count == 0)
                {
                    response.Message.Add(SharedResources.ProjectNotFound);
                    return response;
                }

                response.Model = projects.Select(p => new DropDownResponse<int>
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();

                response.Message.Add(SharedResources.ProjectsRetrievedSuccessfully);
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<List<DropDownResponse<int>>>> GetTagsAsync()
        {
            var response = new ResponseModel<List<DropDownResponse<int>>>();
            try
            {
                var tags = await _unitOfWork.UserProfile.GetTagsAsync();
                if (tags != null && tags.Count > 0)
                {
                    response.Model = tags;

                }
                else
                {
                    response.Message.Add(SharedResources.NoTechnologyFound);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }


        public async Task<ResponseModel<UpdateUserProfileImageDto>> UpdateProfileImageAsync(UpdateUserProfileImageDto updateProfileImageDto, string userId)
        {
            var response = new ResponseModel<UpdateUserProfileImageDto>();
            try
            {
                var existingUserProfile = await _unitOfWork.UserProfile.GetUserProfileDetailAsync(userId);
                if (existingUserProfile == null)
                {
                    response.Message.Add(SharedResources.UserNotFound);
                    return response;
                }

                string? newImageName = null;
                if (updateProfileImageDto.ProfileImage != null && updateProfileImageDto.ProfileImage.Length > 0)
                {
                    const int maxFileSize = 5 * 1024 * 1024;
                    if (updateProfileImageDto.ProfileImage.Length > maxFileSize)
                    {
                        response.Message.Add(SharedResources.FileSizeExceedsThe5MBLimit);
                        return response;
                    }
                    var allowedFormats = new[] { ".jpeg", ".jpg", ".png" };
                    var fileExtension = Path.GetExtension(updateProfileImageDto.ProfileImage.FileName).ToLower();
                    if (!allowedFormats.Contains(fileExtension))
                    {
                        response.Message.Add(SharedResources.OnlyJpegJpgOrPngFormatPhotosAreAllowed);
                        return response;
                    }

                    if (!string.IsNullOrEmpty(existingUserProfile.ProfileImage))
                    {
                        SharedResources.DeleteProfileImage(existingUserProfile.ProfileImage, _webHostEnvironment, _logger);
                    }

                    newImageName = await SharedResources.SaveProfileImage(updateProfileImageDto.ProfileImage, _webHostEnvironment, _logger);
                }

                var updateUserProfileImage = new UpdateUserProfileImage
                {
                    Id = userId,
                    ProfileImageName = newImageName
                };

                var updatedUserProfileImage = await _unitOfWork.UserProfile.UpdateProfileImageAsync(updateUserProfileImage);
                if (updatedUserProfileImage == null)
                {
                    response.Message.Add(SharedResources.InternalServerError);
                    return response;
                }
                response.Model = _mapper.Map<UpdateUserProfileImageDto>(updatedUserProfileImage);
                response.Message.Add(SharedResources.ProfileUpdatedSuccessfully);
            }
            catch (Exception)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }
    }
}
