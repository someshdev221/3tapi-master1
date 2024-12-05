using System.Threading.Tasks;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetUserProfileDetailAsync(string userProfileId);
        Task<List<UserProject>> GetUserProjectsAsync(string userId);
        Task<List<UserTools>> GetUserToolsAsync(string userId);
        Task<UserProfileView> GetUserProfileByIdAsync(string userProfileId);
        Task<UpdateUserProject> UpdateProfileAsync(UpdateUserProject updateUserProfile);
        Task<bool?> AddNewSkill(string skill);
        Task<UserProject> GetUserProjectByUserIdAsync(int id);
        Task<string> UpdateUserEmailAsync(UpdateUserEmailViewModel model, string EmployeeId);
        Task<AddUserProject> AddUserProjectAsync(AddUserProject userProject);
        Task<AddUserProject> UpdateUserProjectAsync(AddUserProject updateUserWorkedProject);
        Task<bool> DeleteUserProjectAsync(int userProjectID);
        Task<UserTools> GetUserToolByUserIdAsync(int id);
        Task<UserTools> AddUserToolAsync(UserTools userTools);
        Task<UserTools> GetUserToolByDescriptionAsync(string applicationUsersId, string description);
        Task<UserTools> UpdateUserToolAsync(UserTools userTool);
        Task<bool> DeleteUserToolAsync(int userToolID);
        Task<UserProfile> UpdateUserTeamAdminAsync(Guid userId, Guid teamAdminId);
        //Task<List<TeamAdminModel>> GetDepTeamAdminAsync(int depId);
        Task<CombinedUserProfile> GetAllUserProfileAsync(string userId);
        Task<List<DropDownResponse<int>>> GetProjectsByDepartmentIdAsync(int departmentId);
        Task<List<DropDownResponse<int>>> GetTagsAsync();
        Task<bool> UserProjectExistsAsync(string userId, int projectId,int workedProjectId);
        Task<UpdateUserProfileImage> UpdateProfileImageAsync(UpdateUserProfileImage updateProfileImage);
        Task<int> GetSkillIdByNameAsync(string skill);
    }
}
