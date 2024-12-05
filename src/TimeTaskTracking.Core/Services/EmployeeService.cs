using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Shared.ViewModels.Project;


namespace TimeTaskTracking.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment, ILogger<EmployeeService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = contextAccessor;
        }

        //public async Task<ResponseModel<bool>> GetEmployeesById(string employeeId, int departmentId)
        //{
        //    var result = new ResponseModel<bool>();
        //    var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId);
        //    departmentId = claims.DepartmentId;
        //    var getEmployeeDetails = await _unitOfWork.Employee.GetEmployeeById(employeeId, departmentId, claims.UserId);
        //    if (!getEmployeeDetails)
        //        result.Message.Add(SharedResources.UserNotFound);
        //    result.Model = getEmployeeDetails;
        //    return result;
        //}
        public async Task<ResponseModel<EmployeeDto>> GetEmployee(string employeeId, int departmentId)
        {
            var result = new ResponseModel<EmployeeDto>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            departmentId = claims.DepartmentId;
            var getEmployeeDetails = await _unitOfWork.Employee.GetEmployeeById(employeeId, departmentId, string.Empty);
            if (getEmployeeDetails == null)
                result.Message.Add(SharedResources.UserNotFound);
            result.Model = _mapper.Map<EmployeeDto>(getEmployeeDetails);
            return result;
        }

        public async Task<ResponseModel<dynamic>> GetEmployeeProfileDetails(string employeeId, int departmentId)
        {
            try
            {
                var result = new ResponseModel<dynamic>();
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
                departmentId = claims.DepartmentId;
                dynamic userDetail = null;
                if (claims.Role == "Team Lead")
                {
                    userDetail = await _unitOfWork.Employee.GetEmployeeDetailsIfAssigned(employeeId, claims.UserId);
                    if (userDetail == null)
                    {
                        result.Message.Add(SharedResources.UserNotFound);
                    }
                    else
                    {
                        result.Model = userDetail;
                    }
                }
                else
                {
                    userDetail = await _unitOfWork.Employee.GetEmployeeProfileById(employeeId, departmentId, string.Empty, claims.Role);
                    if (userDetail == null)
                    {
                        result.Message.Add(SharedResources.UserNotFound);
                        result.Model = userDetail;
                    }
                }
                if (userDetail != null && (userDetail.Designation == "Stipend" || userDetail.Designation == "Trainee"))
                {
                    var getUserDeatils = await _unitOfWork.Employee.GetOnRollFeedbackDetails(employeeId);
                    if (getUserDeatils == null)
                    {
                        result.Message.Add(SharedResources.NoDataFound);
                    }
                    userDetail.FeedbackDetails = getUserDeatils;
                    result.Model = userDetail;
                }
                else
                    result.Model = userDetail;
                return result;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<ResponseModel<List<DropDownResponse<int>>>> GetAllDesignations(int departmentId)
        {
            var getDesignationList = await _unitOfWork.Employee.GetDesignationList(departmentId);
            return new ResponseModel<List<DropDownResponse<int>>>()
            {
                Model = _mapper.Map<List<DropDownResponse<int>>>(getDesignationList)
            };
        }

        public async Task<ResponseModel<ManagerTeamLeadBAAndBDListViewModel>> GetProjectManagerOrTeamLeadOrBDMListByDepartment(int departmentId)
        {
            var response = new ResponseModel<ManagerTeamLeadBAAndBDListViewModel>();
            try
            {
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
                var employeeList = await _unitOfWork.Employee.GetProjectManagerOrTeamLeadOrBDMListByDepartment(claims.DepartmentId);
                if (employeeList == null)
                    response.Message.Add(SharedResources.NoDataFound);
                else
                {
                    employeeList.Manager = employeeList.Manager.Where(a => a.Id != claims.UserId).ToList();
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

        public async Task<ResponseModel<List<DropDownResponse<int>>>> GetAllDepartments()
        {
            var getDepartmentList = await _unitOfWork.Employee.GetDepartmentList();
            return new ResponseModel<List<DropDownResponse<int>>>()
            {
                Model = _mapper.Map<List<DropDownResponse<int>>>(getDepartmentList)
            };
        }

        public async Task<ResponseModel<List<DropDownResponse<string>>>> GetManagerList(int departmentId)
        {
            var response = new ResponseModel<List<DropDownResponse<string>>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            var managerList = await _unitOfWork.Employee.GetManagerListAsync(claims.DepartmentId);

            if (managerList != null)
            {
                //if (claims.Role == "Project Manager")
                //{
                //    var removeManager = managerList.SingleOrDefault(manager => manager.Id == claims.UserId);
                //    if (removeManager != null)
                //    {
                //        managerList.Remove(removeManager);
                //    }
                //}
                response.Model = _mapper.Map<List<DropDownResponse<string>>>(managerList);
            }
            else
                response.Message.Add(SharedResources.NoManagerFound);
            return response;
        }

        public async Task<ResponseModel<dynamic>> GetEmployeesByFilter(EmployeeModel employeeModel)
        {
            var result = new ResponseModel<dynamic>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, employeeModel.DepartmentId, employeeModel.TeamAdminId);
            employeeModel.DepartmentId = claims.DepartmentId;
            if (employeeModel.IsActive.HasValue)
            {
                if (employeeModel.IsActive == 3)
                    employeeModel.IsActive = null;
            }
            if (claims.Role == "Team Lead")
            {
                var getEmployeeDetail = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleaders(employeeModel, claims.UserId);
                if (getEmployeeDetail == null)
                {
                    result.Message.Add(SharedResources.NoEmployeeFound);
                    return result;
                }

                // Map to EmployeeDetailDto and fetch badges for each employee
                var mappedEmployees = _mapper.Map<List<EmployeeDetailsViewModel>>(getEmployeeDetail.results);

                var employeeDetails = new PaginationResponseViewModel<List<EmployeeDetailsViewModel>>();
                //var loggedInUserCount = mappedEmployees.Count(a => a.Id == claims.UserId);
                //if (loggedInUserCount != 0)
                //    getEmployeeDetail.TotalCount = getEmployeeDetail.TotalCount - loggedInUserCount;
                employeeDetails.results = mappedEmployees.ToList();
                employeeDetails.TotalCount = getEmployeeDetail.TotalCount;
                result.Model = employeeDetails;
            }
            else if (claims.Role == "HR")
            {
                employeeModel.TeamAdminId = claims.UserId;
                var getAllEmployeeDetail = await _unitOfWork.Employee.GetAllEmployeeByHR(employeeModel);
                if (getAllEmployeeDetail == null)
                {
                    result.Message.Add(SharedResources.NoEmployeeFound);
                    return result;
                }
                var mappedEmployees = _mapper.Map<List<EmployeeDto>>(getAllEmployeeDetail.results);

                result.Model = new PaginationResponseViewModel<List<EmployeeDto>>
                {

                    results = mappedEmployees.ToList(),
                    TotalCount = getAllEmployeeDetail.TotalCount,
                };
            }
            else
            {
                if (claims.Role == "Project Manager")
                {
                    employeeModel.TeamAdminId = claims.UserId;
                }
                var filteredEmployees = await _unitOfWork.Employee.GetFilteredEmployees(employeeModel, claims.Role);
                if (filteredEmployees?.results?.Any() == true)
                {
                    var mappedDto = _mapper.Map<List<EmployeeDto>>(filteredEmployees.results);
                    var employeeDetails = new PaginationResponseViewModel<List<EmployeeDto>>();
                    //var loggedInUserCount = mappedDto.Count(a => a.EmployeeID == claims.UserId);
                    //if (loggedInUserCount != 0)
                    //    filteredEmployees.TotalCount = filteredEmployees.TotalCount - loggedInUserCount;
                    employeeDetails.results = mappedDto.ToList();
                    employeeDetails.TotalCount = filteredEmployees.TotalCount;
                    result.Model = employeeDetails;
                }
                else
                    result.Message.Add(SharedResources.NoEmployeeFound);
            }
            return result;
        }

        public async Task<ResponseModel<PaginationResponseViewModel<List<UserDetailsViewModel>>>> GetAllUsers(UserViewModel model)
        {
            var result = new ResponseModel<PaginationResponseViewModel<List<UserDetailsViewModel>>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, model.DepartmentId, string.Empty);
            model.DepartmentId = claims.DepartmentId;
            var getAllUsers = await _unitOfWork.Employee.GetAllUsersAsync(model);
            if (getAllUsers != null)
            {
                result.Model = getAllUsers;
            }
            else
                result.Message.Add(SharedResources.NoUserFound);
            return result;
        }




        public async Task<ResponseModel<bool>> DeleteEmployeeById(string employeeId, int departmentId)
        {
            var response = new ResponseModel<bool>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            departmentId = claims.DepartmentId;

            if (claims.UserId != employeeId)
            {
                var employee = await _unitOfWork.UserProfile.GetUserProfileByIdAsync(employeeId);
                if (employee != null)
                {
                    if (employee.Role != claims.Role)
                    {
                        var deleteStatus = await _unitOfWork.Employee.DeleteEmployeeById(employeeId, departmentId);
                        if (deleteStatus)
                        {
                            // Delete the profile image if it exists
                            if (!string.IsNullOrEmpty(employee.ProfileImage))
                                SharedResources.DeleteProfileImage(employee.ProfileImage, _webHostEnvironment, _logger);
                            response.Message.Add(SharedResources.EmployeeRemoved);
                        }
                        else
                        {
                            response.Message.Add(SharedResources.FailedToDeleteEmployee);
                        }
                        response.Model = deleteStatus;
                        return response;
                    }
                    else
                        response.Message.Add(SharedResources.NoAccessToEditUser);
                }
                else
                    response.Message.Add(SharedResources.UserNotFound);
                response.Model = false;
            }
            else
            {
                response.Message.Add(SharedResources.CantDeleteYourself);
            }

            return response;
        }


        //public async Task<ResponseModel<string>> UpdateEmployeeStatusById(string employeeId, int isActive)
        //{
        //    var response = new ResponseModel<string>();
        //    var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0);
        //    var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(employeeId, claims.DepartmentId, claims.UserId);
        //    if (checkEmployeeExists)
        //    {
        //        var updateEmpStatus = await _unitOfWork.Employee.UpdateEmployeeStatusById(employeeId, isActive, claims.DepartmentId, claims.UserId);
        //        if (updateEmpStatus == null)
        //            response.Message.Add(SharedResources.ErrorWhileUpdating);
        //        response.Message.Add(SharedResources.EmployeeStatusUpdated);
        //        response.Model = updateEmpStatus;
        //        return response;
        //    }
        //    response.Message.Add(SharedResources.UserNotFound);
        //    return response;
        //}

        public async Task<ResponseModel<string>> UpdateEmployeeManagerAndStatus(UpdateManagerandStatusDto updateManagerandStatusModel)
        {
            var response = new ResponseModel<string>();
            if (string.IsNullOrEmpty(updateManagerandStatusModel.TeamAdminId) && !updateManagerandStatusModel.IsActive.HasValue)
            {
                response.Message.Add(SharedResources.TeamAdminIdOrIsActiveRequired);
                return response;
            }
            else if (!string.IsNullOrEmpty(updateManagerandStatusModel.TeamAdminId) && updateManagerandStatusModel.IsActive.HasValue)
            {
                response.Message.Add(SharedResources.OnlyOneFieldAllowedTeamAdminIdAndIsActive);
                return response;
            }
            if (!updateManagerandStatusModel.IsActive.HasValue)
                if (!SharedResources.IsValidEnumValue(updateManagerandStatusModel.IsActive ?? 0, new int[] { 0, 1 }))
                {
                    response.Message.Add(SharedResources.IsActiveStatusValidForEmployee);
                    return response;
                }

            if (updateManagerandStatusModel.IsActive == 2)
            {
                response.Message.Add(SharedResources.IsActiveStatusValidForEmployee);
                return response;
            }

            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, updateManagerandStatusModel.TeamAdminId);

            var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(updateManagerandStatusModel.EmployeeId, claims.DepartmentId, string.Empty);
            if (checkEmployeeExists != null)
            {
                if (checkEmployeeExists.RoleName != claims.Role)
                {
                    if (updateManagerandStatusModel.IsActive != null)
                    {
                        if (checkEmployeeExists.IsActive != updateManagerandStatusModel.IsActive)
                        {
                            if (checkEmployeeExists.IsActive == 2 && updateManagerandStatusModel.IsActive == 0)
                            {
                                var deleteEmployee = await _unitOfWork.Employee.DeleteEmployeeById(updateManagerandStatusModel.EmployeeId, claims.DepartmentId);
                                if (deleteEmployee)
                                {
                                    response.Message.Add(SharedResources.EmployeeStatusUpdated);
                                    response.Model = deleteEmployee.ToString();
                                }
                                else
                                    response.Message.Add(SharedResources.ErrorWhileUpdating);
                                return response;
                            }
                            else if (checkEmployeeExists.IsActive == 2 && updateManagerandStatusModel.IsActive == 1)
                            {
                                var managerId = string.Empty;
                                if (claims.Role == "HOD")
                                {
                                    var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                                    managerId = claims_HOD.UserId;
                                }
                                if (claims.Role == "Project Manager")
                                {
                                    managerId = claims.UserId;
                                }

                                if (claims.Role == "Admin")
                                {
                                    var getHODByDepartmentId = await _unitOfWork.Employee.GetHODByDepartmentIdAsync(checkEmployeeExists.DepartmentId);
                                    if (getHODByDepartmentId != null)
                                    {
                                        managerId = getHODByDepartmentId;
                                    }
                                    else
                                    {
                                        var getManagerListByDepartmentId = await _unitOfWork.Employee.GetManagerListByDepartmentIdAsync(checkEmployeeExists.DepartmentId);
                                        if (getManagerListByDepartmentId?.Any() == true)
                                        {
                                            if (getManagerListByDepartmentId.Count > 1)
                                            {
                                                var oldestManger = getManagerListByDepartmentId.OrderBy(e => e.JoiningDate ?? DateTime.MaxValue).FirstOrDefault();
                                                managerId = oldestManger.Id;
                                            }
                                            else
                                                managerId = getManagerListByDepartmentId.FirstOrDefault()?.Id;
                                        }
                                        else
                                        {
                                            response.Message.Add(SharedResources.InvalidManager);
                                            return response;

                                        }
                                    }
                                }
                                var isActiveStatus = await _unitOfWork.Employee.UpdateEmployeeStatusById(updateManagerandStatusModel.EmployeeId, updateManagerandStatusModel.IsActive, claims.DepartmentId, managerId);
                                if (isActiveStatus == null)
                                {
                                    response.Message.Add(SharedResources.ErrorWhileUpdating);
                                    return response;
                                }
                                else
                                {
                                    response.Message.Add(SharedResources.EmployeeStatusUpdated);
                                    if (updateManagerandStatusModel.IsActive == 0)
                                        await _unitOfWork.Employee.RemoveEmployeeFromTeamAsync(updateManagerandStatusModel.EmployeeId);
                                }

                                response.Model = isActiveStatus;
                                return response;
                            }
                            else
                            {
                                var isActiveStatus = await _unitOfWork.Employee.UpdateEmployeeStatusById(updateManagerandStatusModel.EmployeeId, updateManagerandStatusModel.IsActive, claims.DepartmentId, checkEmployeeExists.TeamAdminId);
                                if (isActiveStatus == null)
                                {
                                    response.Message.Add(SharedResources.ErrorWhileUpdating);
                                    return response;
                                }

                                else
                                {
                                    response.Message.Add(SharedResources.EmployeeStatusUpdated);
                                    if (updateManagerandStatusModel.IsActive == 0)
                                        await _unitOfWork.Employee.RemoveEmployeeFromTeamAsync(updateManagerandStatusModel.EmployeeId);
                                }

                                response.Model = isActiveStatus;
                                return response;
                            }
                        }
                        else
                        {
                            response.Message.Add(SharedResources.ProvideValidEmployeeActiveStatus);
                            return response;
                        }
                    }
                    if (updateManagerandStatusModel.TeamAdminId != null)
                    {
                        if (checkEmployeeExists.RoleName == "Project Manager" || checkEmployeeExists.RoleName == "HOD")
                        {
                            response.Message.Add(SharedResources.UnableToUpdateTheManager);
                            response.Model = "Failed";
                            return response;
                        }
                        else
                        {
                            var updateEmployeeManager = await _unitOfWork.Employee.UpdateEmployeeManagerAsync(updateManagerandStatusModel.EmployeeId, claims.DepartmentId, updateManagerandStatusModel.TeamAdminId);
                            if (updateEmployeeManager == null)
                            {
                                response.Message.Add(SharedResources.TeamAdminIdNotFound);
                                return response;
                            }
                            else
                            {
                                await _unitOfWork.Employee.RemoveEmployeeFromTeamAsync(updateManagerandStatusModel.EmployeeId);
                                response.Message.Add(SharedResources.ProjectManagerAssigned);
                                response.Model = updateEmployeeManager;
                            }
                            return response;
                        }
                    }
                }
                else
                {
                    response.Message.Add(SharedResources.NoAccessToEditUser);
                    return response;
                }
            }
            else
                response.Message.Add(SharedResources.UserNotFound);
            return response;
        }

        public async Task<ResponseModel<string>> UpdateEmployeeProfileByManager(UpdateEmployeeProfileByManagerViewModel profileModel)
        {
            var response = new ResponseModel<string>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(profileModel.EmployeeId, claims.DepartmentId, string.Empty);
            if (checkEmployeeExists != null)
            {
                if (checkEmployeeExists.RoleName != claims.Role)
                {
                    var updateEmpStatus = await _unitOfWork.Employee.UpdateEmployeeProfileByManagerAsync(profileModel);
                    if (updateEmpStatus == null)
                    {
                        response.Message.Add(SharedResources.ErrorWhileUpdating);
                        return response;
                    }
                    else
                    {
                        response.Message.Add(SharedResources.EmployeeDetailsUpdated);
                        response.Model = updateEmpStatus;
                    }
                }
                else
                    response.Message.Add(SharedResources.NoAccessToEditUser);
                return response;
            }
            response.Message.Add(SharedResources.UserNotFound);
            return response;
        }

        public async Task<ResponseModel<List<DropDownResponse<string>>>> GetEmployeeByTeamLeader(string teamLeaderId)
        {
            var response = new ResponseModel<List<DropDownResponse<string>>>();
            var employeeList = await _unitOfWork.Employee.GetEmployeeNameByTL(teamLeaderId);
            if (employeeList.Count == 0)
            {
                response.Message.Add(SharedResources.NoEmployeeFound);
            }
            else
            {
                response.Model = _mapper.Map<List<DropDownResponse<string>>>(employeeList);
            }
            return response;
        }

        public async Task<ResponseModel<int>> AssignEmployeeAward(AssignAwardViewModel awardModel)
        {
            var response = new ResponseModel<int>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(awardModel.UserId, 0, string.Empty);
            if (checkEmployeeExists == null)
            {
                response.Message.Add(SharedResources.NoEmployeeFound);
                return response;
            }

            var badgeExists = await _unitOfWork.Employee.GetBadgeExistanceById(awardModel.BadgeId);

            if (!badgeExists)
            {
                response.Message.Add(SharedResources.NoBatchFound);
                return response;
            }

            if (claims.Role == "Project Manager")
            {
                if (checkEmployeeExists.TeamAdminId != claims.LoggedInUserId)
                {
                    response.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                    return response;
                }
            }
            if (claims.Role == "HOD")
            {
                var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                if (checkEmployeeExists.DepartmentId != claims_HOD.DepartmentId)
                {
                    response.Message.Add(SharedResources.InvalidDepartment);
                    return response;
                }
            }
            if (claims.Role == "Team Lead")
            {
                var getEmployeeByTeamLead = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.LoggedInUserId);
                bool checkEmployeeIsUnderTeamLead = getEmployeeByTeamLead.Any(employee => employee.Id == awardModel.UserId);
                if (checkEmployeeIsUnderTeamLead == false)
                {
                    response.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                    return response;
                }
            }


            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Today);
            var dateRecieved = currentDate;
            var submittedBy = claims.LoggedInUserId;

            var awardAssigned = await _unitOfWork.Employee.AssignAwardToEmployee(awardModel, dateRecieved, submittedBy);
            if (awardAssigned != 0)
            {
                response.Message.Add(SharedResources.SuccessfullyAddedAward);
                response.Model = awardAssigned;
            }
            else
                response.Message.Add(SharedResources.FailedToAssignAward);
            return response;
        }

        public async Task<ResponseModel<bool>> GetAwardById(int BadgeId)
        {
            var response = new ResponseModel<bool>();
            var badgeExists = await _unitOfWork.Employee.GetBadgeExistanceById(BadgeId);
            if (badgeExists)
            {
                response.Model = badgeExists;
            }
            response.Message.Add(SharedResources.NoBatchFound);
            return response;
        }

        public async Task<ResponseModel<bool>> GetDepartmentById(int departmentId)
        {
            var response = new ResponseModel<bool>();
            var checkDepartmentExists = await _unitOfWork.Employee.GetDepartmentExistById(departmentId);
            if (checkDepartmentExists)
            {
                response.Model = checkDepartmentExists;
            }
            else
                response.Message.Add(SharedResources.DepartmentNotFound);
            return response;
        }

        public async Task<ResponseModel<bool>> DeleteAssignAwards(string employeeId, int badgeId)
        {

            var response = new ResponseModel<bool>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(employeeId, 0, string.Empty);

            if (checkEmployeeExists == null)
            {
                response.Message.Add(SharedResources.EmployeeNotFound);
                response.Model = false;
                return response;
            }

            if (claims.Role == "Team Lead")
            {
                var getEmployeeByTeamLead = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.LoggedInUserId);
                bool checkEmployeeIsUnderTeamLead = getEmployeeByTeamLead.Any(employee => employee.Id == employeeId);
                if (checkEmployeeIsUnderTeamLead == false)
                {
                    response.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                    return response;
                }
            }

            if (claims.Role == "Project Manager")
            {
                if (checkEmployeeExists.TeamAdminId != claims.LoggedInUserId)
                {
                    response.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                    return response;
                }
            }

            if (claims.Role == "HOD")
            {
                var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                if (checkEmployeeExists.DepartmentId != claims_HOD.DepartmentId)
                {
                    response.Message.Add(SharedResources.InvalidDepartment);
                    return response;
                }
            }

            var checkBadgeIsAssigned = await _unitOfWork.Employee.GetAssignedBadgeDetails(employeeId, badgeId);
            if (checkBadgeIsAssigned == null)
            {
                response.Message.Add(SharedResources.InvalidBadgeId);
                response.Model = false;
                return response;
            }

            var deleteAssignAwards = await _unitOfWork.Employee.DeleteAssignAwards(employeeId, badgeId);
            if (deleteAssignAwards)
            {
                response.Message.Add(SharedResources.BadgesDeleteMessage);
                response.Model = true;
            }
            else
            {
                response.Message.Add(SharedResources.ErrorWhileDeletingBadges);
                response.Model = false;
            }
            return response;

        }

        public async Task<ResponseModel<int>> AddMonthlyFeedbackForm(MonthlyFeedbackFormDto monthlyFeedback)
        {
            var response = new ResponseModel<int>();

            if (!Enum.IsDefined(typeof(Performance), monthlyFeedback.Performance))
            {
                response.Message.Add(SharedResources.InvalidPerformanceType);
                return response;
            }

            var (feedbackForm, errorMessage) = await TraineeFeedbackForm(monthlyFeedback);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                response.Message.Add(errorMessage);
                return response;
            }

            if (feedbackForm != null)
            {
                var addFeedbackForm = await _unitOfWork.Employee.AddMonthlyTraineeFeedback(feedbackForm);
                if (addFeedbackForm != 0)
                {
                    response.Message.Add(SharedResources.SuccessfullyAddedMonthlyFeedback);
                    response.Model = addFeedbackForm;
                }
                else
                {
                    response.Message.Add(SharedResources.FailedToAddMonthlyFeedback);
                }
            }

            return response;
        }

        public async Task<ResponseModel<dynamic>> GetMonthlyTraineeFeedback(string employeeId, int departmentId)
        {
            var response = new ResponseModel<dynamic>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            departmentId = claims.DepartmentId;
            var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(employeeId, claims.DepartmentId, string.Empty);
            if (checkEmployeeExists == null)
            {
                response.Message.Add(SharedResources.NoEmployeeFound);
                return response;
            }
            if (checkEmployeeExists.Designation == "Stipend" || checkEmployeeExists.Designation == "Trainee")
            {
                if (claims.Role == "Project Manager" || claims.Role == "HOD" || claims.Role == "Admin")
                {
                    await GetTraineeFeedbackFormList(employeeId, departmentId, response);
                    return response;
                }
                else if (claims.Role == "Team Lead")
                {
                    var getEmployeeDetailIfAssigned = await _unitOfWork.Employee.GetEmployeeDetailsIfAssigned(employeeId, claims.UserId);
                    if (getEmployeeDetailIfAssigned == null)
                    {
                        response.Message.Add(SharedResources.TraineeNotUnderTeamLead);
                        return response;
                    }
                    else
                    {
                        await GetTraineeFeedbackFormList(employeeId, departmentId, response);
                        return response;
                    }
                }
                else if (claims.UserId == employeeId)
                {
                    await GetTraineeFeedbackFormList(employeeId, departmentId, response);
                    return response;
                }
                else
                {
                    response.Message.Add(SharedResources.FailedToGetStipendOrTrainee);
                    return response;
                }
            }
            else
            {
                response.Message.Add(SharedResources.FailedToGetStipendOrTrainee);
                return response;
            }
        }

        private async Task GetTraineeFeedbackFormList(string employeeId, int departmentId, ResponseModel<dynamic> response)
        {
            var departmenName = string.Empty;
            if (departmentId > 0)
            {
                var departmentResponse = await _unitOfWork.Employee.GetDepartmentNameById(departmentId);
                departmenName = departmentResponse.Name;
            }

            var getTraineeFeedback = await _unitOfWork.Employee.GetMonthlyTraineeFeedbackDetails(employeeId, departmenName);
            if (getTraineeFeedback == null)
            {
                response.Message.Add(SharedResources.NoDataFound);
            }
            var mappedEmployees = _mapper.Map<List<MonthlyTraineeFeedbackViewModel>>(getTraineeFeedback);
            response.Model = mappedEmployees;
        }

        public async Task<ResponseModel<int>> UpdateMonthlyFeedbackForm(MonthlyFeedbackFormDto monthlyFeedbackForm)
        {
            var response = new ResponseModel<int>();

            if (!Enum.IsDefined(typeof(Performance), monthlyFeedbackForm.Performance))
            {
                response.Message.Add(SharedResources.InvalidPerformanceType);
                return response;
            }

            var (feedbackForm, errorMessage) = await TraineeFeedbackForm(monthlyFeedbackForm);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                response.Message.Add(errorMessage);
                return response;
            }

            if (feedbackForm != null)
            {
                var assessmentDate = monthlyFeedbackForm.AssessmentMonth;
                var AssessmentMonth = monthlyFeedbackForm.AssessmentMonth?.Month ?? 0;
                var AssessmentYear = monthlyFeedbackForm.AssessmentMonth?.Year ?? 0;
                var checkFeedBackFormExistsForTheAssessmentMonth = await _unitOfWork.Employee.GetTraineeFeedbackFormByDate(AssessmentMonth, AssessmentYear, monthlyFeedbackForm.ApplicationUserId);

                if (checkFeedBackFormExistsForTheAssessmentMonth != null && checkFeedBackFormExistsForTheAssessmentMonth.FeedBackId != monthlyFeedbackForm.FeedBackId)
                {
                    response.Message.Add(SharedResources.CantUpdateTheFeedBackForm);
                    response.Model = -1;
                }
                else
                {
                    var updateFeedbackForm = await _unitOfWork.Employee.UpdateMonthlyTraineeFeedback(feedbackForm);
                    if (updateFeedbackForm != 0)
                    {
                        response.Message.Add(SharedResources.SuccessfullyUpdatedMonthlyFeedback);
                        response.Model = updateFeedbackForm;
                    }
                    else
                    {
                        response.Message.Add(SharedResources.FailedToUpdateMonthlyFeedback);
                    }
                }

            }
            return response;
        }

        private async Task<(FeedbackForm feedbackForm, string errorMessage)> TraineeFeedbackForm(MonthlyFeedbackFormDto monthlyFeedbackFormDto)
        {
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(monthlyFeedbackFormDto.ApplicationUserId, claims.DepartmentId, string.Empty);

            if (checkEmployeeExists == null)
            {
                return (null, SharedResources.NoEmployeeFound);
            }

            if (checkEmployeeExists.Designation == "Stipend" || checkEmployeeExists.Designation == "Trainee")
            {
                var departmentResponse = await _unitOfWork.Employee.GetDepartmentNameById(checkEmployeeExists.DepartmentId);
                var departmentName = departmentResponse.Name;
                var employeeName = $"{checkEmployeeExists.FirstName} {checkEmployeeExists.LastName}";
                var employeeDesignation = checkEmployeeExists.Designation;
                var employeeDepartment = departmentName;
                var employeeDOJ = checkEmployeeExists.JoiningDate;

                var feedbackForm = _mapper.Map<FeedbackForm>(monthlyFeedbackFormDto);
                feedbackForm.Name = employeeName;
                feedbackForm.Designation = employeeDesignation;
                feedbackForm.Department = employeeDepartment;
                feedbackForm.DOJ = employeeDOJ;

                return (feedbackForm, null);
            }

            return (null, SharedResources.FailedToAddOrUpdateFeedbackForNonStipendOrTrainee);
        }

        public async Task<ResponseModel<MonthlyFeedbackFormDto>> GetTraineeFeedbackById(int id, string employeeId)
        {
            var result = new ResponseModel<MonthlyFeedbackFormDto>();
            try
            {
                var response = await _unitOfWork.Employee.GetTraineeFeedbackFormById(id, employeeId);
                if (response == null)
                    result.Message.Add(SharedResources.TraineeFeedbackFormNotFound);
                else
                    result.Model = _mapper.Map<MonthlyFeedbackFormDto>(response);
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }

        public async Task<ResponseModel<MonthlyFeedbackFormDto>> GetTraineeFeedbackByDate(int month, int year, string employeeId)
        {
            var result = new ResponseModel<MonthlyFeedbackFormDto>();
            try
            {
                var response = await _unitOfWork.Employee.GetTraineeFeedbackFormByDate(month, year, employeeId);
                if (response != null)
                    result.Model = _mapper.Map<MonthlyFeedbackFormDto>(response);
                else
                    result.Message.Add(SharedResources.TraineeFeedbackAlreadySubmitted);
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }

        public async Task<ResponseModel<bool>> DeleteTraineeFeedback(int feedBackId)
        {
            var response = new ResponseModel<bool>();

            var deleteFeedbackForm = await _unitOfWork.Employee.DeleteTraineeFeedbackForm(feedBackId);
            if (deleteFeedbackForm)
            {
                response.Message.Add(SharedResources.FeedbackFormDeleteMessage);
                response.Model = true;
            }
            else
            {
                response.Message.Add(SharedResources.ErrorWhileDeletingFeedbackForm);
                response.Model = false;
            }
            return response;
        }

        public async Task<ResponseModel<int>> AddOnRollFeedbackForm(AddOnRollFeedbackFormDto addOnRollFeedbackFormDto)
        {
            var response = new ResponseModel<int>();

            if (!Enum.IsDefined(typeof(StartSalary), addOnRollFeedbackFormDto.StartSalary))
            {
                response.Message.Add(SharedResources.InvalidStartSalaryType);
                return response;
            }
            var feedbackFormCount = await _unitOfWork.Employee.GetFeedbackCount(addOnRollFeedbackFormDto.ApplicationUserId);
            var count = feedbackFormCount.FeedbackCount;
            if (count <= 6)
            {
                response.Message.Add(SharedResources.AddFeedbackFormForSixMonths);
                return response;
            }
            var (feedbackForm, errorMessage) = await OnRollFeedbackForm(addOnRollFeedbackFormDto);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                response.Message.Add(errorMessage);
                return response;
            }
            else if (feedbackForm != null)
            {
                var addFeedbackForm = await _unitOfWork.Employee.AddOnRollFeedback(feedbackForm);
                if (addFeedbackForm != 0)
                {
                    response.Message.Add(SharedResources.SuccessfullyAddedMonthlyFeedback);
                    response.Model = addFeedbackForm;
                }
                else
                {
                    response.Message.Add(SharedResources.FailedToAddMonthlyFeedback);
                }
            }

            return response;
        }


        public async Task<ResponseModel<int>> UpdateOnRollFeedbackForm(AddOnRollFeedbackFormDto addOnRollFeedbackForm)
        {
            var response = new ResponseModel<int>();


            if (!Enum.IsDefined(typeof(StartSalary), addOnRollFeedbackForm.StartSalary))
            {
                response.Message.Add(SharedResources.InvalidStartSalaryType);
                return response;
            }

            var (feedbackForm, errorMessage) = await OnRollFeedbackForm(addOnRollFeedbackForm);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                response.Message.Add(errorMessage);
                return response;
            }

            if (feedbackForm != null)
            {
                var addFeedbackForm = await _unitOfWork.Employee.UpdateOnRollFeedback(feedbackForm);
                if (addFeedbackForm != 0)
                {
                    response.Message.Add(SharedResources.SuccessfullyUpdatedMonthlyFeedback);
                    response.Model = addFeedbackForm;
                }
                else
                {
                    response.Message.Add(SharedResources.FailedToUpdateMonthlyFeedback);
                }
            }

            return response;
        }

        private async Task<(FeedbackForm feedbackForm, string errorMessage)> OnRollFeedbackForm(AddOnRollFeedbackFormDto feedbackFormDto)
        {
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(feedbackFormDto.ApplicationUserId, claims.DepartmentId, string.Empty);

            if (checkEmployeeExists == null)
            {
                return (null, SharedResources.NoEmployeeFound);
            }

            if (checkEmployeeExists.Designation == "Stipend" || checkEmployeeExists.Designation == "Trainee")
            {
                var departmentResponse = await _unitOfWork.Employee.GetDepartmentNameById(checkEmployeeExists.DepartmentId);
                var getTimeperiod = await _unitOfWork.Employee.GetFeedbackCountForTimePeriod(checkEmployeeExists.Id);
                var timeperiod = getTimeperiod.FeedbackCount;
                var departmentName = departmentResponse.Name;
                var employeeName = $"{checkEmployeeExists.FirstName} {checkEmployeeExists.LastName}";
                var employeeDesignation = checkEmployeeExists.Designation;
                var employeeDepartment = departmentName;
                var employeeDOJ = checkEmployeeExists.JoiningDate;
                var feedbackForm = _mapper.Map<FeedbackForm>(feedbackFormDto);
                feedbackForm.Name = employeeName;
                feedbackForm.Designation = employeeDesignation;
                feedbackForm.Department = employeeDepartment;
                feedbackForm.DOJ = employeeDOJ;
                feedbackForm.TimePeriod = timeperiod.ToString();
                return (feedbackForm, null);
            }

            return (null, SharedResources.FailedToAddOrUpdateFeedbackForNonStipendOrTrainee);
        }


        public async Task<ResponseModel<List<UserBadgeDto>>> GetAllAwardsAsync()
        {
            var response = new ResponseModel<List<UserBadgeDto>>();
            var userBadges = await _unitOfWork.Employee.GetAllUserBadgesAsync();

            if (userBadges == null || !userBadges.Any())
            {
                response.Message.Add(SharedResources.AwardsNotFound);
                return response;
            }
            else
            {
                response.Model = _mapper.Map<List<UserBadgeDto>>(userBadges);
                return response;
            }

        }

        public async Task<ResponseModel<EmployeeListResponse>> GetListOfNewRequestAsync(string searchText, int departmentId)
        {
            var response = new ResponseModel<EmployeeListResponse>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            var (employees, totalCount) = await _unitOfWork.Employee.GetListOfNewRequestAsync(claims.DepartmentId, searchText);

            if (employees == null || !employees.Any())
            {
                response.Message.Add(SharedResources.NewRequestsNotFound);
                return response;
            }

            response.Model = new EmployeeListResponse
            {
                TotalCount = totalCount,
                Results = _mapper.Map<List<EmployeeViewModel>>(employees)
            };
            return response;
        }

        public async Task<ResponseModel<MonthlyTraineeFeedbackViewModel>> GetMonthlyTraineeFeedbackById(int feedbackId, string employeeId)
        {
            var result = new ResponseModel<MonthlyTraineeFeedbackViewModel>();
            try
            {
                var response = await _unitOfWork.Employee.GetMonthlyTraineeFeedbackById(feedbackId, employeeId);
                if (response == null)
                    result.Message.Add(SharedResources.TraineeFeedbackFormNotFound);
                else
                    result.Model = _mapper.Map<MonthlyTraineeFeedbackViewModel>(response);
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }

        public async Task<ResponseModel<string>> RemoveEmployeeByManagerFromManagerTeamList(string employeeId)
        {
            var result = new ResponseModel<string>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

            var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(employeeId, claims.DepartmentId, string.Empty);

            if (checkEmployeeExists != null)
            {
                if (claims.Role == "Project Manager")
                {
                    if (checkEmployeeExists.TeamAdminId != claims.UserId)
                    {
                        result.Model = "Error";
                        result.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                        return result;
                    }
                }
                var response = await _unitOfWork.Employee.RemoveEmployeeByManagerFromManagerTeamListAsync(employeeId);
                if (response != null)
                {
                    await _unitOfWork.Employee.RemoveEmployeeFromTeamAsync(employeeId);
                    result.Model = response;
                    result.Message.Add(SharedResources.EmployeeRemovedFromList);
                }
                else
                {
                    result.Model = "Error";
                    result.Message.Add(SharedResources.SomethingWentWrong);
                }
            }
            else
            {
                result.Model = "Not Found";
                result.Message.Add(SharedResources.EmployeeNotFound);
            }

            return result;
        }
        public async Task<ResponseModel<List<SalesPersonViewModel>>> GetSalesPersonsList([FromQuery] int departmentId)
        {
            var result = new ResponseModel<List<SalesPersonViewModel>>();
            try
            {
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
                var salesPersonsList = await _unitOfWork.Employee.GetSalesPersonsList(departmentId);
                if (salesPersonsList == null)
                    result.Message.Add(SharedResources.SalesProsonsNotFound);
                else
                    result.Model = salesPersonsList;
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }
        public async Task<ResponseModel<EmployeeDto>> GetEmployeeDetail(string employeeId)
        {
            var result = new ResponseModel<EmployeeDto>();

            var getEmployeeDetails = await _unitOfWork.Employee.GetEmployeeById(employeeId, 0, string.Empty);
            if (getEmployeeDetails == null)
                result.Message.Add(SharedResources.UserNotFound);
            result.Model = _mapper.Map<EmployeeDto>(getEmployeeDetails);
            return result;
        }
        public async Task<ResponseModel<List<ManagerTeamLeadBAAndBDListDepartmentsViewModel>>> GetProjectManagerOrTeamLeadOrBDMListByDepartments(List<int> departmentIds)
        {
            var response = new ResponseModel<List<ManagerTeamLeadBAAndBDListDepartmentsViewModel>>();
            try
            {
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
                var employeeList = await _unitOfWork.Employee.GetProjectManagerOrTeamLeadOrBDMListByDepartments(departmentIds);

                if (employeeList == null)
                {
                    response.Message.Add(SharedResources.NoDataFound);
                }
                else
                {
                    // Filter out the logged-in user
                    employeeList.Manager = employeeList.Manager.Where(a => a.Id != claims.LoggedInUserId).ToList();
                    employeeList.TeamLead = employeeList.TeamLead.Where(a => a.Id != claims.LoggedInUserId).ToList();
                    employeeList.BDM = employeeList.BDM.Where(a => a.Id != claims.LoggedInUserId).ToList();

                    // Group by Department
                    var groupedData = employeeList.Manager
                        .Concat(employeeList.TeamLead) // Combine Manager and TeamLead lists
                        .Concat(employeeList.BDM)      // Add BDM list
                        .GroupBy(e => e.Department)    // Assuming each item has a 'Department' property
                        .Select(group => new ManagerTeamLeadBAAndBDListDepartmentsViewModel
                        {
                            Department = group.Key,
                            Manager = employeeList.Manager.Where(m => m.Department == group.Key).ToList(),
                            TeamLead = employeeList.TeamLead.Where(t => t.Department == group.Key).ToList(),
                            BDM = employeeList.BDM.Where(b => b.Department == group.Key).ToList()
                        })
                        .ToList();

                    response.Model = groupedData;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.ErrorWhileGettingMembers);
            }

            return response;
        }

    }
}
