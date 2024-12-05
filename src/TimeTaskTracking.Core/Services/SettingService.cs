using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Ocsp;
using System.Collections.Generic;
using System.Linq;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;
        private readonly IProjectService _projectService;
        private readonly IHttpContextAccessor _contextAccessor;

        public SettingService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor, IEmployeeService employeeService, IProjectService projectService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _employeeService = employeeService;
            _projectService = projectService;
            _contextAccessor = contextAccessor;
        }

        public async Task<ResponseModel<SuccessFailureResultViewModel<TeamAssignmentResponseViewModel<string>>>> AddTeamMemberToTeam(TeamAssignmentViewModel<List<string>> model)
        {
            var response = new ResponseModel<SuccessFailureResultViewModel<TeamAssignmentResponseViewModel<string>>>();
            try
            {
                if (model.EmployeeId == null)
                {
                    response.Message.Add(SharedResources.ProvideEmployeeToAssignTeam);
                    return response;
                }
                if (string.IsNullOrEmpty(model.TeamLeaderId) || model.TeamLeaderId?.Any() == false)
                {
                    response.Message.Add(SharedResources.ProvideTeamLeadToAssignTeam);
                    return response;
                }

                var assignmentResult = new SuccessFailureResultViewModel<TeamAssignmentResponseViewModel<string>>();
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, model.TeamAdminId);
                model.TeamAdminId = claims.UserId;

                //if (claims.Role == "Admin" && model.DepartmentId == 0 || model.DepartmentId == null)
                //{
                //    response.Message.Add(SharedResources.DepartmentIdIsRequired);
                //    return response;
                //}

                //if (claims.Role == "Admin" && model.DepartmentId != 0)
                //{
                //    claims.DepartmentId = model.DepartmentId.Value;
                //}

                var employeeToRemove = new List<string>();
                var employeeInTeam = await _unitOfWork.Employee.GetEmployeeNameByTL(model.TeamLeaderId);
                if (employeeInTeam != null && employeeInTeam.Any())
                {
                    var emloyesIds = employeeInTeam.Select(A => A.Id).ToList();
                    employeeToRemove = emloyesIds.Except(model.EmployeeId).ToList();
                }
                if (model.EmployeeId?.Any() == true)
                {
                    foreach (var empId in model.EmployeeId)
                    {
                        var teamAssignment = new TeamAssignmentResponseViewModel<string>
                        {
                            EmployeeId = empId,
                            TeamLeaderId = model.TeamLeaderId,
                            TeamAdminId = model.TeamAdminId
                        };
                        var teamLead = await _unitOfWork.Employee.GetEmployeeById(teamAssignment.TeamLeaderId, claims.DepartmentId, string.Empty);
                        if (teamLead != null && teamLead.RoleName != "Project Manager" && teamLead.RoleName != "HR"
                            || teamLead.RoleName != "HOD" && teamLead.RoleName != "Admin")
                        {
                            var employee = await _unitOfWork.Employee.GetEmployeeById(empId, claims.DepartmentId, string.Empty);
                            if (employee != null && employee.RoleName != "Project Manager" && employee.RoleName != "HR"
                            || employee.RoleName != "HOD" && employee.RoleName != "Admin")
                            {
                                if (await _unitOfWork.Setting.CheckUsersDepartmentMatch(model.TeamLeaderId, empId, model.TeamAdminId) && empId != model.TeamLeaderId &&
                                    empId != teamAssignment.TeamAdminId)
                                {
                                    if (!await _unitOfWork.Setting.IsTeamAlreadyAssigned(teamAssignment.TeamLeaderId, teamAssignment.EmployeeId))
                                    {
                                        //if (await _unitOfWork.Employee.EmployeeCountInTeam(teamAssignment.EmployeeId) == 0)
                                        //{
                                        await _unitOfWork.Employee.UpdateEmployeeManagerAsync(teamAssignment.EmployeeId, teamLead.DepartmentId, teamLead.TeamAdminId);
                                        var result = await _unitOfWork.Setting.AddTeamMemberToTeam(teamAssignment);
                                        if (result != 0)
                                        {
                                            teamAssignment.Id = result;
                                            assignmentResult.Success.Add(teamAssignment);
                                        }
                                        else
                                            assignmentResult.Failure.Add(teamAssignment);
                                        //}
                                        //else
                                        //{
                                        //    teamAssignment.Reason = SharedResources.ErrorWhileAssignTeamLeadToTeamLead;
                                        //    assignmentResult.Failure.Add(teamAssignment);
                                        //}
                                    }
                                }
                                else
                                {
                                    teamAssignment.Reason = SharedResources.DepartmentsDoesNotMatched;
                                    assignmentResult.Failure.Add(teamAssignment);
                                }
                            }
                            else
                            {
                                teamAssignment.Reason = SharedResources.ErrorWhileAssignEmployeeToTeam;
                                assignmentResult.Failure.Add(teamAssignment);
                            }
                        }
                    }
                    if (assignmentResult?.Failure?.Any() == true)
                        response.Message.Add(SharedResources.ErrorWhileAssignEmployeeToTeam);
                    else
                        response.Message.Add(SharedResources.AssignedEmployeeToTeam);
                    response.Model = assignmentResult;
                }
                else
                {
                    response.Message.Add(SharedResources.UpdatedMessage);
                    response.Model = assignmentResult;
                }
                if (employeeToRemove.Any())
                    foreach (var employeeId in employeeToRemove)
                    {
                        if (await _unitOfWork.Setting.CheckUsersDepartmentMatch(model.TeamLeaderId, employeeId, model.TeamAdminId))
                        {
                            await _unitOfWork.Setting.RemoveEmployeeFromTeam(employeeId);
                        }
                    }
            }
            catch (Exception ex)
            {
                response.Message.Add($"An error occurred: {ex.Message}");
            };
            return response;
        }

        //public async Task<ResponseModel<SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>>> AddTeamMemberInProject(ProjectAssignmentViewModel<List<string>, List<int>> assignDto)
        //{
        //    var response = new ResponseModel<SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>>();
        //    try
        //    {
        //        var assignmentResult = new SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>();
        //        if (assignDto.ProjectId == null || assignDto.ProjectId?.Any() == false)
        //        {
        //            response.Message.Add(SharedResources.ProvideProjectToAssignMember);
        //            return response;
        //        }
        //        if (assignDto.EmployeeId == null)
        //        {
        //            response.Message.Add(SharedResources.ProvideMemberToProjectAssign);
        //            return response;
        //        }
        //        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
        //        if (claims.Role == "HOD")
        //        {
        //            var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
        //            claims.UserId = claims_HOD.UserId;
        //        }
        //        foreach (var projectId in assignDto.ProjectId)
        //        {
        //            var employeesToRemove = new List<string>();
        //            var getLoggedClaims = new Register();
        //            var employeesInProject = await _unitOfWork.Project.GetListOfEmployeesInProjectById(projectId);
        //            if (employeesInProject?.Any() == true)
        //            {
        //                var existingEmployeeIds = employeesInProject.Select(e => e.EmployeeId).ToList();
        //                var newEmployeeIds = assignDto.EmployeeId;

        //                getLoggedClaims = await _unitOfWork.Employee.GetEmployeeById(claims.UserId, claims.DepartmentId, string.Empty);
        //                employeesToRemove = existingEmployeeIds.Except(newEmployeeIds).Where(e => e != getLoggedClaims.TeamAdminId).ToList();
        //                if (employeesToRemove?.Any() == true)
        //                {
        //                    if (employeesToRemove.Contains(claims.UserId))
        //                        employeesToRemove.Remove(claims.UserId);

        //                    if (employeesToRemove.Contains(getLoggedClaims.TeamAdminId))
        //                        employeesToRemove.Remove(getLoggedClaims.TeamAdminId);

        //                    foreach (var employeeId in employeesToRemove)
        //                    {
        //                        if (await _unitOfWork.Setting.CheckUsersAndProjectDepartmentMatch(projectId, employeeId, claims.UserId) && employeeId != claims.UserId)
        //                        {
        //                            await _unitOfWork.Project.RemoveEmployeeFromProject(projectId, employeeId);
        //                        }
        //                    }
        //                }
        //            }
        //            var employeeIdsToProcess = new List<string>(assignDto.EmployeeId);
        //            var processedEmployeeIds = new HashSet<string>();
        //            if (assignDto.EmployeeId.Any())
        //            {
        //                while (employeeIdsToProcess.Count > 0)
        //                {
        //                    var empId = employeeIdsToProcess[0];
        //                    employeeIdsToProcess.RemoveAt(0);

        //                    if (processedEmployeeIds.Contains(empId))
        //                        continue;

        //                    ProjectAssignmentResponseViewModel<string, int> projectAssignDto = new()
        //                    {
        //                        EmployeeId = empId,
        //                        ProjectId = projectId
        //                    };


        //                    var employeeDetails = await _unitOfWork.Employee.GetEmployeeById(empId, claims.DepartmentId, string.Empty);
        //                    if (employeeDetails != null)
        //                    {

        //                        if (!string.IsNullOrEmpty(employeeDetails?.TeamAdminId) && !assignDto.EmployeeId.Contains(employeeDetails.TeamAdminId))
        //                        {
        //                            assignDto.EmployeeId.Add(employeeDetails.TeamAdminId);
        //                            employeeIdsToProcess.Add(employeeDetails.TeamAdminId);
        //                        }

        //                        processedEmployeeIds.Add(empId);
        //                        var allowedRoles = new[] { "HOD", "Admin" };
        //                        if (!allowedRoles.Contains(claims.Role))
        //                        {
        //                            var checkIfAlreadyAssigned = await _unitOfWork.Setting.CheckUsersAndProjectDepartmentMatch(projectAssignDto.ProjectId, empId, claims.UserId);
        //                            if (!checkIfAlreadyAssigned)
        //                            {
        //                                projectAssignDto.Reason = SharedResources.ProjectAndUserDepartmentsDoesNotMatched;
        //                                assignmentResult.Failure.Add(projectAssignDto);
        //                            }
        //                        }
        //                        //    if (await _unitOfWork.Setting.CheckUsersAndProjectDepartmentMatch(projectAssignDto.ProjectId, empId, claims.UserId))
        //                        //{
        //                        if (!await _unitOfWork.Setting.IsProjectAlreadyAssigned(projectAssignDto.ProjectId, projectAssignDto.EmployeeId))
        //                        {
        //                            if (employeeDetails.RoleName == "Team Lead" || employeeDetails.RoleName == "BDM" || employeeDetails.RoleName == "HOD"
        //                                || employeeDetails?.RoleName == "Project Manager" || await _unitOfWork.Employee.EmployeeCountInTeam(empId) != 0)
        //                            {
        //                                var result = await _unitOfWork.Setting.AddTeamMemberInProjectAsync(projectAssignDto);
        //                                if (result != 0)
        //                                {
        //                                    projectAssignDto.Id = result;
        //                                    assignmentResult.Success.Add(projectAssignDto);
        //                                }
        //                                else
        //                                    assignmentResult.Failure.Add(projectAssignDto);
        //                            }
        //                            else
        //                            {
        //                                projectAssignDto.Reason = SharedResources.ErrorNotAuthorizeToAssignProjectToEmployee;
        //                                assignmentResult.Failure.Add(projectAssignDto);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            projectAssignDto.Reason = SharedResources.ProjectIsAlreadyAssigned;
        //                            assignmentResult.Failure.Add(projectAssignDto);
        //                        }
        //                        //}
        //                        //else
        //                        //{
        //                        //    projectAssignDto.Reason = SharedResources.ProjectAndUserDepartmentsDoesNotMatched;
        //                        //    assignmentResult.Failure.Add(projectAssignDto);
        //                        //}

        //                    }
        //                    else
        //                    {
        //                        projectAssignDto.Reason = SharedResources.EmployeeNotFound;
        //                        assignmentResult.Failure.Add(projectAssignDto);
        //                    }
        //                }
        //                if (assignmentResult?.Failure?.Any() == true)
        //                    response.Message.Add(SharedResources.ErrorWhileAssignProjectToEmployee);
        //                else
        //                    response.Message.Add(SharedResources.AssignedProjectToEmployee);
        //                response.Model = assignmentResult;
        //            }
        //            else
        //            {
        //                response.Message.Add(SharedResources.UpdatedMessage);
        //                response.Model = assignmentResult;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message.Add($"An error occurred: {ex.Message}");
        //    }
        //    return response;
        //}

        public async Task<ResponseModel<SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>>> AddTeamMemberInProject(ProjectAssignmentViewModel<List<string>, List<int>> assignDto)
        {
            var response = new ResponseModel<SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>>();
            try
            {
                var assignmentResult = new SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>();

                if (assignDto.ProjectId == null || assignDto.ProjectId?.Any() == false)
                {
                    response.Message.Add(SharedResources.ProvideProjectToAssignMember);
                    return response;
                }

                if (assignDto.EmployeeId == null)
                {
                    response.Message.Add(SharedResources.ProvideMemberToProjectAssign);
                    return response;
                }

                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
                if (claims.Role == "Project Manager")
                {
                    var managerProjectList = await _unitOfWork.Project.GetManagerProjectList(claims.UserId, claims.DepartmentId);
                    var managerProjectIdList = managerProjectList.Select(x => x.Id).ToList();
                    var newProjectIdList = assignDto.ProjectId.Intersect(managerProjectIdList).ToList();

                    if (!newProjectIdList.Any())
                    {
                        response.Message.Add(SharedResources.NoProjectFoundforTheManager);
                        return response;
                    }

                    var newEmployeeList = assignDto.EmployeeId;

                    if (assignDto.EmployeeId?.Any() == false)
                    {
                        newEmployeeList = null;  
                    }
                    
                    await AssignEmployeeToProject(response, assignmentResult, claims, newProjectIdList, newEmployeeList);
                }

                else if (claims.Role == "HOD" || claims.Role == "Admin")
                {
                    var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                    claims.UserId = claims_HOD.UserId;
                    var newProjectIdList = new List<int>();
                    var newEmployeeIdList = new List<string>();


                    foreach (var projectId in assignDto.ProjectId)
                    {
                        var checkProjectDepartmentMatch = await _unitOfWork.Project.GetProject(projectId,
                            claims.Role == "Admin" ? 0:claims_HOD.DepartmentId);
                        if (checkProjectDepartmentMatch != null)
                        {
                            newProjectIdList.Add(projectId);
                        }
                    }

                    if (newProjectIdList?.Any() != true)
                    {
                        {
                            response.Message.Add(SharedResources.ProjectNotFoundForTheDeparment);
                            return response;
                        }
                    }

                    foreach (var employeeId in assignDto.EmployeeId)
                    {
                        var checkEmployeeDepartmentMatch = await _unitOfWork.Employee.GetEmployeeById(employeeId, claims.Role == "Admin" ? 0 : claims_HOD.DepartmentId, string.Empty);
                        if (checkEmployeeDepartmentMatch != null)
                        {
                            newEmployeeIdList.Add(employeeId); 
                        }
                    }

                    if (newEmployeeIdList?.Any() != true)
                    {
                        {
                            response.Message.Add(SharedResources.EmployeeNotFoundForTheDeparment);
                            return response;
                        }
                    }

                    await AssignEmployeeToProject(response, assignmentResult, claims, newProjectIdList, newEmployeeIdList);
                }
                else if (claims.Role == "Team Lead" || claims.Role == "BDM")
                {
                    var newProjectIdList = assignDto.ProjectId;
                    var newEmployeeList = assignDto.EmployeeId;
                    if (assignDto.EmployeeId?.Any() == false)
                    {
                        newEmployeeList = null;
                    }
                    await AssignEmployeeToProject(response, assignmentResult, claims, newProjectIdList, newEmployeeList);
                }
                else
                {
                    response.Message.Add(SharedResources.RoleNotAllowed);
                    return response;
                }
                return response;

                // Handle other roles if necessary

            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (log it, rethrow, etc.)
                throw;
            }
        }

        private async Task AssignEmployeeToProject(ResponseModel<SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>> response, SuccessFailureResultViewModel<ProjectAssignmentResponseViewModel<string, int>>? assignmentResult, UserIdentityModel claims, List<int> newProjectListId, List<string?> newEmployeeList)
        {

            foreach (var projectId in newProjectListId)
            {
                var getListOfEmployeesAssignedToProject = await _unitOfWork.Project.GetAssignedEmployeesToTheProject(projectId, claims.DepartmentId);

                if (getListOfEmployeesAssignedToProject != null && getListOfEmployeesAssignedToProject.Contains(claims.UserId))
                {
                    getListOfEmployeesAssignedToProject.Remove(claims.UserId);
                }

                var employeesToRemoveList = new List<string>();

                if (newEmployeeList == null)
                {
                    employeesToRemoveList = getListOfEmployeesAssignedToProject;
                }

                else if(getListOfEmployeesAssignedToProject != null && getListOfEmployeesAssignedToProject.Any())
                {

                    employeesToRemoveList = getListOfEmployeesAssignedToProject.Except(newEmployeeList).ToList();
                }

                if (employeesToRemoveList?.Any() == true)
                {
                    foreach (var empId in employeesToRemoveList)
                    {
                        await _unitOfWork.Project.RemoveEmployeeFromProject(projectId, empId);
                    }
                }

                if (newEmployeeList != null)
                {
                    var additionalEmployees = new List<string>();

                    int currentIndex = 0; // Track the current position in the list
                    while (currentIndex < newEmployeeList.Count)
                    {
                        var employeeId = newEmployeeList[currentIndex];

                        ProjectAssignmentResponseViewModel<string, int> projectAssignDto = new()
                        {
                            ProjectId = projectId,
                            EmployeeId = employeeId
                        };

                        var checkUsersAndProjectDepartmentMatch = await _unitOfWork.Setting.CheckUsersAndProjectDepartmentMatch(
                            projectAssignDto.ProjectId,
                            employeeId,
                            claims.Role == "Admin" ? employeeId : claims.UserId
                        );

                        if (!checkUsersAndProjectDepartmentMatch)
                        {
                            projectAssignDto.Reason = SharedResources.ProjectAndUserDepartmentsDoesNotMatched;
                            assignmentResult.Failure.Add(projectAssignDto);
                        }
                        else
                        {
                            var employeeDetails = await _unitOfWork.Employee.GetEmployeeById(employeeId, claims.DepartmentId, string.Empty);
                            if (employeeDetails != null && employeeDetails?.TeamAdminId != null)
                            {
                                // Add the new employee directly to the list to be processed
                                if (!newEmployeeList.Contains(employeeDetails.TeamAdminId))
                                {
                                    newEmployeeList.Add(employeeDetails.TeamAdminId);
                                }
                            }

                            var checkProjectIsAlreadyAssignedToEmployee = await _unitOfWork.Setting.IsProjectAlreadyAssigned(
                                projectAssignDto.ProjectId,
                                projectAssignDto.EmployeeId
                            );

                            if (!checkProjectIsAlreadyAssignedToEmployee)
                            {
                                var result = await _unitOfWork.Setting.AddTeamMemberInProjectAsync(projectAssignDto);
                                if (result != 0)
                                {
                                    projectAssignDto.Id = result;
                                    assignmentResult.Success.Add(projectAssignDto);
                                }
                            }
                        }

                        currentIndex++; // Move to the next employee
                    }
                }
                else
                {
                    continue;
                }

            }

            if (assignmentResult?.Failure?.Any() == true)
                response.Message.Add(SharedResources.ErrorWhileAssignProjectToEmployee);
            else
                response.Message.Add(SharedResources.AssignedProjectToEmployee);
            response.Model = assignmentResult;
        }




        public async Task<ResponseModel<List<UsersByRoleViewModel>>> GetUsersByRole(int departmentId, string roleId, string? searchKeyword)
        {
            var response = new ResponseModel<List<UsersByRoleViewModel>>();
            try
            {
                var departmentData = await _unitOfWork.Setting.GetUsersByRole(departmentId, roleId, searchKeyword);
                if (departmentData == null || !departmentData.Any())
                    response.Message.Add(SharedResources.UserNotFoundWithRole);
                else
                    response.Model = _mapper.Map<List<UsersByRoleViewModel>>(departmentData);
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.ErrorWhileGettingUsers);
            }
            return response;
        }

        public async Task<ResponseModel<List<ProjectTeamViewModel>>> GetProjectMembersByProjectIdAsync(int projectId, int departmentId)
        {
            var response = new ResponseModel<List<ProjectTeamViewModel>>();
            try
            {
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
                departmentId = claims.DepartmentId;
                var projectMembers = await _unitOfWork.Setting.GetProjectMembersByProjectIdAsync(projectId, departmentId);
                if (projectMembers == null || projectMembers.Count == 0)
                    response.Message.Add(SharedResources.ProjectMembersNotFound);
                else
                    response.Model = projectMembers;
                return response;
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.ErrorWhileGettingMembers);
            }
            return response;
        }

        public async Task<ResponseModel<List<DropDownResponse<string>>>> GetAssignedUsersByName(string employeeId)
        {
            var response = new ResponseModel<List<DropDownResponse<string>>>();
            var employeeList = await _unitOfWork.Setting.GetAssignedUserByName(employeeId);

            if (employeeList.Count == 0)
            {
                response.Message.Add(SharedResources.NoDataFound);
            }
            else
            {
                response.Model = _mapper.Map<List<DropDownResponse<string>>>(employeeList);
            }

            return response;
        }
        public async Task<ResponseModel<List<EmployeeResponse>>> GetEmployeeByDepartmentIdAsync(string? teamAdminId, int departmentId)
        {
            var response = new ResponseModel<List<EmployeeResponse>>();
            try
            {
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, teamAdminId);
                teamAdminId = claims.UserId;
                departmentId = claims.DepartmentId;
                var employeeList = await _unitOfWork.Setting.GetEmployeeByDepartmentId(teamAdminId, departmentId);
                if (employeeList == null || employeeList.Count == 0)
                    response.Message.Add(SharedResources.NoDataFound);
                else
                    response.Model = employeeList;
                return response;
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.ErrorWhileGettingMembers);
            }
            return response;
        }
        public async Task<ResponseModel<TeamLeadBAAndBDListViewModel>> GetTeamLeadBAAndBDListByDepartmentId(string? teamAdminId, int departmentId)
        {
            var response = new ResponseModel<TeamLeadBAAndBDListViewModel>();
            try
            {
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, teamAdminId);
                var employeeList = await _unitOfWork.Setting.GetTeamLeadBAAndBDListByDepartmentId(claims.UserId, claims.DepartmentId);
                if (employeeList == null)
                    response.Message.Add(SharedResources.NoDataFound);
                else
                {
                    employeeList.TeamLead = employeeList.TeamLead.Where(a => a.Id != claims.UserId).ToList();
                    employeeList.BDM = employeeList.BDM.Where(a => a.Id != claims.UserId).ToList();
                    response.Model = employeeList;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.ErrorWhileGettingMembers);
            }
            return response;
        }

        public async Task<ResponseModel<dynamic>> GetProductiveHoursByDepartmentIdAsync(AssignBadgeToEmployeesViewModel model)
        {
            var response = new ResponseModel<dynamic>();

            int totalWorkingDays = await SharedResources.GetWorkingDaysCount(model.Month, model.Year);
            int BadgeId = 1004;
            var getProductiveHoursEmployeeList = await _unitOfWork.Setting.GetEmployeeListWhoseProductiveHoursIsHigh(totalWorkingDays, model.DepartmentId, model.Month, model.Year, BadgeId);

            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, model.DepartmentId, string.Empty);

            if (getProductiveHoursEmployeeList != null && getProductiveHoursEmployeeList.Count > 0)
            {
                response.Model = new List<int>();
                foreach (var employee in getProductiveHoursEmployeeList)
                {
                    if (employee.TotalBillingHours >= employee.ExpectedProductivity)
                    {
                        var awardModel = new AssignAwardViewModel
                        {
                            BadgeId = 1004, // Example BadgeId
                            UserId = employee.Id.ToString(),
                            BadgeDescription = "Top Performer"
                        };

                        var awardAssigned = await _unitOfWork.Employee.AssignAwardToEmployee(awardModel, DateOnly.FromDateTime(DateTime.Now), claims.UserId);
                        if (awardAssigned != 0)
                        {
                            response.Model.Add(awardAssigned);
                        }
                    }
                }
                response.Message.Add(SharedResources.SuccessfullyAddedAward);
            }
            else
            {
                response.Message.Add(SharedResources.NoDataFound);

            }
            return response;
        }

        public async Task<ResponseModel<bool>> UpdateCanEditStatusByTeamAdminIdOrDepartmentId(UpdateCanEditStatusViewModel updateCanEditStatus)
        {
            var response = new ResponseModel<bool>();
            if (updateCanEditStatus.DepartmentId == 0 && string.IsNullOrEmpty(updateCanEditStatus.TeamAdminId))
            {
                response.Message.Add(SharedResources.ProvideDepartmentOrTeamAdmin);
                return response;
            }

            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, updateCanEditStatus.DepartmentId, updateCanEditStatus.TeamAdminId);

            var editCanEditStatus = await _unitOfWork.Setting.UpdateCanEditStatusByTeamAdminIdOrDepartmentId
            (claims.DepartmentId, claims.UserId, updateCanEditStatus.CanEditStatus);
            if (editCanEditStatus)
            {
                response.Message.Add(SharedResources.CanEditStatusUpdated);
                response.Model = true;
            }
            else
            {
                response.Message.Add(SharedResources.ErrorWhileUpdatingCanEditStatus);
                response.Model = false;
            }
            return response;
        }

        //public async Task<ResponseModel<bool>> IsProjectAssigned(int projectId, string userId)
        //{
        //    var response = new ResponseModel<bool>();
        //    var isProjectAssigned = await _unitOfWork.Setting.IsProjectAlreadyAssigned(projectId, userId);
        //    if (isProjectAssigned)
        //        response.Message.Add(SharedResources.NoDataFound);
        //    else
        //        response.Model = isProjectAssigned;
        //    return response;
        //}
    }
}
