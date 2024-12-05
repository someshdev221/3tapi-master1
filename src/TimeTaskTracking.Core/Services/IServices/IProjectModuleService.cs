using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Core.Services.IServices;

public interface IProjectModuleService
{
    Task<ResponseModel<ProjectDetailsViewModel>> GetProjectDetailsById(ProjectDetailFilterViewModel projectDetailFilterViewModel);
    Task<ResponseModel<ProjectViewModel>> GetProjectBasicDetails(int projectId, int departmentId);
    Task<ResponseModel<PaginationResponseViewModel<List<ModuleDetailsViewModel>>>> GetProjectModuleDetailsByProjectId(ProjectModuleDetailsViewModel projectModuleModel);
    Task<ResponseModel<List<BillingDetailsViewModel>>> GetProjectBillingDetailsByProjectId(ProjectBillingDetailsViewModel projectBillingModel);
    Task<ResponseModel<List<ReportsResponseViewModel>>> GetProjectEmployeeDetailsByProjectId(ProjectBillingDetailsViewModel projectEmployeeDetailsModel);
    Task<ResponseModel<List<DropDownResponse<string>>>> GetProjectTeamMembersByProjectId(int projectId);
    Task<ResponseModel<ProjectModuleResponseDto>> GetProjectModuleByName(int projectId, string name);
    Task<ResponseModel<List<ProjectModuleResponseDto>>> GetProjectModules(int projectId, string? status,int departmentId);
    Task<ResponseModel<ProjectModuleResponseDto>> GetProjectModule(string moduleId, int departmentId);
    Task<ResponseModel<string>> AddProjectModule(ProjectModuleDto projectModuleDto);
    Task<ResponseModel<string>> UpdateProjectModule(ProjectModuleDto projectModuleDto);
    Task<ResponseModel<string>> UpdateProjectModulePaymentAndModuleStatus(UpdateModuleStatusViewModel model );
    Task<ResponseModel<bool>> DeleteProjectModule(string id);
    Task<ResponseModel<List<DropDownResponse<string>>>> GetProjectModulesByProject(int projectId);
    Task<bool> CheckModuleExistsInProject(int projectId, string moduleId);

}
