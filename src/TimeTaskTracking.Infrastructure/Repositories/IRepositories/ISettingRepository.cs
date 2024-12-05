using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface ISettingRepository
    {
        Task<List<UsersByRoleViewModel>> GetUsersByRole(int departmentId, string roleId, string? searchKeyword);
        Task<int> AddTeamMemberToTeam(TeamAssignmentViewModel<string> teamAssignment);
        Task<List<ProjectTeamViewModel>> GetProjectMembersByProjectIdAsync(int projectId, int departmentId);
        Task<int> AddTeamMemberInProjectAsync(ProjectAssignmentViewModel<string, int> teamAssignment);
        Task<List<DropDownResponse<string>>> GetAssignedUserByName(string employeeId);
        Task<List<EmployeeResponse>> GetEmployeeByDepartmentId(string? teamAdminId, int departmentId);
        Task<bool> IsProjectAlreadyAssigned(int projectId, string employeeId);
        Task<bool> IsTeamAlreadyAssigned(string teamLeadId, string employeeId);
        Task<bool> CheckUsersDepartmentMatch(string teamLeadId, string employeeId, string teamAdminId);
        Task<bool> CheckUsersAndProjectDepartmentMatch(int projectId, string employeeId, string teamAdminId);
        Task<TeamLeadBAAndBDListViewModel> GetTeamLeadBAAndBDListByDepartmentId(string? teamAdminId, int departmentId);
        Task<List<ProductiveHoursViewModel>> GetEmployeeListWhoseProductiveHoursIsHigh(int totalWorkingDays, int departmentId, int month, int year, int badgeId);
        Task<bool> RemoveEmployeeFromTeam(string employeeId);
        Task<bool> UpdateCanEditStatusByTeamAdminIdOrDepartmentId (int departmentId, string teamAdminId , bool? canEditStatus);
    }
}
