using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
namespace TimeTaskTracking.Core.Services;
public class TeamStatusService : ITeamStatusService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IHttpContextAccessor _contextAccessor;
    public TeamStatusService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IHttpContextAccessor contextAccessor)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _contextAccessor = contextAccessor;
    }
    public async Task<ResponseModel<List<TeamStatusViewModel>>> GetTeamStatusByTeamLead(DateTime filterByDate, string? teamLeadId)
    {
        var response = new ResponseModel<List<TeamStatusViewModel>>();
        if (filterByDate.Date > DateTime.Now.Date)
            response.Message.Add(SharedResources.DateMustNotBeFutureDate);
        else
        {
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            if (claims.Role == "Team Lead")
                teamLeadId = claims.UserId;
            var result = await _unitOfWork.TeamStatus.GetTeamStatusByTeamLead(teamLeadId, filterByDate);
            if (result != null)
                response.Model = result;
            else
                response.Message.Add(SharedResources.EmployeeStatusNotFound);
        }
        return response;
    }

    public async Task<ResponseModel<List<object>>> UpdateAttendanceStatusAsync(AttendanceDetails attendanceDetails)
    {
        var response = new ResponseModel<List<object>>();

        var currentDate = DateTime.Now.Date;
        if (attendanceDetails.FilterByDate > currentDate)
        {
            response.Message.Add(SharedResources.InvalidDate);
            response.Model = new List<object> { 1 };
            return response;
        }
        try
        {
            if (!Enum.IsDefined(typeof(AttendanceStatus), attendanceDetails.AttendanceStatus))
            {
                response.Message = new List<string> { SharedResources.InvalidAttendanceStatus };
                return response;
            }
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var departmentId = claims.DepartmentId;
            var employeeIds = new List<string?>();
            var teamLeadId = string.Empty;
            var teamAdminId = string.Empty;
            var submittedBy = claims.LoggedInUserId;

            var loggedInUserDetails = await _unitOfWork.Employee.GetEmployeeById(claims.LoggedInUserId, claims.DepartmentId, string.Empty);
            // Fetch employees under the specified team lead
            if (claims.Role == "Team Lead")
            {
                teamLeadId = claims.UserId;
                var employeeDetails = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.UserId);
                var filteredEmployeeDetails = employeeDetails.Where(employee => attendanceDetails.EmpId.Contains(employee.Id)).ToList();
                employeeIds = filteredEmployeeDetails.Select(employee => employee.Id).ToList();
                if (attendanceDetails.EmpId.Contains(claims.UserId))
                {
                    employeeIds.Add(claims.UserId);
                }
            }
            if (claims.Role == "Project Manager")
            {
                teamAdminId = claims.UserId;
                var employeeDetails = await _unitOfWork.Employee.GetEmployeeListByManager(claims.UserId, claims.DepartmentId);
                if (employeeDetails != null)
                {
                    var filteredEmployeeDetails = employeeDetails.Where(employee => attendanceDetails.EmpId.Contains(employee.Id)).ToList();
                    employeeIds = filteredEmployeeDetails.Select(employee => employee.Id).ToList();
                }

            }

            if (claims.Role == "HOD")
            {
                var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                var employeeDetails = await _unitOfWork.Employee.GetEmployeeListByDepartmentId(claims.DepartmentId);
                if (employeeDetails != null)
                {
                    var filteredEmployeeDetails = employeeDetails.Where(employee => attendanceDetails.EmpId.Contains(employee.Id)).ToList();
                    employeeIds = filteredEmployeeDetails.Select(employee => employee.Id).ToList();
                }
            }

            if (claims.Role == "Admin")
            {
                employeeIds = attendanceDetails.EmpId;
            }
            var updatedEmployeeIds = new List<string>();

            if (employeeIds?.Any() == false)
            {
                response.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                response.Model = new List<object> { 1 };
                return response;
            }

            foreach (var id in employeeIds)
            {
                var getAttendanceForTheEmployee = await _unitOfWork.TeamStatus.GetAttendanceStatusByEmployeeId(id, attendanceDetails.FilterByDate);
                string statusString = GetAttendanceStatusString(attendanceDetails.AttendanceStatus);

                if (claims.Role == "Project Manager")
                {
                    var teamLeadDetails = await _unitOfWork.Employee.GetTeamLeadIdByEmployeeId(id);
                    teamLeadId = teamLeadDetails.TeamLeadId;
                }

                if (claims.Role == "Team Lead")
                {
                    var employeeDetails = await _unitOfWork.Employee.GetEmployeeById(id, claims.DepartmentId, string.Empty);
                    teamAdminId = employeeDetails.TeamAdminId;
                }

                if (claims.Role == "Admin" || claims.Role == "HOD")
                {
                    var teamLeadDetails = await _unitOfWork.Employee.GetTeamLeadIdByEmployeeId(id);
                    if (teamLeadDetails != null)
                        teamLeadId = teamLeadDetails.TeamLeadId;
                    else
                        teamLeadId = id; // if user is team lead or does not have any team lead.
                    var employeeDetails = await _unitOfWork.Employee.GetEmployeeById(id, claims.DepartmentId, string.Empty);
                    teamAdminId = employeeDetails.TeamAdminId;
                }
                try
                {
                    if (getAttendanceForTheEmployee)
                    {
                        if (attendanceDetails.AttendanceStatus == (int)AttendanceStatus.P)
                        {
                            var existingLeave = await _unitOfWork.EmployeeStatus.CheckForExistingLeave(id, attendanceDetails.FilterByDate);
                            if (existingLeave != null)
                            {
                                await _unitOfWork.TeamStatus.DeleteEmployeeLeaves(id, attendanceDetails.FilterByDate, statusString);
                            }
                        }
                        else
                        {
                            var checkForEmployeeLeaveExists = await _unitOfWork.EmployeeStatus.CheckForExistingLeave(id, attendanceDetails.FilterByDate);
                            if (checkForEmployeeLeaveExists != null)
                            {
                                await _unitOfWork.EmployeeStatus.UpdateEmployeeLeaveForTheEmployee(id, attendanceDetails.FilterByDate, statusString, checkForEmployeeLeaveExists.Id, submittedBy);
                            }
                            else
                            {
                                await _unitOfWork.EmployeeStatus.AddEmployeeLeave(id, false, attendanceDetails.FilterByDate, statusString, teamLeadId, teamAdminId, submittedBy);
                            }
                            await SendAttendenceUpdatedStatusMail(id,claims.DepartmentId, loggedInUserDetails.Email, loggedInUserDetails.FirstName+" "+ loggedInUserDetails.LastName, SharedResources.GetAttendanceStatusDescription(statusString));
                        }
                        updatedEmployeeIds.Add(id);
                    }
                    else
                    {
                        var checkForEmployeeLeaveExists = await _unitOfWork.EmployeeStatus.CheckForExistingLeave(id, attendanceDetails.FilterByDate);

                        if (attendanceDetails.AttendanceStatus != (int)AttendanceStatus.P)
                        {
                            if (checkForEmployeeLeaveExists != null)
                            {
                                await _unitOfWork.EmployeeStatus.UpdateEmployeeLeaveForTheEmployee(id, attendanceDetails.FilterByDate, statusString, checkForEmployeeLeaveExists.Id, submittedBy);
                            }
                            else
                            {
                                await _unitOfWork.EmployeeStatus.AddEmployeeLeave(id, false, attendanceDetails.FilterByDate, statusString,teamLeadId,teamAdminId,submittedBy);
                            }
                            await SendAttendenceUpdatedStatusMail(id,claims.DepartmentId, loggedInUserDetails.Email, loggedInUserDetails.FirstName + " " + loggedInUserDetails.LastName, SharedResources.GetAttendanceStatusDescription(statusString));;
                        }

                        else if (attendanceDetails.AttendanceStatus == (int)AttendanceStatus.P)
                        {
                            if (checkForEmployeeLeaveExists != null)
                            {
                                await _unitOfWork.TeamStatus.DeleteEmployeeLeaves(id, attendanceDetails.FilterByDate, statusString);
                            }  
                        }
                        else
                        {
                            response.Message.Add(SharedResources.EmployeeStatusNotFoundForTheEmployee);
                            response.Model = new List<Object> { 1 };
                            return response;
                        }
                    }
                }
                catch (Exception)
                {
                    response.Message = new List<string> { SharedResources.ErrorWhileUpdatingAttendanceStatus };
                    return response;
                }
                var updatedEmployeeObjects = updatedEmployeeIds.Select(id => new { Id = id }).ToList();


                response.Model = updatedEmployeeObjects.Cast<object>().ToList();
            }
        }
        catch (Exception)
        {
            response.Message = new List<string> { SharedResources.InternalServerError };
        }
        response.Message.Add(SharedResources.AttendanceStatus);
        return response;
    }
    public static string GetAttendanceStatusString(int? statusId)
    {
        if (Enum.IsDefined(typeof(AttendanceStatus), statusId))
        {
            return ((AttendanceStatus)statusId).ToString();
        }
        throw new ArgumentException("Invalid AttendanceStatus Id");
    }

    public async Task<ResponseModel<bool>> SendWarningMailAsync(TeamStatusDetails<List<string>> teamStatus)
    {
        var response = new ResponseModel<bool>();
        try
        {
            foreach (var employeeId in teamStatus.EmployeeId)
            {
                var model = new TeamStatusDetails<string>()
                {
                    EmployeeId = employeeId,
                    TeamLeadId = teamStatus.TeamLeadId
                };

                var result = await _unitOfWork.TeamStatus.GetEmployeeAndTeamLeadDetails(model);
                if (result != null)
                {
                    var content = $@"
            <h1 style='background-color: blue; color: white; padding: 10px;'>Warning Mail</h1>
            <p>This is a warning mail to you. You haven't added status for day {teamStatus.StatusDate}. Please add this now.</p>
            <p>Thank You,<br>
            {result.TeamLeadUserName}<br>
            {DateTime.Now}</p>";
                    _emailService.SendMail(result.EmployeeEmail,
                    result.EmployeeUserName,
                             result.TeamLeadEmail,
                             result.TeamLeadUserName, content, "Warning Mail");

                }
                response.Model = true;
                return response;
            }
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<bool>> SendWarningMailToEmployees(WarningEmailViewModel model)
    {
        var response = new ResponseModel<bool>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
       
        var getALLTeamMember = new List<EmployeeDetailsViewModel>();
        var validEmployeeEmails = new List<string>();
        var allowedRoles = new[] { "HOD", "Project Manager" };
        
        if (claims.Role == "Team Lead")
        {
            model.DepartmentId = claims.DepartmentId;
            getALLTeamMember = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.UserId);
        }
        if (allowedRoles.Contains(claims.Role))
        {
            model.DepartmentId = claims.DepartmentId;
            getALLTeamMember = await _unitOfWork.Employee.GetEmployeeListByDepartmentId(model.DepartmentId);
        }
        else
        {
            getALLTeamMember = await _unitOfWork.Employee.GetEmployeeListByDepartmentId(model.DepartmentId);
        }
        foreach (var employeeId in model.EmployeeId)
        {
            if (getALLTeamMember.Any(e => e.Id == employeeId))
            {
                var details = await _unitOfWork.Employee.GetEmployeeById(employeeId, claims.DepartmentId, string.Empty);
                if (!string.IsNullOrEmpty(details.Email))
                {
                    validEmployeeEmails.Add(details.Email);
                }
            }
        }

        if (validEmployeeEmails.Any())
        {
            bool emailsSent = await _emailService.SendWarningEmailsAsync(validEmployeeEmails, model.Description, "Warning Email");
            if (emailsSent)
            {
                response.Message.Add(SharedResources.WarningEmailsSentSuccessfully);
                response.Model = true;
            }
            else
            {
                response.Message.Add(SharedResources.FailedToSendWarningMails);
                response.Model = false;
            }
        }
        else
        {
            response.Message.Add(SharedResources.TeamMembersNotFound);
        }
        return response;
    }

    private async Task SendAttendenceUpdatedStatusMail(string employeeId,int departmentId, string teamLeadEmail, string TeamLeadName, string attendenceStatus)
    {
        var employeeDetail = await _unitOfWork.Employee.GetEmployeeById(employeeId, departmentId, string.Empty);

        var emailTeamplate = await SharedResources.GetAttendenceUpdatedEmailTemplate();
        emailTeamplate = emailTeamplate.Replace("{AttendanceStatus}", attendenceStatus)
                                    .Replace("{StatusDate}", attendenceStatus)
                                    .Replace("{Name}", TeamLeadName)
                                    .Replace("{Date}", DateTime.Now.ToString());
        _emailService.SendMail(employeeDetail.Email, employeeDetail.FirstName+" "+ employeeDetail.LastName, teamLeadEmail, TeamLeadName,
                    emailTeamplate, "Attendence Status Updated");
    }
}
