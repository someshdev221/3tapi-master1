using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface IUserProfileService
    {
        Task<ResponseModel<UserProfileView>> GetUserProfileByIdAsync(string userId);
        Task<ResponseModel<List<UserProject>>> GetUserProjectsAsync(string userProfileId);
        Task<ResponseModel<List<UserTools>>> GetUserToolsAsync(string userProfileId);
        Task<ResponseModel<CombinedUserProfile>> GetAllUserProfileAsync(string userId);
        Task<ResponseModel<UpdateUserProfileDto>> UpdateProfileAsync(UpdateUserProfileDto updateProfile);
        Task<ResponseModel<string?>> UpdateUserEmail(UpdateUserEmailViewModel model);
        Task<ResponseModel<UserProjectDto>> GetUserProjectByUserIdAsync(int id);
        Task<ResponseModel<UsersProjectDto>> AddUserProjectAsync(UsersProjectDto userProjectDto);
        Task<ResponseModel<UsersProjectDto>> UpdateUserProjectAsync(UsersProjectDto updateUserProjectDto);
        Task<ResponseModel<bool>> DeleteUserProjectAsync(int userProjectID);
        Task<ResponseModel<UserToolsDto>> GetUserToolByUserIdAsync(int id);
        Task<ResponseModel<UsersToolDto>> AddUserToolAsync(UsersToolDto userTools);
        Task<ResponseModel<UsersToolDto>> UpdateUserToolAsync(UsersToolDto userToolDto);
        Task<ResponseModel<bool>> DeleteUserToolAsync(int userToolId);
        Task<ResponseModel<UserProfileDto>> UpdateUserTeamAdminAsync(Guid userId, Guid teamAdminId);
        //Task<ResponseModel<List<TeamAdminDto>>> GetDepTeamAdminAsync(int depId);
        Task<ResponseModel<List<DropDownResponse<int>>>> GetProjectsByDepartmentIdAsync(int departmentId);
        Task<ResponseModel<List<DropDownResponse<int>>>> GetTagsAsync();
        Task<ResponseModel<UpdateUserProfileImageDto>> UpdateProfileImageAsync(UpdateUserProfileImageDto updateProfileImageDto, string userId);
    }
}
