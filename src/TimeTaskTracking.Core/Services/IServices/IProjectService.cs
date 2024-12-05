using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Core.Services.IServices;

public interface IProjectService
{
    Task<ResponseModel<PaginationResponseViewModel<List<ProjectResponseDto>>>> GetAllProjects(ProjectFilterViewModel projectFilterViewModel);
    Task<ResponseModel<List<DropDownResponse<string>>>> GetListOfOpenAssignedProjectsToTeamLead(string teamLeadId, int departmentId);
    Task<ResponseModel<PaginationResponseViewModel<List<ProjectResponseDto>>>> GetListOfProjectsAssignedToTeamLead(TeamLeadProjectListViewModel model);
    Task<ResponseModel<ProjectResponseDto>> GetProject(int id, int departmentId);
    Task<ResponseModel<List<ProjectBillingHistoryViewModel>>> GetProjectBillingHistory(int projectId);
    Task<ResponseModel<List<ApplicationDomainDto>>> GetApplicationDomainsAsync();
    Task<ResponseModel<int>> AddProject(AddAssignProjectDto projectDto);
    Task<ResponseModel<int>> UpdateProject(AddAssignProjectDto projectDto);
    Task<ResponseModel<int>> UpdateProjectStatus(UpdateProjectStatusViewModel model);
    Task<ResponseModel<bool>> DeleteProject(int id);
    Task<ResponseModel<List<ProjectResponseDto>>> GetAllProjectsByTL(string teamLeadId);
    Task<ResponseModel<List<DropDownResponse<string>>>> GetProjectByTeamLeader(string teamLeaderId, int departmentId);
    Task<ResponseModel<ProjectResponseDto>> GetProjectByName(string name);
    Task<ResponseModel<List<ProjectTypeViewModel>>> GetProjectsByDepartment(int departmentId);
    Task<ResponseModel<int>> UploadDocumentAsync(DocumentUploadDto documentUploadDto);
    Task<ResponseModel<List<DocumentUploads>>> GetUploadedDocumentsAsync(int projectId);
    Task<ResponseModel<ProjectProductivityDto>> GetProjectProductivityByProjectIdAsync(int projectId, int departmentId);

}
