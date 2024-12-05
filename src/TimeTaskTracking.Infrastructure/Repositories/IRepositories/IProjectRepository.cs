using System.Security.Claims;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface IProjectRepository
{
    Task<PaginationResponseViewModel<List<Project>>> GetAllProjects(string employeeId, string role, ProjectFilterViewModel projectFilterViewModel);
    Task<List<DropDownResponse<string>>> GetListOfOpenAssignedProjectsToTeamLeadAsync(string teamLeadId, int departmentId);
    Task<PaginationResponseViewModel<List<Project>>> GetListOfProjectsAssignedToTeamLeadAsync(TeamLeadProjectListViewModel model);
    Task<Project> GetProject(int id, int departmentId);
    Task<bool> UpdateProjectStatusAsync (int projectId, int projectStatus);
    Task<List<string>> GetProjectTeamMembers(int projectId);
    Task<List<ProjectBillingHistoryViewModel>> GetProjectBillingHistoryAsync(int projectId);
    Task<List<ApplicationDomain>> GetApplicationDomainsAsync();
    Task<int> AddNewProject(Project project);
    Task<int> UpdateProjectDetails(Project project);
    Task<bool> DeleteProject(int id);
    Task<List<Project>> GetAllProjectsByTL(string teamLeadId);
    Task<List<DropDownResponse<string>>> GetProjectNameByTL(string teamLeaderId, int departmentId);
    Task<Project> GetProjectByName(string name);
    Task<List<ProjectTypeViewModel>> GetProjectsByDepartment(int departmentId);
    Task<List<ProjectTeamsViewModel>> GetListOfEmployeesInProjectById(int projectId);
    Task<bool> RemoveEmployeeFromProject(int projectId, string employeeId);
    Task<int> AddDocumentAsync(string fileName, int projectId);
    Task<List<DocumentUploads>> GetUploadedDocumentAsync(int projectId);
    Task<ProjectProductivity> GetProjectProductivityByProjectIdAsync(int projectId, int departmentId);
    Task<(int, string)> ProjectCountInCurrentMonthAndDepartmentName(int departmentId);
    Task<bool?> AddNewApplicationDomain(string ApplicationDomain);
    Task<int> GetApplicationDomainIdByName(string skill);
    Task<List<DropDownResponse<int>>> GetManagerProjectList(string teamAdminId, int departmentId);
    Task<List<string>> GetAssignedEmployeesToTheProject(int projectId, int departmentId);
    Task<Project> GetProjectByClient(int clientId);
    Task AddProjectDepartment(int projectId,int departmentId);
    Task<List<int>> GetProjectDepartments(int projectId);
    Task RemoveDepartmentFromProject(int projectId, int departmentId);
    Task RemoveEmployeesFromTheProject(int projectId, int departmentId);
    //Task<string>AddSkillsToProject(string Skills);
}
