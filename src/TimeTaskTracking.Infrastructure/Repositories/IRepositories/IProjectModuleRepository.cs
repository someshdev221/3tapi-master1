using System.Globalization;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface IProjectModuleRepository
{
    Task<ProjectDetailsViewModel> GetProjectDetailsById(ProjectDetailFilterViewModel projectDetailFilterViewModel);
    Task<ProjectViewModel> GetProjectBasicDetailsByIdAsync(int projectId, int departmentId);
    Task<PaginationResponseViewModel<List<ModuleDetailsViewModel>>> GetProjectModuleDetailsByProjectIdAsync(ProjectModuleDetailsViewModel projectModuleModel);
    Task<List<BillingDetailsViewModel>> GetProjectBillingDetailsByProjectIdAsync(ProjectBillingDetailsViewModel projectBillingModel);
    Task<List<ReportsResponseViewModel>> GetProjectEmployeeDetailsByProjectIdAsync(ProjectBillingDetailsViewModel projectBillingModel);
    Task<List<DropDownResponse<string>>> GetProjectTeamMembersByProjectIdAsync(int projectId);
    Task<ProjectModule> GetProjectModuleName(int projectId, string name);
    Task<List<ProjectModule>> GetProjectModules(int projectId, string? status, int departmentId);
    Task<ProjectModule> GetProjectModule(string moduleId, int departmentId);
    Task<string> AddProjectModule(ProjectModule projectModule);
    Task<bool> DeleteProjectModule(string moduleId);
    Task<string> UpdateProjectModule(ProjectModule projectModule);
    Task<string> UpdateProjectModuleStatusAsync(string moduleId, string moduleStatus);
    Task<string> UpdateProjectModulePaymentStatusAsync(string moduleId, string paymentStatus);
    Task<List<DropDownResponse<string>>> GetProjectModulesByProject(int projectId, int departmentId);
    Task<bool> CheckModuleExistsInProjectAsync(int projectId, string moduleId);
    Task<bool> DeleteProjectModulesByProjectId(int projectId);
}
