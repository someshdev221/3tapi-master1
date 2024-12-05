using AutoMapper;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

namespace TimeTaskTracking.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public DashboardService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<ResponseModel<TeamProductivityViewModel>> GetTeamProductivity(TeamLeadProductivityViewModel teamsProductivityRequest)
        {
            var response = new ResponseModel<TeamProductivityViewModel>();
            try
            {
                var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
                var teamLeadId = string.Empty;
                var teamLeadDepartmentId = 0;

                var dateValidation = await SharedResources.ValidateReportDateFilter(teamsProductivityRequest.FromDate, teamsProductivityRequest.ToDate);
                if (!dateValidation.IsValid)
                {
                    response.Model = new TeamProductivityViewModel { TeamLeadId = "BadRequest" };
                    response.Message.Add(SharedResources.ToDateMustGreater);
                    return response;
                }

                var getTeamLeadDetails = await _unitOfWork.Employee.GetEmployeeById(teamsProductivityRequest.TeamLeadId, 0, string.Empty);

                if (claims.Role == "Admin" || claims.Role == "HOD" || claims.Role == "Project Manager")
                {
                    if (teamsProductivityRequest.TeamLeadId == null)
                    {
                        response.Message.Add(SharedResources.TeamLeadIdIsRequired);
                        response.Model = new TeamProductivityViewModel { TeamLeadId = "BadRequest" };
                        return response;
                    }
                    else
                    {
                        if (getTeamLeadDetails == null)
                        {
                            response.Message.Add(SharedResources.TeamLeadNotFound);
                            return response;
                        }
                        //if (getTeamLeadDetails != null && getTeamLeadDetails.RoleName != "Team Lead")
                        //{
                        //    response.Message.Add(SharedResources.NotAValidTeamLeadId);
                        //    return response;
                        //}
                        if (getTeamLeadDetails != null)
                        {
                            teamLeadId = teamsProductivityRequest.TeamLeadId;
                            teamLeadDepartmentId = getTeamLeadDetails.DepartmentId;
                        }
                    }  
                }
                else
                {
                    teamLeadId = claims.UserId;
                    teamLeadDepartmentId = claims.DepartmentId;
                }
                var teamProductivity = await _unitOfWork.Dashboard.GetTeamProductivity(teamLeadId, teamsProductivityRequest, teamLeadDepartmentId);
                if (teamProductivity == null)
                {
                    response.Message.Add(SharedResources.TeamsProductivitySummaryNotFound);
                    response.Model = new TeamProductivityViewModel { TeamLeadId = "ERROR" };
                } 
                else
                    response.Model = teamProductivity;     
            }
            catch (Exception ex)
            {
                response.Message.Add(SharedResources.InternalServerError);
            }
            return response;
        }

        public async Task<ResponseModel<List<TeamPerformanceViewModel>>> PerformanceDetails(DateFilterViewModel model, string? teamLeadId)
        {
            var response = new ResponseModel<List<TeamPerformanceViewModel>>();

            if (model.FromDate == null || model.ToDate == null)
            {
                response.Model = new List<TeamPerformanceViewModel> { new TeamPerformanceViewModel { EmployeeId = "ERROR" } }; 
                response.Message.Add(SharedResources.DatesAreRequired);
                return response;
            }

            var dateValidation = await SharedResources.ValidateReportDateFilter(model.FromDate, model.ToDate);
            if (!dateValidation.IsValid)
            {
                response.Model = new List<TeamPerformanceViewModel> { new TeamPerformanceViewModel { EmployeeId = "ERROR" } };
                response.Message.Add(SharedResources.ToDateMustGreater);
                return response;
            }

            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            if (claims.Role == "Team Lead")
                teamLeadId = claims.UserId;

            var teamPerformanceDetails = await _unitOfWork.Dashboard.TeamPerformance(teamLeadId, claims.DepartmentId, model);
            if (teamPerformanceDetails != null && teamPerformanceDetails.Any())
            {
                var teamPerformanceDtoList = _mapper.Map<List<TeamPerformanceViewModel>>(teamPerformanceDetails);
                response.Model = teamPerformanceDtoList;
            }
            else
            {
                response.Message.Add(SharedResources.NoRecordFound);
            }

            return response;
        }

        public async Task<ResponseModel<List<EmployeeProjectModuleBillingDetailsViewModel>>> EmployeeProjectBillingByModule(EmployeeRequestViewModel model, int departmentId)
        {
            var response = new ResponseModel<List<EmployeeProjectModuleBillingDetailsViewModel>>();

            if (model.FromDate == null || model.ToDate == null)
            {
                response.Model = new List<EmployeeProjectModuleBillingDetailsViewModel> { new EmployeeProjectModuleBillingDetailsViewModel { ProjectId = -1 } }; // Use a specific value to indicate an error
                response.Message.Add(SharedResources.DatesAreRequired);
                return response;
            }

            var DateValidation = await SharedResources.ValidateReportDateFilter(model.FromDate, model.ToDate);
            if (!DateValidation.IsValid)
            {
                response.Model = new List<EmployeeProjectModuleBillingDetailsViewModel> { new EmployeeProjectModuleBillingDetailsViewModel { ProjectId = -1 } };
                response.Message.Add(SharedResources.ToDateMustGreater);
                return response;
            }

            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            switch (claims.Role)
            {
                case "Admin":
                    if (departmentId == 0)
                    { response.Model = new List<EmployeeProjectModuleBillingDetailsViewModel> {new EmployeeProjectModuleBillingDetailsViewModel { ProjectId = -1 }};
                        response.Message.Add(SharedResources.DepartmentIdIsRequired);
                        return response;
                    }
                    break;

                case "HOD":
                    var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                    departmentId = claims_HOD.DepartmentId;
                    break;

                case "Project Manager":
                case "Team Lead":
                    departmentId = claims.DepartmentId;
                    break;
            }

            var billingDetailsModuleWise = await _unitOfWork.Dashboard.BillingDetailsModuleWise(model, departmentId);
            if (billingDetailsModuleWise != null && billingDetailsModuleWise.Any())
            {
                response.Model = _mapper.Map<List<EmployeeProjectModuleBillingDetailsViewModel>>(billingDetailsModuleWise);
            }
            else
                response.Message.Add(SharedResources.NoRecordFound);
            return response;
        }


        public async Task<ResponseModel<List<ProjectsDetailViewModel>>> ProjectDetails(DateFilterViewModel model, string? teamLeadId, int departmentId)
        {
            var response = new ResponseModel<List<ProjectsDetailViewModel>>();

            // Check if the date filter is provided
            if (model.FromDate == null || model.ToDate == null)
            {
                response.Model = new List<ProjectsDetailViewModel> { new ProjectsDetailViewModel { ProjectId = -1 } };
                response.Message.Add(SharedResources.DatesAreRequired);
                return response;
            }

            // Validate the date range
            var DateValidation = await SharedResources.ValidateReportDateFilter(model.FromDate, model.ToDate);
            if (!DateValidation.IsValid)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                response.Model = new List<ProjectsDetailViewModel> { new ProjectsDetailViewModel { ProjectId = -1 } };
                return response;
            }

            // Retrieve claims including role and department ID
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);


            if (claims.Role == "Admin")
            {
                if (departmentId == 0)
                {
                    response.Message.Add(SharedResources.DepartmentIdIsRequired);
                    response.Model = null;
                    return response;
                }

                if (teamLeadId == null)
                {
                    response.Message.Add(SharedResources.TeamLeadIdIsRequired);
                    response.Model = null;
                    return response;
                }
            }

            if (claims.Role == "HOD")
            {
                var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                departmentId = claims_HOD.DepartmentId;
            }

            if (claims.Role == "Project Manager")
            {
                departmentId = claims.DepartmentId;
            }

            // Assign TeamLeadId from claims if user is a Team Lead
            if (claims.Role == "Team Lead")
                teamLeadId = claims.UserId;
            var projectDetails = await _unitOfWork.Dashboard.ProjectDetailsByTeamLead(teamLeadId, departmentId, model);
             if (projectDetails != null && projectDetails.Any())
            {
                var teamPerformanceDtoList = _mapper.Map<List<ProjectsDetailViewModel>>(projectDetails);
                response.Model = teamPerformanceDtoList;
            }
            else
            {
                response.Message.Add(SharedResources.NoRecordFound);
            }
            return response;
        }

        public async Task<ResponseModel<List<CompleteModuleDetailsViewModel>>> ModuleBillingDetails(ModuleBillingRequestViewModel model, int departmentId)
        {
            var response = new ResponseModel<List<CompleteModuleDetailsViewModel>>();


            if (model.FromDate == null || model.ToDate == null)
            {
                response.Model = new List<CompleteModuleDetailsViewModel> { new CompleteModuleDetailsViewModel { ModuleId = "ERROR" } };
                response.Message.Add(SharedResources.DatesAreRequired);
                return response;
            }

            var DateValidation = await SharedResources.ValidateReportDateFilter(model.FromDate, model.ToDate);
            if (!DateValidation.IsValid)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                response.Model = new List<CompleteModuleDetailsViewModel> { new CompleteModuleDetailsViewModel { ModuleId = "ERROR" } };
                return response;
            }


            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            switch (claims.Role)
            {
                case "Admin":
                    if (departmentId == 0)
                    {
                        response.Message.Add(SharedResources.DepartmentIdIsRequired);
                        response.Model = new List<CompleteModuleDetailsViewModel> { new CompleteModuleDetailsViewModel { ModuleId = "ERROR" } };
                        return response;
                    }
                    else
                        claims.DepartmentId = departmentId;
                    break;

                case "HOD":
                    var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                    claims.DepartmentId = claims_HOD.DepartmentId;
                    break;
            }
            var moduleDetails = await _unitOfWork.Dashboard.ModuleDetailsByTeamLead(model, claims.DepartmentId);
            if (moduleDetails != null && moduleDetails.Any())
            {
                var teamPerformanceDetails = _mapper.Map<List<CompleteModuleDetailsViewModel>>(moduleDetails);

                response.Model = teamPerformanceDetails;
            }
            else
                response.Message.Add(SharedResources.NoRecordFound);
            return response;
        }

        public async Task<ResponseModel<List<ProjectDetailsModuleAndEmployeeViewModel>>> GetProjectDetailsModuleAndEmployeeWiseByTeamLead(ProjectRequestViewModel model, int departmentId)
        {
            var response = new ResponseModel<List<ProjectDetailsModuleAndEmployeeViewModel>>();


            if (model.FromDate == null || model.ToDate == null)
            {
                response.Model = new List<ProjectDetailsModuleAndEmployeeViewModel> { new ProjectDetailsModuleAndEmployeeViewModel { EmployeeId = "ERROR" } };
                response.Message.Add(SharedResources.DatesAreRequired);
                return response;
            }

            var DateValidation = await SharedResources.ValidateReportDateFilter(model.FromDate, model.ToDate);
            if (!DateValidation.IsValid)
            {
                response.Message.Add(SharedResources.ToDateMustGreater);
                response.Model = new List<ProjectDetailsModuleAndEmployeeViewModel> { new ProjectDetailsModuleAndEmployeeViewModel { EmployeeId = "ERROR" } };
                return response;
            }

            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

            switch (claims.Role)
            {
                case "Admin":
                    if (departmentId == 0)
                    {
                        response.Model = new List<ProjectDetailsModuleAndEmployeeViewModel> { new ProjectDetailsModuleAndEmployeeViewModel { EmployeeId = "ERROR" } };
                        response.Message.Add(SharedResources.DepartmentIdIsRequired);
                        return response;
                    }
                    if(model.TeamLeadId == null)
                    {
                        response.Model = new List<ProjectDetailsModuleAndEmployeeViewModel> { new ProjectDetailsModuleAndEmployeeViewModel { EmployeeId = "ERROR" } };
                        response.Message.Add(SharedResources.TeamLeadIdIsRequired);
                        return response;
                    }
                    else
                        claims.DepartmentId = departmentId;
                    break;

                case "HOD":
                    if (model.TeamLeadId == null)
                    {
                        response.Model = new List<ProjectDetailsModuleAndEmployeeViewModel> { new ProjectDetailsModuleAndEmployeeViewModel { EmployeeId = "ERROR" } };
                        response.Message.Add(SharedResources.TeamLeadIdIsRequired);
                        return response;
                    }
                    else
                    {
                        var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                        claims.DepartmentId = claims_HOD.DepartmentId;
                    }
                    break;

                case "Project Manager":
                    if (model.TeamLeadId == null)
                    {
                        response.Model = new List<ProjectDetailsModuleAndEmployeeViewModel> { new ProjectDetailsModuleAndEmployeeViewModel { EmployeeId = "ERROR" } };
                        response.Message.Add(SharedResources.TeamLeadIdIsRequired);
                        return response;
                    }
                    break;

                case "Team Lead":
                    model.TeamLeadId = claims.UserId;
                    break;
            }

            var projectModulesBillingDetailsEmployeeWise = await _unitOfWork.Dashboard.GetProjectDetailsModuleAndEmployeeWiseByTeamLeadAsync(model, claims.DepartmentId);
            if (projectModulesBillingDetailsEmployeeWise != null && projectModulesBillingDetailsEmployeeWise.Any())
            {
                response.Model = projectModulesBillingDetailsEmployeeWise;
            }
            else
                response.Message.Add(SharedResources.NoRecordFound);
            return response;
        }

        public async Task<ResponseModel<List<TeamAttendanceViewModel>>> TeamAttendanceDetails(TeamLeadRequestViewModel model)
        {
            var response = new ResponseModel<List<TeamAttendanceViewModel>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

            var getTeamLeadDetails = new Register();

            int departmentId = 0;

            if (claims.Role == "Team Lead")
            {
                model.TeamLeadId = claims.UserId;
                departmentId = claims.DepartmentId;
            }

            if (claims.Role == "Project Manager" || claims.Role == "HOD" || claims.Role == "Admin")
            {
                if (model.TeamLeadId == null)
                {
                    response.Model = new List<TeamAttendanceViewModel> { new TeamAttendanceViewModel { EmployeeName = "ERROR" } };
                    response.Message.Add(SharedResources.TeamLeadIdIsRequired);
                    return response;
                }
                getTeamLeadDetails = await _unitOfWork.Employee.GetEmployeeById(model.TeamLeadId, claims.DepartmentId, string.Empty);
                departmentId = getTeamLeadDetails.DepartmentId;
            }

            var teamAttedance = await _unitOfWork.Dashboard.AttendanceDetails(departmentId, model);
            if (teamAttedance != null && teamAttedance.Any())
            {
                var teamAttendanceDetails = _mapper.Map<List<TeamAttendanceViewModel>>(teamAttedance);
                response.Model = teamAttendanceDetails;
            }
            else
                response.Message.Add(SharedResources.NoRecordFound);
            return response;
        }

        public async Task<ResponseModel<List<TeamLeadPerformanceViewModel>>> TeamLeadPerformaceDetails(TeamLeadRequestViewModel model)
        {
            var response = new ResponseModel<List<TeamLeadPerformanceViewModel>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var teamLeadPerformace = await _unitOfWork.Dashboard.PerformanceDetails(claims.UserId, claims.DepartmentId, model);
            if (teamLeadPerformace != null && teamLeadPerformace.Any())
            {
                response.Model = _mapper.Map<List<TeamLeadPerformanceViewModel>>(teamLeadPerformace);
            }
            else
                response.Message.Add(SharedResources.NoRecordFound);
            return response;
        }


        //public async Task<ResponseModel<ProductivityResponseViewModel<List<TeamProducitivityViewModel>>>> GetTeamProductivity(TeamLeadRequestViewModel model)
        //{
        //    var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
        //    var expectedProductivity = await _unitOfWork.Dashboard.GetTeamProductivity(claims.UserId, model);
        //    var deliveredProductivity = await _unitOfWork.Dashboard.TeamPerformance(claims.UserId, model);

        //    decimal totalBillingSum = 0;

        //    if (deliveredProductivity != null)
        //    {
        //        totalBillingSum = deliveredProductivity
        //            .Where(dp => !string.IsNullOrEmpty(dp.TotalBilling)) // Ensure TotalBilling is not null or empty
        //            .Sum(dp => decimal.TryParse(dp.TotalBilling, out var billing) ? billing : 0);
        //    }

        //    decimal? productivityPercentage = null;
        //    if (expectedProductivity?.EstimatedProductivityHours.HasValue == true && expectedProductivity.EstimatedProductivityHours.Value > 0)
        //    {
        //        productivityPercentage = Math.Round((totalBillingSum / expectedProductivity.EstimatedProductivityHours.Value) * 100, 1);
        //    }
        //    else
        //    {
        //        productivityPercentage = 0;
        //    }

        //    return new ResponseModel<ProductivityResponseViewModel<List<TeamProducitivityViewModel>>>
        //    {
        //        Model = new ProductivityResponseViewModel<List<TeamProducitivityViewModel>>
        //        {
        //            EstimatedProductivityHours = expectedProductivity?.EstimatedProductivityHours ?? 0,
        //            TeamLeadName = expectedProductivity?.TeamLeadName,
        //            DeliveredProductivityHours = totalBillingSum,
        //            ProductivityPercentage = productivityPercentage
        //        },
        //    };
        //}



        public async Task<ResponseModel<List<UserBadgeViewModel>>> GetAssignedBadges()
        {
            var response = new ResponseModel<List<UserBadgeViewModel>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var teamLeadBadges = await _unitOfWork.Dashboard.GetAssignedUserBadges(claims.UserId, claims.DepartmentId);
            if (teamLeadBadges != null && teamLeadBadges.Any())
            {
                response.Model = _mapper.Map<List<UserBadgeViewModel>>(teamLeadBadges);
            }
            return response;
        }
        public async Task<ResponseModel<List<TraineeDto>>> GetTraineeListAsync()
        {
            var response = new ResponseModel<List<TraineeDto>>
            {
                Message = new List<string>(),
                Model = new List<TraineeDto>()
            };

            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var traineeResponse = await _unitOfWork.Dashboard.GetTraineeListAsync(claims.UserId);

            if (traineeResponse.Model != null && traineeResponse.Model.Any())
            {
                response.Model = _mapper.Map<List<TraineeDto>>(traineeResponse.Model);
            }

            return response;
        }

    }
}

