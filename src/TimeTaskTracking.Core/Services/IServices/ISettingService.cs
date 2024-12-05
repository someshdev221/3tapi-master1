using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface ISettingService
    {
        Task<ResponseModel<List<UsersByRoleViewModel>>> GetUsersByRole(int departmentId,string roleId, string? searchKeyword);
        Task<ResponseModel<SuccessFailureResultViewModel<TeamAssignmentResponseViewModel<string>>>> AddTeamMemberToTeam(TeamAssignmentViewModel<List<string>> model);
        Task<ResponseModel<List<ProjectTeamViewModel>>> GetProjectMembersByProjectIdAsync(int projectId, int departmentId);
        Task<ResponseModel<SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>>> AddTeamMemberInProject(ProjectAssignmentViewModel<List<string>,List<int>> assignDto);
        Task<ResponseModel<List<DropDownResponse<string>>>> GetAssignedUsersByName(string employeeId);
        Task<ResponseModel<List<EmployeeResponse>>> GetEmployeeByDepartmentIdAsync(string? teamAdminId,int departmentId);
        Task<ResponseModel<TeamLeadBAAndBDListViewModel>> GetTeamLeadBAAndBDListByDepartmentId(string? teamAdminId, int departmentId);
        Task<ResponseModel<dynamic>> GetProductiveHoursByDepartmentIdAsync(AssignBadgeToEmployeesViewModel model);
        //Task<ResponseModel<bool>> IsProjectAssigned(int projectId, string userId);
        Task<ResponseModel<bool>> UpdateCanEditStatusByTeamAdminIdOrDepartmentId(UpdateCanEditStatusViewModel updateCanEditStatusViewModel);
    }
}
