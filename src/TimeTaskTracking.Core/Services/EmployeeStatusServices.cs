using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services
{
    public class EmployeeStatusServices : IEmployeeStatusServices
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public EmployeeStatusServices(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }
        public async Task<ResponseModel<bool>> DeleteEmployeeStatusByIDAsync(EmployeeStatusFilterViewModel getAndDeleteEmployeeStatusViewModel)
        {
            var result = new ResponseModel<bool>();
            try
            {
                var checkDateIsValid = DateValidation(getAndDeleteEmployeeStatusViewModel.ToDate);
                if (!string.IsNullOrEmpty(checkDateIsValid))
                    result.Message.Add(checkDateIsValid);
                else
                {
                    var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
                    getAndDeleteEmployeeStatusViewModel.UserProfileId = claims.UserId;
                    var response = await _unitOfWork.EmployeeStatus.DeleteEmployeeStatus(getAndDeleteEmployeeStatusViewModel);
                    if (response == true)
                    {
                        result.Message.Add(SharedResources.DeleteMessage);
                        result.Model = true;
                    }
                    else
                    {
                        result.Message.Add(SharedResources.ErrorWhileDeleteEmployeeStatus);
                        result.Model = false;
                    };
                }
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }

        public async Task<ResponseModel<bool>> DeleteStatusAsync(int id)
        {
            var result = new ResponseModel<bool>();
            try
            {
                var getEmployeeDetailsByStatusId = await _unitOfWork.EmployeeStatus.GetEmployeeStatusByIdAsync(id);
                if (getEmployeeDetailsByStatusId != null)
                {
                    var response = await _unitOfWork.EmployeeStatus.DeleteEmployeeStatusByID(id);
                    if (response)
                    {
                        await DeleteEmployeeHalfAbsentLeaves(getEmployeeDetailsByStatusId.Date, getEmployeeDetailsByStatusId.ApplicationUsersId);
                        result.Message.Add(SharedResources.DeleteMessage);
                    }
                    else
                        result.Message.Add(SharedResources.ErrorWhileDeleteEmployeeStatus);
                    result.Model = response;
                }
                else
                {
                    result.Message.Add(SharedResources.ErrorWhileDeleteEmployeeStatus);
                }
                
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }

        public async Task<ResponseModel<PaginationResponseViewModel<List<EmployeeStatusResponseDto>>>> GetEmployeeStatusListAsync(EmployeeStatusPaginationViewModel employeeStatusPaginationViewModel)
        {
            var response = new ResponseModel<PaginationResponseViewModel<List<EmployeeStatusResponseDto>>>();
            try
            {
                if (_contextAccessor.HttpContext != null)
                {
                    var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
                    employeeStatusPaginationViewModel.UserProfileId = claims.UserId;
                }

                var employeeStatusList = await _unitOfWork.EmployeeStatus.GetEmployeeStatusList(employeeStatusPaginationViewModel);
                if (employeeStatusList?.results?.Any() == true)
                {
                    var mappedEmployees = _mapper.Map<List<EmployeeStatusResponseDto>>(employeeStatusList.results);

                    var paginationResponse = new PaginationResponseViewModel<List<EmployeeStatusResponseDto>>();
                    paginationResponse.results = mappedEmployees;
                    paginationResponse.TotalCount = employeeStatusList.TotalCount;

                    response.Model = paginationResponse;
                }
                else
                {
                    response.Message.Add(SharedResources.EmployeeStatusNotFound);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }

            return response;
        }
        public async Task<ResponseModel<EstimatedHoursDto>> GetProjectModuleByIdAsync(string moduleId)
        {
            var result = new ResponseModel<EstimatedHoursDto>();
            try
            {
                var response = await _unitOfWork.EmployeeStatus.GetProjectModuleByIdAsync(moduleId, 0);
                if (response == null)
                    result.Message.Add(SharedResources.ProjectModuleNotFound);
                else
                    result.Model = _mapper.Map<EstimatedHoursDto>(response);
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }

        public async Task<ResponseModel<EmployeeStatusDto>> AddNewEmployeeStatusAsync(Dtos.EmployeeStatusDto employeeStatusList)
        {
            var result = new ResponseModel<EmployeeStatusDto>();
            try
            {          
                var validHours = await ValidHours(employeeStatusList);
                if (!string.IsNullOrEmpty(validHours))
                {
                    result.Message.Add(validHours);
                    return result;
                }
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
                employeeStatusList.ApplicationUsersId = claims.UserId;

                var teamLeadId = string.Empty;
                var teamAdminId = string.Empty;

                if (claims.Role == "Team Lead")
                {
                    teamLeadId = claims.UserId;

                }
                else if (claims.Role == "Employee" || claims.Role == "BDM")
                {
                    // If the user is an Employee or BDM, retrieve the TeamLeadId from a repository method
                    var teamLeadDetails = await _unitOfWork.Employee.GetTeamLeadIdByEmployeeId(claims.UserId);
                    teamLeadId = teamLeadDetails?.TeamLeadId;
                }

                var getTeamAdminId = await _unitOfWork.Employee.GetEmployeeById(claims.UserId, claims.DepartmentId, string.Empty);
                if (!string.IsNullOrEmpty(getTeamAdminId.Id))
                {
                    teamAdminId = getTeamAdminId.TeamAdminId;
                }
                else
                {
                    result.Message.Add(SharedResources.UnableToFindTheManager);
                    return result;
                }

                var getProjectDetails = await _unitOfWork.Project.GetProject(employeeStatusList.ProjectID, claims.DepartmentId);
                if (getProjectDetails.IsBilling == 3)
                {
                    employeeStatusList.ProfileId = 1;
                }
                
                if (getProjectDetails.IsBilling != 3 && employeeStatusList.ProfileId == 1)
                {
                    result.Message.Add(SharedResources.PleaseSelectAProfile);
                    return result;
                }
                
                var responseList = await _unitOfWork.EmployeeStatus.AddNewEmployeeStatus(_mapper.Map<EmployeeStatus>(employeeStatusList), teamLeadId, teamAdminId);
                var responseDtoList = _mapper.Map<Dtos.EmployeeStatusDto>(responseList);

                if (responseDtoList != null)
                {
                    var ids = new List<int> { responseDtoList.Id };
                    result.Message.Add(SharedResources.StatusAddedSuccessfully);
                    result.Model = responseDtoList;

                    if (responseDtoList != null)
                        await DeleteEmployeeHalfAbsentLeaves(responseDtoList.Date, claims.UserId);
                }
                else
                    result.Message.Add(SharedResources.ErrorWhileAddStatus);
                return result;
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
                return result;
            }        
        }

        public async Task<ResponseModel<int>> UpdateEmployeeStatusAsync(Dtos.EmployeeStatusDto editStatus)
        {
            var result = new ResponseModel<int>();
            try
            {
                if (editStatus.Id == 0)
                {
                    result.Message.Add(SharedResources.StatusNotFound);
                    return result;
                }
                var validHours = await ValidHours(editStatus);
                if (!string.IsNullOrEmpty(validHours))
                {
                    result.Message.Add(validHours);
                    return result;
                }
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
                editStatus.ApplicationUsersId = claims.UserId;
                var response = await _unitOfWork.EmployeeStatus.UpdateEmployeeStatus(_mapper.Map<Models.Entities.EmployeeStatus>(editStatus));
                if (response != null)
                {
                    result.Message.Add(SharedResources.StatusUpdatedSuccessfully);
                    result.Model = response.Id;
                    await DeleteEmployeeHalfAbsentLeaves(editStatus.Date, editStatus.ApplicationUsersId);
                }
                else
                    result.Message.Add(SharedResources.ErrorWhileEditStatus);
                return result;
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }

        public async Task<ResponseModel<int>> AddEmployeeLeaveAsync(string userId, bool? isPresent, DateTime date, string attendanceStatus)
        {
            var result = new ResponseModel<int>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var teamLeadId = string.Empty;
            var teamAdminId = string.Empty;
            var submittedBy = claims.LoggedInUserId;

            if (claims.Role == "Team Lead")
            {
                teamLeadId = claims.UserId;
;
            }
            else if (claims.Role == "Employee" || claims.Role == "BDM")
            {
                // If the user is an Employee or BDM, retrieve the TeamLeadId from a repository method
                var teamLeadDetails = await _unitOfWork.Employee.GetTeamLeadIdByEmployeeId(claims.UserId);
                teamLeadId = teamLeadDetails?.TeamLeadId;

            }
            else
            {
                result.Message.Add(SharedResources.TeamLeadIDNotFound);
                return result;
            }
            var getTeamAdminId = await _unitOfWork.Employee.GetEmployeeById(claims.UserId, claims.DepartmentId, string.Empty);
            if (!string.IsNullOrEmpty(getTeamAdminId.Id))
            {
                teamAdminId = getTeamAdminId.TeamAdminId;
            }
            else
            {
                result.Message.Add(SharedResources.UnableToFindTheManager);
                return result;
            }

            var leaveResponse = await _unitOfWork.EmployeeStatus.AddEmployeeLeave(userId, isPresent, date, attendanceStatus,teamLeadId,teamAdminId, submittedBy);
            if (leaveResponse != null)
            {
                result.Message.Add(SharedResources.LeaveStatusMessage);
                result.Model = leaveResponse.Id;
            }
            else
                result.Message.Add(SharedResources.ErrorWhileEditStatus);
            return result;
        }
        public async Task<ResponseModel<int>> CheckIfUserAlreadyAppliedLeave(Dtos.EmployeeStatusDto status)
        {
            var result = new ResponseModel<int>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var checkUserCanEditStatus = await _unitOfWork.Employee.GetEmployeesCanEditStatus(claims.UserId, claims.DepartmentId);

            if (checkUserCanEditStatus == false)
            {
                var checkDateIsValid = DateValidation(status.Date);
                if (!string.IsNullOrEmpty(checkDateIsValid))
                {
                    result.Model = 2;
                    result.Message.Add(checkDateIsValid);
                    return result;
                }
            }
            claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            status.ApplicationUsersId = claims.UserId;
            var isLeaveApplied = await _unitOfWork.EmployeeStatus.CheckIfUserAlreadyAppliedLeave(status.ApplicationUsersId, status.Date);
            var allowedStatuses = new List<string> { "L", "AB", "HL" };
            var isLeaveMarked = isLeaveApplied?.Where(a => allowedStatuses.Contains(a.AttendanceStatus)).ToList();

            var employeeStatuses = await GetEmployeeStatusListAsync(new EmployeeStatusPaginationViewModel
            {
                FromDate = status.Date,
                ToDate = status.Date,
                UserProfileId = status.ApplicationUsersId
            });

            decimal totalCurrentHours = status.UpworkHours + status.OfflineHours + status.FixedHours;

            decimal totalHours = 0;

            if (employeeStatuses != null && employeeStatuses.Model != null && employeeStatuses.Model.results != null)
            {
                totalHours = employeeStatuses.Model.results.Where(a => a.Id != status.Id)
                                        .Sum(a => a.UpworkHours + a.FixedHours + a.OfflineHours);
                totalHours += totalCurrentHours;
            }
            else
            {
                totalHours = totalCurrentHours;
            }

            if (status.MarkAsLeave == true && (isLeaveMarked == null || !isLeaveMarked.Any()))
            {
                await _unitOfWork.TeamStatus.DeleteEmployeeLeaves(status.ApplicationUsersId, status.Date, string.Empty);
                var leaveResponse = await AddEmployeeLeaveAsync(status.ApplicationUsersId, false, status.Date, "L");
                if (leaveResponse.Model != 0)
                    result.Model = 1;
                result.Message = leaveResponse.Message;
                await DeleteEmployeeStatusByIDAsync(new EmployeeStatusFilterViewModel { UserProfileId = status.ApplicationUsersId, ToDate = status.Date });
                return result;
            }
            else if (isLeaveApplied != null && isLeaveApplied?.Where(a => a.AttendanceStatus == "L")?.Any() == true)
            {
                result.Message.Add(SharedResources.LeaveAlreadyAppliedMessage);
                result.Model = 2;
                return result;
            }
            else if (isLeaveApplied?.Where(a => a.AttendanceStatus == "HL")?.Any() == true)
            {
                if (totalHours > 4)
                {
                    result.Message.Add(SharedResources.HalfDayLeaveAlreadyAppliedMessage);
                    result.Model = 2;
                    return result;
                }
            }
            //else if (isLeaveApplied?.Where(a => a.AttendanceStatus == "HA")?.Any() == true)
            //{
            //    if (totalHours > 4)
            //    {
            //        result.Message.Add(SharedResources.HalfAbsetLeaveAlreadyAppliedMessage);
            //        return result;
            //    }
            //}
            else if (isLeaveApplied?.Where(a => a.AttendanceStatus == "AB")?.Any() == true)
            {
                result.Message.Add(SharedResources.AbsentAlreadyAppliedMessage);
                result.Model = 2;
                return result;
            }
            if (totalHours > 24)
            {
                result.Message.Add(SharedResources.EmployeeStatusHoursLimit);
                result.Model = 2;
            }

            return result;
        }

        public async Task<ResponseModel<EmployeeStatusResponseDto>> GetEmployeeStatusById(int id)
        {
            var result = new ResponseModel<EmployeeStatusResponseDto>();
            try
            {
                var response = await _unitOfWork.EmployeeStatus.GetEmployeeStatusById(id);
                if (response == null)
                    result.Message.Add(SharedResources.EmployeeStatusNotFound);
                else
                    result.Model = _mapper.Map<EmployeeStatusResponseDto>(response);
            }
            catch (Exception ex)
            {
                result.Message.Add(SharedResources.InternalServerError);
            }
            return result;
        }
        private string DateValidation(DateTime date)
        {
            if ((DateTime.Now.Date - date.Date).TotalDays > 7)
                return SharedResources.CheckStatusDate;
            if (date.Date > DateTime.Today.Date)
                return SharedResources.DateStatusMessage;
            return string.Empty;
        }
        private async Task<string> ValidHours(Dtos.EmployeeStatusDto status)
        {
            if (status.UpworkHours == 0 && status.FixedHours == 0 && status.OfflineHours == 0)
                return SharedResources.HoursDetails;

            var response = await _unitOfWork.EmployeeStatus.GetProjectModuleByIdAsync(Convert.ToString(status.ModuleId), status.Id);

            var paymentStatus = response.PaymentStatus;
            if (paymentStatus == "NonBillable" && (status.UpworkHours > 0 || status.FixedHours > 0))
            {
                return SharedResources.DoNotAddBillableHours;
            }

            if (response == null)
                return SharedResources.ErrorWhileFetchModuleIdData;

            var overAllHours = response.EstimatedHours - response.BilledHours;
            var totalBilledHours = status.UpworkHours + status.FixedHours;

            if (overAllHours < totalBilledHours)
            {
                int overAllHoursInt = (int)overAllHours;
                int overAllMinutesInt = (int)((overAllHours - overAllHoursInt) * 60);
                return $"Allowed hours: {overAllHoursInt}:{overAllMinutesInt:D2}";
            }

            //var totalHours = status.UpworkHours + status.OfflineHours + status.FixedHours;
            status.UpworkHours = ConvertHoursToDecimal(status.UpworkHours);
            status.FixedHours = ConvertHoursToDecimal(status.FixedHours);
            status.OfflineHours = ConvertHoursToDecimal(status.OfflineHours);

            return string.Empty;
        }
        private decimal ConvertHoursToDecimal(decimal hours)
        {
            if (hours != 0)
            {
                int integerPart = (int)hours;
                decimal fractionalPart = hours - integerPart;
                hours = integerPart + (fractionalPart * 100 / 60);
            }
            return hours;
        }
        private async Task DeleteEmployeeHalfAbsentLeaves(DateTime date, string userId)
        {
            var isLeaveApplied = await _unitOfWork.EmployeeStatus.CheckIfUserAlreadyAppliedLeave(userId, date);
            var employeeStatuses = await GetEmployeeStatusListAsync(new EmployeeStatusPaginationViewModel
            {
                FromDate = date,
                ToDate = date,
                UserProfileId = userId
            });

            decimal totalHours = 0;

            if (employeeStatuses != null && employeeStatuses.Model != null && employeeStatuses.Model.results != null)
            {
                totalHours = employeeStatuses.Model.results.Sum(a => a.UpworkHours + a.FixedHours + a.OfflineHours);
            }

            if (employeeStatuses != null && employeeStatuses.Model != null && employeeStatuses.Model.results != null
                && employeeStatuses.Model.results.Any() && totalHours >= 8)
            {
                await _unitOfWork.TeamStatus.DeleteEmployeeLeaves(userId, date, "HA");
            }

            if (totalHours > 0 && totalHours < 4)
            {
                await _unitOfWork.TeamStatus.DeleteEmployeeLeaves(userId, date, "Ab");
                await AddEmployeeLeaveAsync(userId, true, date, "Ab");
            }

            if (totalHours < 8 && totalHours >= 4 && (isLeaveApplied == null || (!isLeaveApplied.Any(a => a.AttendanceStatus == "HA") &&
                !isLeaveApplied.Any(a => a.AttendanceStatus == "HL"))))
            {
                if (isLeaveApplied?.Any(a => a.AttendanceStatus == "Ab") == true)
                {
                     await _unitOfWork.TeamStatus.DeleteEmployeeLeaves(userId, date, "Ab"); 
                }
                await AddEmployeeLeaveAsync(userId, true, date, "HA");
            }

            if (totalHours == 0 && isLeaveApplied?.Any() == true)
            {
                var attendanceStatus = isLeaveApplied.FirstOrDefault()?.AttendanceStatus;
                await _unitOfWork.TeamStatus.DeleteEmployeeLeaves(userId, date, attendanceStatus);
            }
        }

        public async Task<ResponseModel<List<EmployeeStatusViewModel>>> GetEmployeeStatusListByManager(int Month, int Year, string EmployeeId, int departmentId)
        {
            var response = new ResponseModel<List<EmployeeStatusViewModel>>();
            try
            {

                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
                var employeeStatusList = await _unitOfWork.EmployeeStatus.GetEmployeeStatusListByManager(Month, Year, claims.DepartmentId, EmployeeId);
                if (employeeStatusList?.Any() == true)
                {
                    var mappedEmployees = _mapper.Map<List<EmployeeStatusViewModel>>(employeeStatusList);
                    response.Model = mappedEmployees;
                    return response;
                }
                else
                    response.Message.Add(SharedResources.EmployeeStatusNotFound);
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<List<DropDownResponse<string>>>> GetOpenProjectModulesListByProjectId(int projectId)
        {
            var result = new ResponseModel<List<DropDownResponse<string>>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var projectModules = await _unitOfWork.EmployeeStatus.GetOpenProjectModulesListByProjectIdAsync(projectId, claims.DepartmentId);
            if (projectModules?.Any() != true)
                result.Message.Add(SharedResources.ProjectModuleNotFound);
            else
                result.Model = projectModules;
            return result;
        }
    }
}
