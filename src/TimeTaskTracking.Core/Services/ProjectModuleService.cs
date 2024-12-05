using AutoMapper;
using Microsoft.AspNetCore.Http;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Core.Services;

public class ProjectModuleService : IProjectModuleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    public ProjectModuleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<ResponseModel<ProjectDetailsViewModel>> GetProjectDetailsById(ProjectDetailFilterViewModel projectDetailFilterViewModel)
    {
        var response = new ResponseModel<ProjectDetailsViewModel>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, projectDetailFilterViewModel.DepartmentId, string.Empty);
        projectDetailFilterViewModel.DepartmentId = claims.DepartmentId;
        var projectDetailsViewModel = await _unitOfWork.ProjectModule.GetProjectDetailsById(projectDetailFilterViewModel);
        if (projectDetailsViewModel != null)
            response.Model = projectDetailsViewModel;
        else
            response.Message.Add(SharedResources.ProjectModulesNotFound);
        return response;
    }

    public async Task<ResponseModel<ProjectViewModel>> GetProjectBasicDetails(int projectId, int departmentId)
    {
        var response = new ResponseModel<ProjectViewModel>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
        var projectDetailsModel = await _unitOfWork.ProjectModule.GetProjectBasicDetailsByIdAsync(projectId, claims.DepartmentId);
        if (projectDetailsModel != null)
        {
            if (projectDetailsModel.IsBilling == 3)
            {
                projectDetailsModel.ProjectProfileDetails = null;
                response.Model = projectDetailsModel;
            }
            else
            {
                response.Model = projectDetailsModel;
            }
        }
        else
            response.Message.Add(SharedResources.ProjectNotFound);
        return response;
    }

    public async Task<ResponseModel<PaginationResponseViewModel<List<ModuleDetailsViewModel>>>> GetProjectModuleDetailsByProjectId(ProjectModuleDetailsViewModel projectModuleModel)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<ModuleDetailsViewModel>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, projectModuleModel.DepartmentId, string.Empty);
        projectModuleModel.DepartmentId = claims.DepartmentId;
        var projectModuleDetails = await _unitOfWork.ProjectModule.GetProjectModuleDetailsByProjectIdAsync(projectModuleModel);
        if (projectModuleDetails?.results?.Any() == true)
            response.Model = projectModuleDetails;
        else
            response.Message.Add(SharedResources.ProjectModuleNotFound);
        return response;
    }

    public async Task<ResponseModel<List<BillingDetailsViewModel>>> GetProjectBillingDetailsByProjectId(ProjectBillingDetailsViewModel projectBillingModel)
    {
        var response = new ResponseModel<List<BillingDetailsViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, projectBillingModel.DepartmentId, string.Empty);
        projectBillingModel.DepartmentId = claims.DepartmentId;
        var moduleDetails = await _unitOfWork.ProjectModule.GetProjectBillingDetailsByProjectIdAsync(projectBillingModel);
        if (moduleDetails != null)
            response.Model = moduleDetails;
        else
            response.Message.Add(SharedResources.NoBillingDetailsFound);
        return response;
    }

    public async Task<ResponseModel<List<ReportsResponseViewModel>>> GetProjectEmployeeDetailsByProjectId(ProjectBillingDetailsViewModel projectEmployeeDetailsModel)
    {
        var response = new ResponseModel<List<ReportsResponseViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, projectEmployeeDetailsModel.DepartmentId, string.Empty);
        projectEmployeeDetailsModel.DepartmentId = claims.DepartmentId;
        var emloyeeDetails = await _unitOfWork.ProjectModule.GetProjectEmployeeDetailsByProjectIdAsync(projectEmployeeDetailsModel);
        if (emloyeeDetails != null)
            response.Model = emloyeeDetails;
        else
            response.Message.Add(SharedResources.NoEmployeeDetailsFound);
        return response;
    }

    public async Task<ResponseModel<List<DropDownResponse<string>>>> GetProjectTeamMembersByProjectId(int projectId)
    {
        var response = new ResponseModel<List<DropDownResponse<string>>>();
        var teamMembers = await _unitOfWork.ProjectModule.GetProjectTeamMembersByProjectIdAsync(projectId);
        if (teamMembers != null)
            response.Model = teamMembers;
        else
            response.Message.Add(SharedResources.NoTeamMembersFound);
        return response;
    }

    public async Task<ResponseModel<List<ProjectModuleResponseDto>>> GetProjectModules(int projectId, string? status, int departmentId)
    {
        var response = new ResponseModel<List<ProjectModuleResponseDto>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
        departmentId = claims.DepartmentId;
        var projectModules = await _unitOfWork.ProjectModule.GetProjectModules(projectId, status, departmentId);
        if (projectModules?.Any() == true)
            response.Model = _mapper.Map<List<ProjectModuleResponseDto>>(projectModules);
        else
            response.Message.Add(SharedResources.ProjectModulesNotFound);
        return response;
    }
    public async Task<ResponseModel<string>> AddProjectModule(ProjectModuleDto projectModuleDto)
    {
        var result = new ResponseModel<string>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
        var mappedDto = _mapper.Map<ProjectModule>(projectModuleDto);
        _mapper.Map<ProjectModule>(projectModuleDto).CreatedBy = claims.UserId;
        mappedDto.CreatedBy = claims.UserId;
        mappedDto.IsActive = true;
        var projectModuleId = await _unitOfWork.ProjectModule.AddProjectModule(mappedDto);
        if (projectModuleId == null)
            result.Message.Add(SharedResources.ErrorWhileSaveProject);
        else
            result.Message.Add(SharedResources.SaveMessage);
        result.Model = projectModuleId;
        return result;
    }
    public async Task<ResponseModel<string>> UpdateProjectModule(ProjectModuleDto projectModuleDto)
    {
        var result = new ResponseModel<string>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
        var mappedDto = _mapper.Map<ProjectModule>(projectModuleDto);
        _mapper.Map<ProjectModule>(projectModuleDto).UpdatedBy = claims.UserId;
        mappedDto.UpdatedBy = claims.UserId;

        if (claims.Role == "Team Lead")
        {
            var checkExistingModuleStatus = await _unitOfWork.ProjectModule.GetProjectModule(projectModuleDto.Id, claims.DepartmentId);
            if (checkExistingModuleStatus.PaymentStatus != projectModuleDto.PaymentStatus)
            {
                result.Message.Add(SharedResources.UnAuthorizedAccess);
                result.Model = SharedResources.UnAuthorizedAccess;
                return result;
            }
        }
        var projectModuleId = await _unitOfWork.ProjectModule.UpdateProjectModule(mappedDto);
        if (projectModuleId == null)
            result.Message.Add(SharedResources.ErrorWhileUpdateProjectModule);
        else
            result.Message.Add(SharedResources.UpdatedMessage);
        result.Model = projectModuleId;

        return result;
    }

    public async Task<ResponseModel<string>> UpdateProjectModulePaymentAndModuleStatus(UpdateModuleStatusViewModel model)
    {
        var result = new ResponseModel<string>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
        var checkModuleExists = await _unitOfWork.ProjectModule.GetProjectModule(model.ModuleId, claims.DepartmentId);

        if (checkModuleExists != null)
        {
            if (claims.Role == "Team Lead")
            {
                var projectId = checkModuleExists.ProjectId; // obtained project Id
                var listOfAssignedEmployees = await _unitOfWork.Project.GetListOfEmployeesInProjectById(projectId); //Obtained the users that are assigned to this project.
                bool isUserAssigned = listOfAssignedEmployees.Any(employee => employee.EmployeeId == claims.UserId); // check if the logged user is assigned to the project or not.

                if (!isUserAssigned)
                {
                    result.Message.Add(SharedResources.ProjectIsNotAssigned);
                    return result;
                }

                if (!string.IsNullOrEmpty(model.PaymentStatus))
                {
                    result.Message.Add(SharedResources.UnAuthorizedAccess);
                }
                else
                {
                    var updateModuleStatus = await _unitOfWork.ProjectModule.UpdateProjectModuleStatusAsync(model.ModuleId, model.ModuleStatus);
                    if (updateModuleStatus != null)
                    {
                        result.Message.Add(SharedResources.ProjectModuleStatusUpdated);
                        result.Model = updateModuleStatus;
                    }
                }
            }

            if (claims.Role == "Project Manager" || claims.Role == "BDM")
            {
                var projectId = checkModuleExists.ProjectId; // obtained project Id
                var listOfAssignedEmployees = await _unitOfWork.Project.GetListOfEmployeesInProjectById(projectId); //Obtained the users that are assigned to this project.
                bool isUserAssigned = listOfAssignedEmployees.Any(employee => employee.EmployeeId == claims.UserId); // check if the logged user is assigned to the project or not.

                if (!isUserAssigned)
                {
                    result.Message.Add(SharedResources.ProjectIsNotAssigned);
                    return result;
                }

                if (!string.IsNullOrEmpty(model.PaymentStatus))
                {
                    var updateModuleStatus = await _unitOfWork.ProjectModule.UpdateProjectModulePaymentStatusAsync(model.ModuleId, model.PaymentStatus);
                    if (updateModuleStatus != null)
                    {
                        result.Message.Add(SharedResources.ProjectModulePaymentStatusUpdated);
                        result.Model = updateModuleStatus;
                    }
                }

                if (!string.IsNullOrEmpty(model.ModuleStatus))
                {
                    var updateModuleStatus = await _unitOfWork.ProjectModule.UpdateProjectModuleStatusAsync(model.ModuleId, model.ModuleStatus);
                    if (updateModuleStatus != null)
                    {
                        result.Message.Add(SharedResources.ProjectModuleStatusUpdated);
                        result.Model = updateModuleStatus;
                    }
                }
            }

            if (claims.Role == "HOD" || claims.Role == "Admin")
            {
                if (!string.IsNullOrEmpty(model.PaymentStatus))
                {
                    var updateModuleStatus = await _unitOfWork.ProjectModule.UpdateProjectModulePaymentStatusAsync(model.ModuleId, model.PaymentStatus);
                    if (updateModuleStatus != null)
                    {
                        result.Message.Add(SharedResources.ProjectModulePaymentStatusUpdated);
                        result.Model = updateModuleStatus;
                    }
                }

                if (!string.IsNullOrEmpty(model.ModuleStatus))
                {
                    var updateModuleStatus = await _unitOfWork.ProjectModule.UpdateProjectModuleStatusAsync(model.ModuleId, model.ModuleStatus);
                    if (updateModuleStatus != null)
                    {
                        result.Message.Add(SharedResources.ProjectModuleStatusUpdated);
                        result.Model = updateModuleStatus;
                    }
                }
            }
        }
        else
        {
            result.Message.Add(SharedResources.ModuleNotFound);
        }
        return result;
    }


    public async Task<ResponseModel<bool>> DeleteProjectModule(string id)
    {
        var result = new ResponseModel<bool>();
        var checkEmployeeStatusForTheModule = await _unitOfWork.EmployeeStatus.GetEmployeeStatusByModuleId(id);
        if (checkEmployeeStatusForTheModule)
        {
            result.Message.Add(SharedResources.CantDeleteModuleId);
            return result;
        }
        var isDelete = await _unitOfWork.ProjectModule.DeleteProjectModule(id);
        if (!isDelete)
        {
            result.Message.Add(SharedResources.ErrorWhileDeleteProjectModule);
        }
        else
            result.Message.Add(SharedResources.DeletedMessage);
        result.Model = isDelete;
        return result;
    }

    public async Task<ResponseModel<ProjectModuleResponseDto>> GetProjectModule(string moduleId, int departmentId)
    {
        var result = new ResponseModel<ProjectModuleResponseDto>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
        departmentId = claims.DepartmentId;
        var projectModule = await _unitOfWork.ProjectModule.GetProjectModule(moduleId, departmentId);
        if (projectModule == null)
            result.Message.Add(SharedResources.ProjectModuleNotFound);
        else
            result.Model = _mapper.Map<ProjectModuleResponseDto>(projectModule);
        return result;
    }

    public async Task<ResponseModel<ProjectModuleResponseDto>> GetProjectModuleByName(int projectId, string name)
    {
        var result = new ResponseModel<ProjectModuleResponseDto>();
        var projectModuleName = await _unitOfWork.ProjectModule.GetProjectModuleName(projectId, name);
        if (projectModuleName == null)
            result.Message.Add(SharedResources.ProjectModuleNotFound);
        else
            result.Model = _mapper.Map<ProjectModuleResponseDto>(projectModuleName);
        return result;
    }
    public async Task<ResponseModel<List<DropDownResponse<string>>>> GetProjectModulesByProject(int projectId)
    {
        var result = new ResponseModel<List<DropDownResponse<string>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
        var projectModules = await _unitOfWork.ProjectModule.GetProjectModulesByProject(projectId, claims.DepartmentId);
        if (projectModules?.Any() != true)
            result.Message.Add(SharedResources.ProjectModuleNotFound);
        else
            result.Model = projectModules;
        return result;
    }

    public async Task<bool> CheckModuleExistsInProject(int projectId, string moduleId)
    {
        var projectModuleExists = await _unitOfWork.ProjectModule.CheckModuleExistsInProjectAsync(projectId, moduleId);
        return projectModuleExists;
    }

    
}
