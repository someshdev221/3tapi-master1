using AutoMapper;
using Microsoft.AspNetCore.Http;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services;

public class PerformanceService : IPerformanceService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _contextAccessor;
    public PerformanceService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<ResponseModel<EmployeeAttendanceSummaryViewModel>> GetEmployeeAttendanceSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var result = new ResponseModel<EmployeeAttendanceSummaryViewModel>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, employeeProjectRequestModel.DepartmentId, string.Empty);
        employeeProjectRequestModel.DepartmentId = claims.DepartmentId;
        var employeeAttendanceSummary = await _unitOfWork.Performance.GetEmployeeAttendanceSummary(employeeProjectRequestModel);
        if (employeeAttendanceSummary == null)
            result.Message.Add(SharedResources.attendanceReportsDoesNotExists);
        result.Model = employeeAttendanceSummary;
        return result;
    }

    public async Task<ResponseModel<List<MonthlyBillingSummaryViewModel>>> GetEmployeeMonthlyBillingSummary(string employeeId,int departmentId)
    {
        var result = new ResponseModel<List<MonthlyBillingSummaryViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
        var monthlyBillingSummary = await _unitOfWork.Performance.GetEmployeeMonthlyBillingSummary(employeeId,claims.DepartmentId);
        if (monthlyBillingSummary == null)
            result.Message.Add(SharedResources.NoBillingDetailsFound);
        result.Model = monthlyBillingSummary;
        return result;
    }

    public async Task<ResponseModel<List<EmployeePerformanceSummaryViewModel>>> GetEmployeeWorkedProjectSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var result = new ResponseModel<List<EmployeePerformanceSummaryViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, employeeProjectRequestModel.DepartmentId, string.Empty);
        employeeProjectRequestModel.DepartmentId = claims.DepartmentId;
        var employeePerformanceSummary = await _unitOfWork.Performance.GetEmployeeWorkedProjectSummary(employeeProjectRequestModel);
        if (employeePerformanceSummary == null)
            result.Message.Add(SharedResources.ProjectSummaryNotFound);
        result.Model = employeePerformanceSummary;
        return result;
    }

    public async Task<ResponseModel<List<ProjectPerformanceDetailsViewModel>>> GetEmployeeWorkedProjectWithBillingDetailSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var result = new ResponseModel<List<ProjectPerformanceDetailsViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, employeeProjectRequestModel.DepartmentId, string.Empty);
        employeeProjectRequestModel.DepartmentId = claims.DepartmentId;
        var employeeWorkedProjectWithBillingDetail = await _unitOfWork.Performance.GetEmployeeWorkedProjectWithBillingDetailSummary(employeeProjectRequestModel);
        if (employeeWorkedProjectWithBillingDetail == null)
            result.Message.Add(SharedResources.ProjectSummaryNotFound);
        result.Model = employeeWorkedProjectWithBillingDetail;
        return result;
    }
    public async Task<ResponseModel<EmployeeAttendanceSummaryViewModel>> GetTeamAttendanceSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var result = new ResponseModel<EmployeeAttendanceSummaryViewModel>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, employeeProjectRequestModel.DepartmentId, string.Empty);
        employeeProjectRequestModel.DepartmentId = claims.DepartmentId;
        var teamAttendanceSummary = await _unitOfWork.Performance.GetTeamAttendanceSummary(employeeProjectRequestModel);
        if (teamAttendanceSummary == null)
            result.Message.Add(SharedResources.attendanceReportsDoesNotExists);
        result.Model = teamAttendanceSummary;
        return result;
    }

    public async Task<ResponseModel<List<MonthlyBillingSummaryViewModel>>> GetTeamMonthlyBillingSummary(string employeeId,int departmentId)
    {
        var result = new ResponseModel<List<MonthlyBillingSummaryViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
        var monthlyBillingSummary = await _unitOfWork.Performance.GetTeamMonthlyBillingSummary(employeeId, claims.DepartmentId);
        if (monthlyBillingSummary == null)
            result.Message.Add(SharedResources.NoBillingDetailsFound);
        result.Model = monthlyBillingSummary;
        return result;
    }

    public async Task<ResponseModel<List<EmployeePerformanceSummaryViewModel>>> GetTeamWorkedProjectSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var result = new ResponseModel<List<EmployeePerformanceSummaryViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, employeeProjectRequestModel.DepartmentId, string.Empty);
        employeeProjectRequestModel.DepartmentId = claims.DepartmentId;
        var teamPerformanceSummary = await _unitOfWork.Performance.GetTeamWorkedProjectSummary(employeeProjectRequestModel);
        if (teamPerformanceSummary == null)
            result.Message.Add(SharedResources.ProjectSummaryNotFound);
        result.Model = teamPerformanceSummary;
        return result;
    }

    public async Task<ResponseModel<List<ProjectPerformanceDetailsViewModel>>> GetTeamWorkedProjectWithBillingDetailSummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var result = new ResponseModel<List<ProjectPerformanceDetailsViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, employeeProjectRequestModel.DepartmentId, string.Empty);
        employeeProjectRequestModel.DepartmentId = claims.DepartmentId;
        var teamWorkedProjectWithBillingDetail = await _unitOfWork.Performance.GetTeamWorkedProjectWithBillingDetailSummary(employeeProjectRequestModel);
        if (teamWorkedProjectWithBillingDetail == null)
            result.Message.Add(SharedResources.ProjectSummaryNotFound);
        result.Model = teamWorkedProjectWithBillingDetail;
        return result;
    }
    public async Task<ResponseModel<List<TeamProductivityPerformanceReportViewModel>>> GetTeamProductivitySummary(DepartmentEmployeeProjectRequestModel employeeProjectRequestModel)
    {
        var result = new ResponseModel<List<TeamProductivityPerformanceReportViewModel>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, employeeProjectRequestModel.DepartmentId, string.Empty);
        employeeProjectRequestModel.DepartmentId = claims.DepartmentId;
        var getTeamProductivitySummary = await _unitOfWork.Performance.GetTeamProductivitySummary(employeeProjectRequestModel);
        if (getTeamProductivitySummary == null)
            result.Message.Add(SharedResources.ProjectSummaryNotFound);
        result.Model = getTeamProductivitySummary;
        return result;
    }
}
