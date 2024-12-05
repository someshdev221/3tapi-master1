using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;


namespace TimeTaskTracking.Core.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
        _webHostEnvironment = webHostEnvironment;


    }

    public async Task<ResponseModel<PaginationResponseViewModel<List<ProjectResponseDto>>>> GetAllProjects(ProjectFilterViewModel projectFilterViewModel)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<ProjectResponseDto>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, projectFilterViewModel.DepartmentId, projectFilterViewModel.TeamAdminId);
        projectFilterViewModel.DepartmentId = claims.DepartmentId;
        projectFilterViewModel.EndDate ??= DateTime.Now;
        var projects = await _unitOfWork.Project.GetAllProjects(claims.UserId, claims.Role, projectFilterViewModel);
        if (projects?.results?.Any() == true)
            response.Model = new PaginationResponseViewModel<List<ProjectResponseDto>>
            {
                results = _mapper.Map<List<ProjectResponseDto>>(projects.results),
                TotalCount = projects.TotalCount
            };
        else
            response.Message.Add(SharedResources.ProjectsNotFound);
        return response;
    }

    public async Task<ResponseModel<List<DropDownResponse<string>>>> GetListOfOpenAssignedProjectsToTeamLead(string teamLeadId, int departmentId)
    {
        var response = new ResponseModel<List<DropDownResponse<string>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);

        int validDepartmentId = 0;

        if (claims.Role == "Admin")
        {
            validDepartmentId = departmentId;
        }

        if (claims.Role == "HOD")
        {
            var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
            validDepartmentId = claims_HOD.DepartmentId;
        }

        if (claims.Role == "Project Manager")
        {
            validDepartmentId = claims.DepartmentId;
        }

        if (claims.Role == "Team Lead" || claims.Role == "BDM")
        {
            teamLeadId = claims.UserId;
            validDepartmentId = claims.DepartmentId;
        }

        var checkTeamLeadExists = await _unitOfWork.Employee.GetEmployeeById(teamLeadId, departmentId, string.Empty);

        if (checkTeamLeadExists != null)
        {
            if (checkTeamLeadExists.DepartmentId == validDepartmentId)
            {
                var projectList = await _unitOfWork.Project.GetListOfOpenAssignedProjectsToTeamLeadAsync(teamLeadId, departmentId);

                if (projectList?.Any() == false)
                {
                    response.Message.Add(SharedResources.NoProjectFound);
                    return response;
                }
                else
                {
                    response.Model = _mapper.Map<List<DropDownResponse<string>>>(projectList);
                    return response;
                }
            }
            else
            {
                response.Message.Add(SharedResources.InValidTeamLeadId);
                response.Model = new List<DropDownResponse<string>>
                    {
                        new DropDownResponse<string>
                        {
                            Id = "ERROR",  // For example, an error case
                            Name = "Error occurred"  // Add other properties if needed
                        }
                    };
                return response;
            }
        }
        else
            response.Message.Add(SharedResources.TeamLeadNotFound);
        return response;
    }

    public async Task<ResponseModel<PaginationResponseViewModel<List<ProjectResponseDto>>>> GetListOfProjectsAssignedToTeamLead(TeamLeadProjectListViewModel model)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<ProjectResponseDto>>>();
        var projects = await _unitOfWork.Project.GetListOfProjectsAssignedToTeamLeadAsync(model);
        if (projects?.results?.Any() == true)
            response.Model = new PaginationResponseViewModel<List<ProjectResponseDto>>
            {
                results = _mapper.Map<List<ProjectResponseDto>>(projects.results),
                TotalCount = projects.TotalCount
            };
        else
            response.Message.Add(SharedResources.ProjectsNotFound);
        return response;
    }
    public async Task<ResponseModel<ProjectResponseDto>> GetProject(int id, int departmentId)
    {
        var result = new ResponseModel<ProjectResponseDto>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
        departmentId = claims.DepartmentId;
        var project = await _unitOfWork.Project.GetProject(id, departmentId);
        if (project == null)
            result.Message.Add(SharedResources.ProjectNotFound);
        else
        {
            result.Model = _mapper.Map<ProjectResponseDto>(project);
            result.Model.DepartmentIds = await GetProjectDepartments(result.Model.Id);
        }

        return result;
    }

    public async Task<ResponseModel<List<ProjectBillingHistoryViewModel>>> GetProjectBillingHistory(int projectId)
    {
        var result = new ResponseModel<List<ProjectBillingHistoryViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

        var checkProjectIsAssigned = await _unitOfWork.Project.GetProjectTeamMembers(projectId);
        if (checkProjectIsAssigned == null)
        {
            result.Message.Add(SharedResources.NoEmployeeAssignUnderThisProject);
            return result;
        }
        if (claims.Role == "Admin")
        {
            checkProjectIsAssigned.Add(claims.LoggedInUserId);
        }
        if (claims.Role == "HOD")
        {
            var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
            var projectDetails = await _unitOfWork.Project.GetProjectDepartments(projectId);
            if (projectDetails != null && projectDetails.Contains(claims_HOD.DepartmentId))
            {
                checkProjectIsAssigned.Add(claims.LoggedInUserId);
            }
            else
            {
                result.Message.Add(SharedResources.ProjectNotBelongsToYourDepartment);
                return result;
            }
        }
        if (checkProjectIsAssigned?.Any() == true && checkProjectIsAssigned.Contains(claims.LoggedInUserId))
        {
            var projectBillingHistory = await _unitOfWork.Project.GetProjectBillingHistoryAsync(projectId);
            if (projectBillingHistory?.Any() == true)
            {
                result.Model = projectBillingHistory;
            }
            else
            {
                result.Message.Add(SharedResources.ProjectBillingHistoryNotFound);
            }
        }
        else
        {
            result.Message.Add(SharedResources.ProjectIsNotAssigned);
        }

        return result;
    }

    public async Task<ResponseModel<ProjectResponseDto>> GetProjectByName(string name)
    {
        var result = new ResponseModel<ProjectResponseDto>();
        var project = await _unitOfWork.Project.GetProjectByName(name);
        if (project == null)
            result.Message.Add(SharedResources.ProjectNotFound);
        result.Model = _mapper.Map<ProjectResponseDto>(project);
        return result;
    }
    // New Method to Get Application Domains
    public async Task<ResponseModel<List<ApplicationDomainDto>>> GetApplicationDomainsAsync()
    {
        var response = new ResponseModel<List<ApplicationDomainDto>>();
        var domains = await _unitOfWork.Project.GetApplicationDomainsAsync();

        if (domains?.Any() == true)
        {
            response.Model = _mapper.Map<List<ApplicationDomainDto>>(domains);
        }
        else
        {
            response.Message.Add(SharedResources.DomainsNotFound);
        }
        return response;
    }

    public async Task<ResponseModel<int>> AddProject(AddAssignProjectDto projectDto)
    {
        var result = new ResponseModel<int>();
        var mappedModel = _mapper.Map<Project>(projectDto);
        var claims_HOD = new UserIdentityModel();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

        if (claims.Role == "Admin")
        {
            mappedModel.DepartmentId = projectDto.DepartmentIds.FirstOrDefault();
        }

        if (claims.Role == "HOD")
        {
            claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
            mappedModel.DepartmentId = claims_HOD.DepartmentId;
        }

        if (claims.Role == "Project Manager" || claims.Role == "DBM" || claims.Role == "Team Lead")
        {
            mappedModel.DepartmentId = claims.DepartmentId;
        }

        mappedModel.CreatedBy = claims.LoggedInUserId;

        mappedModel.IsActive = 2;
        var (projectCount, depatmentName) = await _unitOfWork.Project.ProjectCountInCurrentMonthAndDepartmentName(mappedModel.DepartmentId);
        mappedModel.InvoiceProjectID = SharedResources.GenerateInvoiceProjectID(depatmentName, (BillingType)mappedModel.IsBilling, projectCount + 1);

        if (string.IsNullOrEmpty(projectDto.Skills))
        {
            result.Message.Add(SharedResources.SkillSetsAreRequired);
            result.Model = 0;
            return result;
        }

        if (string.IsNullOrEmpty(projectDto.ApplicationDomains))
        {
            result.Message.Add(SharedResources.ApplicationDomainIsRequired);
            result.Model = 0;
            return result;
        }

        var skillList = projectDto.Skills.Split(',').Select(s => s.Trim()).ToList();
        var applicationDomainList = projectDto.ApplicationDomains.Split(',').Select(s => s.Trim()).ToList();

        if (skillList?.Any() == false || skillList.Count < 3)
        {
            result.Message.Add(SharedResources.PleaseProvideAtLeast3SkillSets);
            result.Model = 0;
            return result;

        }

        var skillIds = new List<int>();
        foreach (var skill in skillList)
        {
            await _unitOfWork.UserProfile.AddNewSkill(skill);
            var skillId = await _unitOfWork.UserProfile.GetSkillIdByNameAsync(skill);
            if (skillId != 0)
            {
                skillIds.Add(skillId);
            }
        }

        var applicationDomainsIds = new List<int>();
        foreach (var domain in applicationDomainList)
        {
            await _unitOfWork.Project.AddNewApplicationDomain(domain);
            var domainId = await _unitOfWork.Project.GetApplicationDomainIdByName(domain);
            if (domainId != 0)
            {
                applicationDomainsIds.Add(domainId);
            }
        }

        string skillSetIdList = string.Join(", ", skillIds);
        string ApplicationDomainsIdList = string.Join(", ", applicationDomainsIds);

        mappedModel.Skills = skillSetIdList;
        mappedModel.ApplicationDomains = ApplicationDomainsIdList;


        //if (!projectDto.DepartmentIds.Contains(projectDto.DepartmentId))
        //{
        //    projectDto.DepartmentIds.Add(projectDto.DepartmentId);
        //}

        if (!projectDto.InterDepartment && projectDto.DepartmentIds.Count > 1)
        {
            result.Message.Add(SharedResources.CanNotAssignToMultipleDepartments);
            return result;
        }
        else if (projectDto.InterDepartment && projectDto.DepartmentIds.Count < 2)
        {
            result.Message.Add(SharedResources.PleaseSelectMultipleDepartments);
            return result;
        }

        var project = await _unitOfWork.Project.AddNewProject(mappedModel);
        if (project == 0)
        {

            result.Message.Add(SharedResources.ErrorWhileSaveProject);
        }
        else
        {
            var response = new Register();
            var allowedRoles = new[] { "HOD", "Project Manager", "Admin" };
            if (!allowedRoles.Contains(claims.Role))
            {
                response = await _unitOfWork.Employee.GetEmployeeById(claims.UserId, claims.DepartmentId, string.Empty);
            }
            if (claims.Role != "Admin")
            {
                //if (projectDto.DepartmentId == 0)
                //{
                //    result.Message.Add(SharedResources.DepartmentIdIsRequired);
                //    result.Model = 0;
                //    return result;
                //}
                projectDto.EmployeeList.Add(claims.LoggedInUserId);
            }

            if (response?.TeamAdminId != null)
            {
                projectDto.EmployeeList.Add(response.TeamAdminId);
            }

            foreach (var departmentId in projectDto.DepartmentIds)
            {
                await _unitOfWork.Project.AddProjectDepartment(project, departmentId);
            }

            var ProjectDepartments = await _unitOfWork.Project.GetProjectDepartments(project);

            var employeeListToAddInProject = new List<string>();


            if (ProjectDepartments?.Any() == true)
            {
                if (projectDto.EmployeeList?.Any() == true)
                {
                    foreach (var employeeId in projectDto.EmployeeList)
                    {
                        var employeeDetails = await _unitOfWork.Employee.GetEmployeeById(employeeId, 0, string.Empty);
                        if (ProjectDepartments.Contains(employeeDetails.DepartmentId))
                        {
                            if (employeeDetails != null && employeeDetails?.TeamAdminId != null)
                            {
                                employeeListToAddInProject.Add(employeeDetails.TeamAdminId);
                            }
                            employeeListToAddInProject.Add(employeeId);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                employeeListToAddInProject = employeeListToAddInProject.Distinct().ToList();

                foreach (var employeeId in employeeListToAddInProject)
                {
                    await _unitOfWork.Setting.AddTeamMemberInProjectAsync(new ProjectAssignmentResponseViewModel<string, int>
                    {
                        EmployeeId = employeeId,
                        ProjectId = project
                    });

                }
            }
            else
            {
                result.Message.Add(SharedResources.ProjectDepartmentsNotFound);
                return result;

            }
            result.Message.Add(SharedResources.SaveMessage);
            result.Model = project;
        }
        return result;
    }

    public async Task<ResponseModel<int>> UpdateProject(AddAssignProjectDto projectDto)
    {
        var result = new ResponseModel<int>();
        var claims_HOD = new UserIdentityModel();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
        var mappedModel = _mapper.Map<Project>(projectDto);
        mappedModel.DepartmentId = claims.DepartmentId;
        mappedModel.UpdatedBy = claims.LoggedInUserId;

        if (projectDto.Skills == null)
        {
            result.Message.Add(SharedResources.SkillSetsAreRequired);
            result.Model = 0;
            return result;
        }

        if (projectDto.ApplicationDomains == null)
        {
            result.Message.Add(SharedResources.ApplicationDomainIsRequired);
            result.Model = 0;
            return result;
        }
        var skillList = projectDto.Skills.Split(',').Select(s => s.Trim()).ToList();
        var ApplicationDomainList = projectDto.ApplicationDomains.Split(',').Select(s => s.Trim()).ToList();
        if (skillList?.Any() == false || skillList.Count < 3)
        {
            result.Message.Add(SharedResources.PleaseProvideAtLeast3SkillSets);
            result.Model = 0;
            return result;
        }
        var skillIds = new List<int>();
        var ApplicationDomainIds = new List<int>();
        foreach (var skill in skillList)
        {
            await _unitOfWork.UserProfile.AddNewSkill(skill);
            var skillId = await _unitOfWork.UserProfile.GetSkillIdByNameAsync(skill);
            skillIds.Add(skillId);
        }

        foreach (var domain in ApplicationDomainList)
        {
            await _unitOfWork.Project.AddNewApplicationDomain(domain);
            var domainId = await _unitOfWork.Project.GetApplicationDomainIdByName(domain);
            ApplicationDomainIds.Add(domainId);
        }

        mappedModel.Skills = string.Join(",", skillIds);
        mappedModel.ApplicationDomains = string.Join(",", ApplicationDomainIds);

        //if (!projectDto.DepartmentIds.Contains(projectDto.DepartmentId))
        //{
        //    projectDto.DepartmentIds.Add(projectDto.DepartmentId);
        //}

        if (!projectDto.InterDepartment && projectDto.DepartmentIds.Count > 1)
        {
            result.Message.Add(SharedResources.CanNotAssignToMultipleDepartments);
            return result;
        }
        else if (projectDto.InterDepartment && projectDto.DepartmentIds.Count < 2)
        {
            result.Message.Add(SharedResources.PleaseSelectMultipleDepartments);
            return result;
        }


        //if (await _unitOfWork.Setting.IsProjectAlreadyAssigned(projectDto.Id, claims.UserId))
        //{
        var project = await _unitOfWork.Project.UpdateProjectDetails(mappedModel);
        if (project == 0)
        {
            result.Message.Add(SharedResources.ErrorWhileUpdateProject);
            return result;
        }
        else
        {
            var employeesToAdd = new List<string>();
            var employeesToRemove = new List<string>();
            var employeesInProject = await _unitOfWork.Project.GetListOfEmployeesInProjectById(project);
            var response = new Register();
            var allowedRoles = new[] { "HOD", "Project Manager", "Admin" };
            if (!allowedRoles.Contains(claims.Role))
            {
                response = await _unitOfWork.Employee.GetEmployeeById(claims.UserId, claims.DepartmentId, string.Empty);
            }
            if (employeesInProject?.Any() == true)
            {
                var existingEmployeeIds = employeesInProject.Select(e => e.EmployeeId).ToList();
                var newEmployeeIds = projectDto.EmployeeList;

                employeesToAdd = newEmployeeIds.Except(existingEmployeeIds).ToList(); // Employees to be added

                employeesToRemove = existingEmployeeIds.Except(newEmployeeIds).Where(e => e != response?.TeamAdminId).ToList();
            }
            else
                employeesToAdd = projectDto.EmployeeList;

            //if (response?.TeamAdminId == null)
            //    employeesToAdd.Add(response.TeamAdminId);
            if (claims.Role != "Admin")
            {
                employeesToAdd.Add(claims.LoggedInUserId);
            }

            //if (claims.Role == "HOD")
            //{
            //    if (!employeesToAdd.Contains(claims_HOD.UserId))
            //        employeesToAdd.Add(claims_HOD.UserId);
            //}
            //else
            //{
            //    if (!employeesToAdd.Contains(claims.UserId))
            //        employeesToAdd.Add(claims.UserId);
            //}

            var departmentAssignList = await _unitOfWork.Project.GetProjectDepartments(project);
            if (departmentAssignList != null)
            {
                var departmentsToRemove = departmentAssignList.Except(projectDto.DepartmentIds).ToList();
                foreach (var departmentId in departmentsToRemove)
                {
                    await _unitOfWork.Project.RemoveDepartmentFromProject(project, departmentId);
                    await _unitOfWork.Project.RemoveEmployeesFromTheProject(project, departmentId);
                }
            }
            projectDto.DepartmentIds = projectDto.DepartmentIds.Except(departmentAssignList).ToList();

            foreach (var departmentId in projectDto.DepartmentIds)
            {
                await _unitOfWork.Project.AddProjectDepartment(project, departmentId);
            }

            var projectDepartments = await _unitOfWork.Project.GetProjectDepartments(project);

            var assignNewEmployeesToTheProject = new List<string>();

            if (employeesToAdd?.Any() == true)
            {
                foreach (var employeeId in employeesToAdd)
                {
                    if (!await _unitOfWork.Setting.IsProjectAlreadyAssigned(project, employeeId))
                    {
                        var employeeExists = await _unitOfWork.Employee.GetEmployeeById(employeeId, 0, string.Empty);
                        var isUserMatch = projectDepartments.Contains(employeeExists.DepartmentId);
                        var projectEmployeeList = await _unitOfWork.Project.GetListOfEmployeesInProjectById(project);
                        var isEmployeeInProject = projectEmployeeList?.Any(e => e.EmployeeId == employeeExists.TeamAdminId);

                        if (isUserMatch && isEmployeeInProject == false)
                        {
                            if (employeeExists.TeamAdminId != null)
                            {
                                assignNewEmployeesToTheProject.Add(employeeExists.TeamAdminId);
                            }
                            assignNewEmployeesToTheProject.Add(employeeId);
                        }
                    }
                    continue;
                }

                foreach (var employeeId in assignNewEmployeesToTheProject)
                {
                    await _unitOfWork.Setting.AddTeamMemberInProjectAsync(new ProjectAssignmentResponseViewModel<string, int>
                    {
                        EmployeeId = employeeId,
                        ProjectId = project
                    });
                }
            }

            if (employeesToRemove?.Any() == true)
            {
                if (employeesToRemove.Contains(claims.UserId))
                    employeesToRemove.Remove(claims.UserId);

                if (employeesToRemove.Contains(response.TeamAdminId))
                    employeesToRemove.Remove(response.TeamAdminId);

                // Remove employees from the project
                foreach (var employeeId in employeesToRemove)
                {
                    var employeeDetails = await _unitOfWork.Employee.GetEmployeeById(employeeId, 0, string.Empty);
                    if (employeeDetails.RoleName == "Employee" || employeeDetails.RoleName == "BDM")
                    {
                        await _unitOfWork.Project.RemoveEmployeeFromProject(project, employeeId);
                    }
                    else
                        continue;
                }
            }
        }
        result.Message.Add(SharedResources.UpdatedMessage);
        result.Model = project;
        return result;
    }

    public async Task<ResponseModel<int>> UpdateProjectStatus(UpdateProjectStatusViewModel model)
    {
        var result = new ResponseModel<int>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

        int projectDepartmentId = 0;

        if (claims.Role == "Project Manager" || claims.Role == "BDM")
        {
            projectDepartmentId = claims.DepartmentId;
        }

        if (claims.Role == "HOD")
        {
            var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
            projectDepartmentId = claims_HOD.DepartmentId;
        }

        if (claims.Role == "Admin")
        {
            if (model.DepartmentId == null || model.DepartmentId == 0)
            {
                result.Model = -1;
                result.Message.Add(SharedResources.DepartmentIdIsRequired);
                return result;
            }
            else
            {
                projectDepartmentId = model.DepartmentId.Value;
            }
        }

        var getProjectDetails = await _unitOfWork.Project.GetProject(model.ProjectId, projectDepartmentId);

        if (getProjectDetails == null)
        {
            result.Model = 0;
            result.Message.Add(SharedResources.ProjectNotFound);
            return result;
        }

        if (!Enum.IsDefined(typeof(ProjectStatus), model.ProjectStatus))
        {
            result.Model = -1;
            result.Message.Add(SharedResources.ProjectStatus);
            return result;
        }

        var updateProjectStatus = await _unitOfWork.Project.UpdateProjectStatusAsync(model.ProjectId, model.ProjectStatus);
        if (updateProjectStatus)
        {
            result.Message.Add(SharedResources.ProjectStatusUpdatedSuccessfully);
            result.Model = 1;
        }
        else
        {
            result.Message.Add(SharedResources.FailedToUpdateProjectStatus);
            result.Model = 2;
        }
        return result;
    }


    public async Task<ResponseModel<bool>> DeleteProject(int projectId)
    {
        var result = new ResponseModel<bool>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
        var checkProjectModules = await _unitOfWork.ProjectModule.GetProjectModulesByProject(projectId, claims.DepartmentId);
        if (checkProjectModules?.Any() == true)
        {
            var deleteProjecModule = await _unitOfWork.ProjectModule.DeleteProjectModulesByProjectId(projectId);
            if (!deleteProjecModule)
            {
                result.Message.Add(SharedResources.ErrorWhileDeleteProjectModule);
                return result;
            }
        }
        var isDeleted = await _unitOfWork.Project.DeleteProject(projectId);
        if (!isDeleted)
            result.Message.Add(SharedResources.ErrorWhileDeleteProject);
        else
            result.Message.Add(SharedResources.DeletedMessage);
        result.Model = isDeleted;
        return result;
    }

    public async Task<ResponseModel<List<ProjectResponseDto>>> GetAllProjectsByTL(string teamLeadId)
    {
        var response = new ResponseModel<List<ProjectResponseDto>>();
        var projects = await _unitOfWork.Project.GetAllProjectsByTL(teamLeadId);
        if (projects?.Any() == true)
            response.Model = _mapper.Map<List<ProjectResponseDto>>(projects);
        else
            response.Message.Add(SharedResources.ProjectsNotFound);
        return response;
    }

    public async Task<ResponseModel<List<DropDownResponse<string>>>> GetProjectByTeamLeader(string teamLeaderId, int departmentId)
    {
        var response = new ResponseModel<List<DropDownResponse<string>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
        departmentId = claims.DepartmentId;
        var employeeList = await _unitOfWork.Project.GetProjectNameByTL(teamLeaderId, departmentId);

        if (employeeList.Count() == 0)
        {
            response.Message.Add(SharedResources.NoDataFound);
        }
        else
        {
            response.Model = _mapper.Map<List<DropDownResponse<string>>>(employeeList);
        }

        return response;
    }
    public async Task<ResponseModel<List<ProjectTypeViewModel>>> GetProjectsByDepartment(int departmentId)
    {
        var result = new ResponseModel<List<ProjectTypeViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
        var projects = await _unitOfWork.Project.GetProjectsByDepartment(claims.DepartmentId);
        if (projects?.Any() != true)
            result.Message.Add(SharedResources.ProjectsNotFound);
        else
            result.Model = projects;
        return result;
    }
    public async Task<ResponseModel<int>> UploadDocumentAsync(DocumentUploadDto documentUploadDto)
    {
        var response = new ResponseModel<int>();
        try
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".pdf", ".docx", ".xlsx" };
            var originalFileName = Path.GetFileName(documentUploadDto.File?.FileName);
            var extension = Path.GetExtension(documentUploadDto.File?.FileName).ToLower();

            if (documentUploadDto.File == null ||
                documentUploadDto.File.Length == 0 ||
                documentUploadDto.File.Length > 10 * 1024 * 1024 ||
                !allowedExtensions.Contains(extension))
            {
                if (documentUploadDto.File == null || documentUploadDto.File.Length == 0)
                {
                    response.Message.Add(SharedResources.NoFileFound);
                }
                else if (documentUploadDto.File.Length > 10 * 1024 * 1024)
                {
                    response.Message.Add(SharedResources.FileSizeExceedsThe10MBLimit);
                }
                else
                {
                    response.Message.Add(SharedResources.InvalidFileFormat);
                }
                return response;
            }
            var randomString = SharedResources.GenerateRandomString(5);
            var newFileName = $"{originalFileName}-{randomString}{extension}";

            var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Documents");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var filePath = Path.Combine(directoryPath, newFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await documentUploadDto.File.CopyToAsync(stream);
            }
            var documentUpload = new DocumentUpload
            {
                FileName = originalFileName,
                ProjectId = documentUploadDto.ProjectId
            };
            var result = await _unitOfWork.Project.AddDocumentAsync(newFileName, documentUploadDto.ProjectId);

            response.Model = result;
            response.Message.Add(SharedResources.FileUploadedSuccessfully);
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.FailedToUploadFile);
        }
        return response;
    }

    public async Task<ResponseModel<List<DocumentUploads>>> GetUploadedDocumentsAsync(int projectId)
    {
        var response = new ResponseModel<List<DocumentUploads>>();
        try
        {
            var result = await _unitOfWork.Project.GetUploadedDocumentAsync(projectId);
            response.Model = result;
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.NoDocumentsUploaded);
        }
        return response;
    }

    public async Task<ResponseModel<ProjectProductivityDto>> GetProjectProductivityByProjectIdAsync(int projectId, int departmentId)
    {
        var response = new ResponseModel<ProjectProductivityDto>();
        try
        {
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            int effectiveDepartmentId;

            if (claims.Role == "Admin")
            {

                effectiveDepartmentId = claims.DepartmentId;
            }
            else
            {
                effectiveDepartmentId = departmentId > 0 ? departmentId : claims.DepartmentId;
            }
            var productivity = await _unitOfWork.Project.GetProjectProductivityByProjectIdAsync(projectId, effectiveDepartmentId);

            if (productivity != null)
            {
                var productivityDto = _mapper.Map<ProjectProductivityDto>(productivity);
                response.Model = productivityDto;
            }
            else
            {
                response.Message.Add(SharedResources.ProjectNotFound);
            }

        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }
    private async Task<List<int>> GetProjectDepartments(int projectId)
    {
        return await _unitOfWork.Project.GetProjectDepartments(projectId);
    }
}
