using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Data;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Core.Services;
public class ReportsService : IReportsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    public ReportsService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<ResponseModel<List<MonthlyHoursViewModel>>> GetMonthlyHoursReportsAsync(ReportsViewModel model)
    {
        var response = new ResponseModel<List<MonthlyHoursViewModel>>();
        try
        {
            var result = ValidateReportDates(model);
            if (result == false)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                return response;
            }
            var reportsList = await _unitOfWork.Reports.MonthlyHoursReport(model);
            response.Model = _mapper.Map<List<MonthlyHoursViewModel>>(reportsList);
            if (response.Model.Count == 0)
                response.Message.Add(SharedResources.NoDataFound);
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<PaginationResponseViewModel<List<ProjectBillingReportViewModel>>>> GetProjectReportAsync(ProjectReportRequestModel requestModel)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<ProjectBillingReportViewModel>>>();
        try
        {
            var validateResponse = await ValidateReportParams(requestModel.DepartmentId, requestModel.TeamAdminId);
            if (validateResponse.Message!.Any())
            {
                response.Message.AddRange(validateResponse.Message!);
                return response;
            }
            requestModel.DepartmentId = validateResponse.DepartmentId;
            if (validateResponse.TeamAdminId != null)
                requestModel.TeamAdminId = validateResponse.TeamAdminId;
            var reportsList = await _unitOfWork.Reports.GetProjectReport(requestModel);
            if (reportsList?.results?.Any() == true)
            {
                response.Model = new PaginationResponseViewModel<List<ProjectBillingReportViewModel>>
                {
                    results = _mapper.Map<List<ProjectBillingReportViewModel>>(reportsList.results),
                    TotalCount = reportsList.TotalCount
                };
            }
            else
                response.Message.Add(SharedResources.NoDataFound);
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<List<ReportsResponseViewModel>>> GetReportsListByTeamLeaderAsync(ReportsViewModel model)
    {
        var response = new ResponseModel<List<ReportsResponseViewModel>>();
        try
        {
            var result = ValidateReportDates(model);
            if (!result)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                return response;
            }
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var teamLeadEmployeeList = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.UserId);

            var employeeIds = teamLeadEmployeeList.Select(emp => emp.Id).ToList();
            employeeIds.Add(claims.UserId);

            var reportsList = await _unitOfWork.Reports.ReportsDetailByTeamLeader(model);

            if (reportsList?.Any() == true)
            {
                var filteredReportsList = reportsList.Where(report => employeeIds.Contains(report.ApplicationUsersId)).ToList();

                response.Model = _mapper.Map<List<ReportsResponseViewModel>>(filteredReportsList);
                if (response.Model.Count == 0)
                {
                    response.Message.Add(SharedResources.NoDataFound);
                }
            }
            else
                response.Message.Add(SharedResources.NoDataFound);
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<PaginationResponseViewModel<List<EmployeeReportViewModel>>>> GetEmployeeReportAsync(EmployeeReportRequestModel employeeReportViewModel)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<EmployeeReportViewModel>>>();
        try
        {
            // Validate the input parameters
            var validateResponse = await ValidateReportParams(employeeReportViewModel.DepartmentId, employeeReportViewModel.TeamAdminId);
            if (validateResponse.Message!.Any())
            {
                response.Message.AddRange(validateResponse.Message!);
                return response;
            }
            employeeReportViewModel.DepartmentId = validateResponse.DepartmentId;
            if (validateResponse.TeamAdminId != null)
                employeeReportViewModel.TeamAdminId = validateResponse.TeamAdminId;

            // Validate date filter
            var validationResult = await SharedResources.ValidateReportDateFilter(employeeReportViewModel.From, employeeReportViewModel.To);
            if (!validationResult.IsValid)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                return response;
            }
            employeeReportViewModel.From = validationResult.FromDate;
            employeeReportViewModel.To = validationResult.ToDate;
            // Fetch reports list from the repository
            var reportsList = await _unitOfWork.Reports.GetActiveEmployeeReport(employeeReportViewModel);
            if (reportsList?.results?.Any() == true)
            {
                // Map the results and set ProjectIds
                var employeeReports = _mapper.Map<List<EmployeeReportViewModel>>(reportsList.results);
                foreach (var report in employeeReports)
                {
                    report.ProjectIds = await GetProjectIdsByNames(report.ProjectNames);
                }
                response.Model = new PaginationResponseViewModel<List<EmployeeReportViewModel>>
                {
                    results = employeeReports,
                    TotalCount = reportsList.TotalCount
                };
            }
            else
            {
                response.Message.Add(SharedResources.NoDataFound);
            }
        }
        catch (Exception ex)
        {
            // Log the exception if needed
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    private async Task<string> GetProjectIdsByNames(string projectNames)
    {
        if (string.IsNullOrWhiteSpace(projectNames))
        {
            return string.Empty;
        }
        var projectNameList = projectNames.Split(',').Select(p => p.Trim()).ToList();
        var projectIds = new List<int>();
        foreach (var projectName in projectNameList)
        {
            // Fetch project ID by name
            var projectDetails = await _unitOfWork.Project.GetProjectByName(projectName);
            if (projectDetails != null && projectDetails.Id != 0)
            {
                projectIds.Add(projectDetails.Id);
            }
            else
            {
                projectIds.Add(0);
            }
        }
        return string.Join(",", projectIds);
    }

    public async Task<ResponseModel<List<ProjectReportViewModel>>> GetEmployeeProjectReportAsync(EmployeeProjectRequestModel employeeReportViewModel)
    {
        var response = new ResponseModel<List<ProjectReportViewModel>>();
        try
        {
            var validationResult = await SharedResources.ValidateReportDateFilter(employeeReportViewModel.From, employeeReportViewModel.To);
            if (!validationResult.IsValid)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                return response;
            }
            employeeReportViewModel.From = validationResult.FromDate;
            employeeReportViewModel.To = validationResult.ToDate;

            var reportsList = await _unitOfWork.Reports.GetEmployeeProjectReport(employeeReportViewModel);
            response.Model = _mapper.Map<List<ProjectReportViewModel>>(reportsList);
            if (response.Model.Count == 0)
                response.Message.Add(SharedResources.NoDataFound);
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }
    public async Task<ResponseModel<PaginationResponseViewModel<List<EmployeeAttendanceReportViewModel>>>> GetEmployeesAttendanceReport(EmployeeAttendanceRequestViewModel employeeAttendanceRequestViewModel)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<EmployeeAttendanceReportViewModel>>>();
        try
        {
            var validateResponse = await ValidateReportParams(employeeAttendanceRequestViewModel.departmentId, employeeAttendanceRequestViewModel.TeamAdminId);
            if (validateResponse.Message!.Any())
            {
                response.Message.AddRange(validateResponse.Message!);
                return response;
            }
            employeeAttendanceRequestViewModel.departmentId = validateResponse.DepartmentId;
            if (validateResponse.TeamAdminId != null)
                employeeAttendanceRequestViewModel.TeamAdminId = validateResponse.TeamAdminId;
            var reportsList = await _unitOfWork.Reports.GetEmployeesAttendanceReport(employeeAttendanceRequestViewModel);
            if (reportsList == null)
                response.Message.Add(SharedResources.attendanceReportsDoesNotExists);
            else
                response.Model = reportsList;
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<PaginationResponseViewModel<List<EmployeeMonthlyLeaveReportsViewModel>>>> GetEmployeesMonthlyLeaveReportByHR(EmployeesMonthlyLeaveRequestViewModel employeeMonthlyLeaveReportsViewModel)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<EmployeeMonthlyLeaveReportsViewModel>>>();
        try
        {
            if (employeeMonthlyLeaveReportsViewModel.departmentId == 0 || employeeMonthlyLeaveReportsViewModel.departmentId == null)
            {
                response.Message.Add(SharedResources.DepartmentIdIsRequired);
                return response;
            }
            var reportsList = await _unitOfWork.Reports.GetEmployeesMonthlyLeaveReportByHR(employeeMonthlyLeaveReportsViewModel);
            if (reportsList == null)
                response.Message.Add(SharedResources.attendanceReportsDoesNotExists);
            else
                response.Model = reportsList;
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    private bool ValidateReportDates(ReportsViewModel model)
    {
        model.FromDate ??= DateTime.Today;
        model.ToDate ??= DateTime.Today;
        return model.FromDate <= model.ToDate;
    }

    public async Task<ResponseModel<List<TeamReportResponseModel>>> GetTeamReportByDepartment(TeamReportRequestModel teamReportRequestModel)
    {
        var response = new ResponseModel<List<TeamReportResponseModel>>();
        try
        {
            var validateResponse = await ValidateReportParams(teamReportRequestModel.DepartmentId, teamReportRequestModel.TeamAdminId);
            if (validateResponse.Message!.Any())
            {
                response.Message.AddRange(validateResponse.Message!);
                return response;
            }
            teamReportRequestModel.DepartmentId = validateResponse.DepartmentId;
            if (validateResponse.TeamAdminId != null)
                teamReportRequestModel.TeamAdminId = validateResponse.TeamAdminId;
            var validationResult = await SharedResources.ValidateReportDateFilter(teamReportRequestModel.From, teamReportRequestModel.To);
            if (!validationResult.IsValid)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                return response;
            }
            teamReportRequestModel.From = validationResult.FromDate;
            teamReportRequestModel.To = validationResult.ToDate;

            var teamReports = await _unitOfWork.Reports.GetTeamReportByDepartment(teamReportRequestModel);
            response.Model = _mapper.Map<List<TeamReportResponseModel>>(teamReports);
            if (response.Model.Count == 0)
                response.Message.Add(SharedResources.NoDataFound);
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<PaginationResponseViewModel<List<EmployeeReportResponseModel>>>> GetEmployeeReportByDepartmentManager(EmployeeReportRequestViewModel employeeReportRequestViewModel)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<EmployeeReportResponseModel>>>();
        try
        {
            var validateResponse = await ValidateReportParams(employeeReportRequestViewModel.DepartmentId, employeeReportRequestViewModel.TeamAdminId);
            if (validateResponse.Message!.Any())
            {
                response.Message.AddRange(validateResponse.Message!);
                return response;
            }
            employeeReportRequestViewModel.DepartmentId = validateResponse.DepartmentId;
            if (validateResponse.TeamAdminId != null)
                employeeReportRequestViewModel.TeamAdminId = validateResponse.TeamAdminId;
            var teamReports = await _unitOfWork.Reports.GetEmployeesReportByDepartmentManager(employeeReportRequestViewModel);

            if (teamReports?.results?.Any() == true)
            {
                response.Model = new PaginationResponseViewModel<List<EmployeeReportResponseModel>>
                {
                    results = _mapper.Map<List<EmployeeReportResponseModel>>(teamReports.results),
                    TotalCount = teamReports.TotalCount
                };
            }
            else
                response.Message.Add(SharedResources.NoDataFound);
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<List<ClientsReportViewModel>>> GetClientsBillingReport(ClientReportRequestViewModel clientReportRequestModel)
    {
        var response = new ResponseModel<List<ClientsReportViewModel>>();
        try
        {
            var validateResponse = await ValidateReportParams(clientReportRequestModel.DepartmentId, clientReportRequestModel.TeamAdminId);
            if (validateResponse.Message!.Any())
            {
                response.Message.AddRange(validateResponse.Message!);
                return response;
            }
            clientReportRequestModel.DepartmentId = validateResponse.DepartmentId;
            if (validateResponse.TeamAdminId != null)
                clientReportRequestModel.TeamAdminId = validateResponse.TeamAdminId;
            if (clientReportRequestModel.ToDate > DateTime.Now)
            {
                response.Message.Add(SharedResources.ToDateNotAboveTodayDate);
                return response;
            }
            var validationResult = await SharedResources.ValidateReportDateFilter(clientReportRequestModel.FromDate, clientReportRequestModel.ToDate);
            if (!validationResult.IsValid)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                return response;
            }
            clientReportRequestModel.FromDate = validationResult.FromDate;
            clientReportRequestModel.ToDate = validationResult.ToDate;

            var clientReports = await _unitOfWork.Reports.GetClientsBillingReport(clientReportRequestModel);
            response.Model = _mapper.Map<List<ClientsReportViewModel>>(clientReports);
            if (response.Model.Count == 0)
                response.Message.Add(SharedResources.NoDataFound);
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }

    public async Task<ResponseModel<List<ProjectPaymentStatus>>> GetPaymentStatusReportAsync(string teamAdminId, int departmentId, string searchText)
    {
        var response = new ResponseModel<List<ProjectPaymentStatus>>();
        try
        {
            var ManagerId = string.Empty;
            var DepartmentId = 0;
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            if (claims.Role == "Project Manager")
            {
                DepartmentId = claims.DepartmentId;
                ManagerId = claims.UserId;
            }
            else if (claims.Role == "HOD")
            {
                DepartmentId = claims.DepartmentId;
                ManagerId = teamAdminId;
            }
            else if (claims.Role == "Admin")
            {
                if (departmentId == 0)
                {
                    response.Message.Add(SharedResources.DepartmentIdIsRequired);
                    return response;
                }
                else
                {
                    DepartmentId = departmentId;
                    ManagerId = teamAdminId;
                }
            }
            var reportsList = await _unitOfWork.Reports.GetPaymentStatusReportAsync(ManagerId, DepartmentId, searchText);

            if (reportsList == null || !reportsList.Any())
            {
                response.Model = new List<ProjectPaymentStatus>();
                return response;
            }
            response.Model = reportsList;
            return response;
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
            return response;
        }
    }

    public async Task<ResponseModel<List<WorkInHandReportViewModel>>> GetWorkInHandReportAsync(string? teamAdminId, int departmentId, string? searchText)
    {
        var response = new ResponseModel<List<WorkInHandReportViewModel>>();
        try
        {
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            if (claims.Role == "HOD" || claims.Role == "Admin")
            {
                if (teamAdminId != null)
                {
                    var isTeamAdminIdValid = await _unitOfWork.Employee.GetEmployeeById(teamAdminId, departmentId, string.Empty);
                    if (isTeamAdminIdValid != null)
                    {
                        if (isTeamAdminIdValid.RoleName != "HOD" && isTeamAdminIdValid.RoleName != "Project Manager")
                        {
                            response.Message.Add(SharedResources.InvalidTeamAdminId);
                            response.Model = new List<WorkInHandReportViewModel> { new WorkInHandReportViewModel { ProjectId = -1 } };
                            return response;
                        }
                        else
                            claims.UserId = teamAdminId;
                    }
                    else
                    {
                        response.Message.Add(SharedResources.NoManagerFound);
                        response.Model = new List<WorkInHandReportViewModel> { new WorkInHandReportViewModel { ProjectId = -1 } };
                        return response;
                    }
                }
                else
                {
                    claims.UserId = string.Empty;
                }
            }


            var reportsList = await _unitOfWork.Reports.GetWorkInHandReportAsync(claims.UserId, claims.DepartmentId, searchText);

            if (reportsList == null)
            {
                response.Message.Add(SharedResources.NoDataFound);
            }
            response.Model = reportsList;
            return response;
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
            return response;
        }
    }

    private async Task<ValidateResponseModel> ValidateReportParams(int DepartmentId, string? TeamAdminId)
    {
        var response = new ValidateResponseModel();
        response.Message = new();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, DepartmentId, string.Empty);
        switch (claims.Role)
        {
            case "Admin":
                if (DepartmentId == 0)
                    response.Message.Add(SharedResources.DepartmentIdIsRequired);
                else
                    response.DepartmentId = claims.DepartmentId;
                if (!string.IsNullOrEmpty(TeamAdminId))
                    response.TeamAdminId = TeamAdminId;
                break;
            case "Project Manager":
                response.DepartmentId = claims.DepartmentId;
                response.TeamAdminId = claims.UserId;
                break;
            case "HOD":
                response.DepartmentId = claims.DepartmentId;
                if (!string.IsNullOrEmpty(TeamAdminId) && !await _unitOfWork.Reports.CheckTeamAdminAndHodDepartment(response.DepartmentId, TeamAdminId))
                    response.Message.Add(SharedResources.ManagerDoesNotExists);
                break;
            case "HR":
                response.DepartmentId = claims.DepartmentId;
                break;
            default:
                return response;
        }
        return response;
    }

    public async Task<ResponseModel<List<GroupFullReportViewModel>>> GetFullReport(FullReportRequestViewModel model)
    {
        var response = new ResponseModel<List<GroupFullReportViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, model.DepartmentId, model.TeamAdminId);
        if (claims.Role == "HOD")
        {
            if (model.TeamAdminId == null)
            {
                response.Message.Add(SharedResources.TeamAdminIdIsRequired);
                response.Model = new List<GroupFullReportViewModel>
                {
                    new GroupFullReportViewModel
                    {
                        Date = DateTime.Now,
                        ReportViewModel = new List<FullReportViewModel>
                        {
                            new FullReportViewModel
                            {
                                Employee = "Error",
                            }
                        }
                    }
                };
                return response;
            }
            var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
            model.DepartmentId = claims_HOD.DepartmentId;
        }

        if (claims.Role == "Admin")
        {
            if (model.DepartmentId == 0)
            {

                response.Message.Add(SharedResources.DepartmentIdIsRequired);
                response.Model = new List<GroupFullReportViewModel>
                {
                    new GroupFullReportViewModel
                    {
                        Date = DateTime.Now,
                        ReportViewModel = new List<FullReportViewModel>
                        {
                            new FullReportViewModel
                            {
                                Employee = "Error",
                            }
                        }
                    }
                };
                return response;
            }
            //if (model.TeamAdminId == null)
            //{
            //    response.Message.Add(SharedResources.TeamAdminIdIsRequired);
            //    response.Model = new List<GroupFullReportViewModel>
            //    {
            //        new GroupFullReportViewModel
            //        {
            //            Date = DateTime.Now,
            //            ReportViewModel = new List<FullReportViewModel>
            //            {
            //                new FullReportViewModel
            //                {
            //                    Employee = "Error",
            //                }
            //            }
            //        }
            //    };
            //    return response;
            //}
        }

        if (claims.Role == "Project Manager")
        {
            model.TeamAdminId = claims.UserId;
            model.DepartmentId = claims.DepartmentId;
        }


        var getFullReport = await _unitOfWork.Reports.GetFullReportAsync(model);
        if (getFullReport != null)
        {
            response.Model = getFullReport;
        }
        return response;
    }

    public async Task<ResponseModel<List<DropDownResponse<string>>>> GetEmployeeListForFullReport(int departmentId, string? teamAdminId)
    {
        var response = new ResponseModel<List<DropDownResponse<string>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, teamAdminId);

        //if (claims.Role == "HOD")
        //{
        //    var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
        //    claims.DepartmentId = claims_HOD.DepartmentId;
        //}

        //if (claims.Role == "Admin")
        //{
        //    if (departmentId == 0)
        //    {
        //        response.Message.Add(SharedResources.DepartmentIdIsRequired);
        //        response.Model = new List<DropDownResponse<string>>
        //                {
        //                    new DropDownResponse<string>
        //                    {
        //                        Id = "Error"
        //                    }
        //                };
        //        return response;
        //    }
        //    claims.DepartmentId = departmentId;
        //}

        var employeeList = await _unitOfWork.Reports.GetEmployeeListForFullReportAsync(claims.UserId, claims.DepartmentId);
        if (employeeList != null)
        {
            response.Model = employeeList;
        }
        else
            response.Message.Add(SharedResources.NoEmployeeFound);
        return response;
    }
    public async Task<ResponseModel<List<DropDownResponse<int>>>> GetProjectListForFullReport(int departmentId, string? teamAdminId)
    {
        var response = new ResponseModel<List<DropDownResponse<int>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, teamAdminId);

        //if (claims.Role == "Admin")
        //{

        //}

        //if (claims.Role == "HOD")
        //{

        //}

        var projectList = await _unitOfWork.Reports.GetProjectListForFullReportAsync(claims.UserId, claims.DepartmentId);
        if (projectList != null)
        {
            response.Model = projectList;
        }
        else
            response.Message.Add(SharedResources.NoEmployeeFound);
        return response;
    }

    public async Task<ResponseModel<List<DropDownResponse<int>>>> GetClientListForFullReport(int departmentId, string? teamAdminId)
    {
        var response = new ResponseModel<List<DropDownResponse<int>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, teamAdminId);


        var clientList = await _unitOfWork.Reports.GetClientListForFullReportAsync(claims.UserId, claims.DepartmentId);
        if (clientList != null)
        {
            response.Model = clientList;
        }
        else
            response.Message.Add(SharedResources.NoEmployeeFound);

        return response;
    }
    public async Task<ResponseModel<List<MonthlyHourFullReportViewModel>>> GetMonthlyHoursListForFullReport(MonthlyHourFullReportRequestViewModel model)
    {
        var response = new ResponseModel<List<MonthlyHourFullReportViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

        var checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(model.EmployeeId, claims.DepartmentId, string.Empty);

        if (checkEmployeeExists != null)
        {
            if (claims.Role == "Project Manager")
            {
                var employeeList = await _unitOfWork.Employee.GetEmployeeListByManager(claims.UserId, claims.DepartmentId);
                if (employeeList != null)
                {
                    var employeeListByManager = employeeList.Select(employee => employee.Id).ToList();
                    var checkEmployeeIsAssigned = employeeListByManager.Contains(model.EmployeeId);

                    if (checkEmployeeIsAssigned != true)
                    {
                        response.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                        response.Model = new List<MonthlyHourFullReportViewModel> { new MonthlyHourFullReportViewModel { Month = "N/A" } };
                        return response;
                    }
                }
            }

            if (claims.Role == "HOD")
            {
                var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                if (checkEmployeeExists.DepartmentId != claims_HOD.DepartmentId)
                {
                    response.Message.Add(SharedResources.InvalidDepartment);
                    response.Model = new List<MonthlyHourFullReportViewModel> { new MonthlyHourFullReportViewModel { Month = "N/A" } };
                    return response;
                }
            }

            var monthlyHourReport = await _unitOfWork.Reports.GetMonthlyHoursListForFullReportAsync(model);

            if (monthlyHourReport != null)
            {
                response.Model = monthlyHourReport;
            }
            else
            {
                response.Message.Add(SharedResources.ReportNotFound);
                return response;
            }
        }
        else
        {
            response.Message.Add(SharedResources.EmployeeNotFound);
            response.Model = new List<MonthlyHourFullReportViewModel> { new MonthlyHourFullReportViewModel { Month = "N/A" } };
        }
        return response;
    }
    public async Task<ResponseModel<PaginationResponseViewModel<List<ProfileReportViewModel>>>> GetProfileReport(DateTime? fromDate, DateTime? toDate, int? pageSize, int? pageNumber)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<ProfileReportViewModel>>>();
        try
        {
            var requestModel = new ProfileReportRequestViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                PageSize = pageSize > 0 ? pageSize : (int?)null,
                PageNumber = pageNumber > 0 ? pageNumber : (int?)null
            };

            var profileReportData = await _unitOfWork.Reports.GetProfileReportAsync(requestModel);

            if (profileReportData == null || profileReportData.results.Count == 0)
            {
                response.Model = new PaginationResponseViewModel<List<ProfileReportViewModel>>
                {
                    TotalCount = 0,
                    results = new List<ProfileReportViewModel>()
                };
                response.Message.Add(SharedResources.NoDataFound);
            }
            else
            {
                response.Model = profileReportData;
            }
        }
        catch (Exception ex)
        {
            response.Message.Add(SharedResources.InternalServerError);
        }
        return response;
    }
}