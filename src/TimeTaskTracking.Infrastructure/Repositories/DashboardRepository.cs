using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ExecuteProcedure _exec;
        
        public DashboardRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);
            
        }


        public async Task<List<TeamPerformanceViewModel>> TeamPerformance(string teamLeadId, int departmentId, DateFilterViewModel model)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetEmployeePerformanceDetailsByTeamLead",
                    new string[] { "@TeamLeadId", "@DepartmentId", "@FromDate", "@ToDate" },
                    new string[] { teamLeadId, Convert.ToString(departmentId), Convert.ToString(model.FromDate), Convert.ToString(model.ToDate) });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }

                List<TeamPerformanceViewModel> TeamPerformanceModel = new List<TeamPerformanceViewModel>();
                TeamPerformanceModel = await _exec.DataTableToListAsync<TeamPerformanceViewModel>(obj);

                // Calculate ProductivityPercentage for each employee
                foreach (var employee in TeamPerformanceModel)
                {
                    if (employee.UpworkHours.HasValue && employee.FixedHours.HasValue && employee.NonBillableHours.HasValue)
                    {
                        var upworkFixedSum = employee.UpworkHours.Value + employee.FixedHours.Value;
                        var totalHours = upworkFixedSum + employee.NonBillableHours.Value;

                        if (totalHours > 0)
                        {
                            employee.ProductivityPercentage = (upworkFixedSum / totalHours) * 100;
                        }
                        else
                        {
                            employee.ProductivityPercentage = 0; // or any default value if total hours are zero
                        }
                    }
                }

                return TeamPerformanceModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ProjectsDetailViewModel>> ProjectDetailsByTeamLead(string teamLeadId, int departmentId, DateFilterViewModel model)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetProjectDetailsByTeamLead",
                    new string[] { "@TeamLeadId", "@DepartmentId", "@FromDate", "@ToDate" },
                    new string[] { teamLeadId, Convert.ToString(departmentId), Convert.ToString(model.FromDate), Convert.ToString(model.ToDate)});

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }

                List<ProjectsDetailViewModel> projectDetailModel = new List<ProjectsDetailViewModel>();
                projectDetailModel = await _exec.DataTableToListAsync<ProjectsDetailViewModel>(obj);

                return projectDetailModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<CompleteModuleDetailsViewModel>> ModuleDetailsByTeamLead(ModuleBillingRequestViewModel model, int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetModuleBillingDetailsByTeamLead",
                    new string[] { "@ProjectId","@DepartmentId", "@FromDate", "@ToDate", "@PaymentStatus", "@ModuleStatus" },
                    new string[] { model.ProjectId.ToString(), Convert.ToString(departmentId), Convert.ToString(model.FromDate), Convert.ToString(model.ToDate), model.PaymentStatus ?? "", model.ModuleStatus ?? ""});

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }

                List<CompleteModuleDetailsViewModel> moduleDetailModel = new List<CompleteModuleDetailsViewModel>();
                moduleDetailModel = await _exec.DataTableToListAsync<CompleteModuleDetailsViewModel>(obj);

                return moduleDetailModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ProjectDetailsModuleAndEmployeeViewModel>> GetProjectDetailsModuleAndEmployeeWiseByTeamLeadAsync(ProjectRequestViewModel model, int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetProjectBillingDetailsByModuleAndEmployee",
                    new string[] { "@TeamLeadId", "@ProjectId", "@DepartmentId", "@FromDate", "@ToDate" },
                    new string[] { model.TeamLeadId, Convert.ToString(model.ProjectId), Convert.ToString(departmentId), Convert.ToString(model.FromDate), Convert.ToString(model.ToDate) });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }

                var groupedData = obj.AsEnumerable()
                    .GroupBy(row => new
                    {
                        EmployeeId = row.Field<string>("EmployeeId"),
                        EmployeeName = row.Field<string>("EmployeeName")
                    })
                    .Select(g => new ProjectDetailsModuleAndEmployeeViewModel
                    {
                        EmployeeId = g.Key.EmployeeId,
                        EmployeeName = g.Key.EmployeeName,
                        ModuleDetails = g.Select(row => new ModuleDetailsViewModel
                        {
                            ModuleId = row.Field<Guid>("ModuleId").ToString(),
                            ModuleName = row.Field<string>("ModuleName"),
                            UpworkHours = row.Field<decimal>("UpworkHours"),
                            FixedHours = row.Field<decimal>("FixedHours"),
                            NonBillableHours = row.Field<decimal>("NonBillableHours"),
                            TotalHours = row.Field<decimal>("TotalHours"),
                            BillingHours = row.Field<decimal>("BillingHours"),
                        }).ToList()
                    }).ToList();

                return groupedData;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                return null;
            }
        }


        public async Task<List<TeamAttendanceViewModel>> AttendanceDetails(int departmentId, TeamLeadRequestViewModel model)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetDashboardAttendanceByTeamLead",
                    new string[] { "@TeamLeadId", "@DepartmentId", "@Month", "@Year" },
                    new string[] { model.TeamLeadId, Convert.ToString(departmentId),  model.Month.ToString(), model.Year.ToString() });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }

                // Group by EmployeeId
                var groupedData = obj.AsEnumerable().GroupBy(row => row.Field<string>("EmployeeID"));

                List<TeamAttendanceViewModel> teamAttendanceModels = new List<TeamAttendanceViewModel>();

                foreach (var group in groupedData)
                {
                    var employeeId = group.Key;
                    var employeeName = group.First().Field<string>("EmployeeName");
                    var employeeNumber = group.First().Field<string>("EmployeeNumber");

                    // Create AttendanceViewModel for each EmployeeId
                    var attendanceViewModels = group.Select(row => new AttendanceViewModel
                    {
                        AttendanceStatus = row.Field<string>("AttendanceStatus"),
                        Day = row.Field<int>("Day"),
                        FixedHours = row.Field<decimal?>("FixedHours"),
                        UpworkHours = row.Field<decimal?>("UpworkHours"),
                        OfflineHours = row.Field<decimal?>("OfflineHours"),
                        TotalHours = row.Field<decimal?>("TotalHours")
                    }).ToList();

                    // Create TeamAttendanceViewModel
                    var teamAttendanceModel = new TeamAttendanceViewModel
                    {
                        EmployeeId = employeeId,
                        EmployeeName = employeeName,
                        EmployeeNumber = employeeNumber,
                        EmployeeAttendance = attendanceViewModels
                    };

                    teamAttendanceModels.Add(teamAttendanceModel);
                }

                return teamAttendanceModels;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<List<TeamLeadPerformanceViewModel>> PerformanceDetails(string teamLeadId, int departmentId, TeamLeadRequestViewModel model)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetTeamLeadPerformanceDetails",
                    new string[] { "@TeamLeadId", "@DepartmentId", "@Month", "@Year" },
                    new string[] { teamLeadId, Convert.ToString(departmentId), model.Month.ToString(), model.Year.ToString() });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }

                List<TeamLeadPerformanceViewModel> teamLeadPerformanceModel = new List<TeamLeadPerformanceViewModel>();
                teamLeadPerformanceModel = await _exec.DataTableToListAsync<TeamLeadPerformanceViewModel>(obj);
                return teamLeadPerformanceModel;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TeamProductivityViewModel> GetTeamProductivity(string teamLeadId, TeamLeadProductivityViewModel teamsProductivityRequest,int departmentId)
        {
            var teamProductivity = await _exec.Get_DataTableAsync("TeamLeadProductivity",
                 new string[] { "@TeamLeadId", "@DepartmentId", "@FromDate", "@ToDate" },
                 new string[] { Convert.ToString(teamLeadId), Convert.ToString(departmentId),
                       Convert.ToString(teamsProductivityRequest.FromDate),Convert.ToString(teamsProductivityRequest.ToDate) });
            if (teamProductivity != null && teamProductivity.Rows.Count > 0)
            {
                return await _exec.DataTableToModelAsync<TeamProductivityViewModel>(teamProductivity);
            }
            return null;
        }
        //public async Task<ProductivityResponseViewModel<List<TeamProducitivityViewModel>>> GetTeamProductivity(string teamLeadId, TeamLeadRequestViewModel model)
        //{
        //    var workingDays = await SharedResources.GetWorkingDaysCount(model.Month, model.Year);

        //    try
        //    {
        //        var dataset = await _exec.Get_DataSetAsync("GetTeamProductivity",
        //           new string[] { "OptId", "@TotalWorkingdays", "@TeamLeadId", "@Month", "@Year" },
        //           new string[] { "1", workingDays.ToString(), teamLeadId, model.Month.ToString(), model.Year.ToString() });

        //        if (dataset == null || dataset?.Tables.Count == 0)
        //        {
        //            return null;
        //        }
        //        ProductivityResponseViewModel<List<TeamProducitivityViewModel>> productivityResponseViewModel = new();
        //        productivityResponseViewModel.results = await _exec.DataTableToListAsync<TeamProducitivityViewModel>(dataset.Tables[0]);
        //        productivityResponseViewModel.EstimatedProductivityHours = Convert.ToInt32(dataset.Tables[1].Rows[0][0]);
        //        productivityResponseViewModel.TeamLeadName = dataset.Tables[2].Rows[0]["TeamLeadName"].ToString();

        //        return productivityResponseViewModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public async Task<List<EmployeeProjectModuleBillingDetailsViewModel>> BillingDetailsModuleWise(EmployeeRequestViewModel model, int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetEmployeeProjectBillingDetailsModuleWise",
                    new string[] { "@EmployeeId", "@DepartmentId", "@FromDate", "@ToDate" },
                    new string[] { model.EmployeeId.ToString(), Convert.ToString(departmentId), Convert.ToString(model.FromDate), Convert.ToString(model.ToDate) });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }

                // Step 2: Group data by ProjectId
                var groupedData = obj.AsEnumerable()
                    .GroupBy(row => new
                    {
                        ProjectId = row.Field<int>("ProjectId"),
                        ProjectName = row.Field<string>("ProjectName")
                    })
                    .Select(group => new EmployeeProjectModuleBillingDetailsViewModel
                    {
                        ProjectId = group.Key.ProjectId,
                        ProjectName = group.Key.ProjectName,
                        ModuleDetails = group.Select(row => new ModuleDetailsViewModel
                        {
                            ModuleId = row.Field<Guid>("ModuleId").ToString(),  // Convert Guid to string
                            ModuleName = row.Field<string>("ModuleName"),
                            UpworkHours = row.Field<decimal?>("UpworkHours"),
                            FixedHours = row.Field<decimal?>("FixedHours"),
                            BillingHours = row.Field<decimal?>("BillingHours"),
                            TotalHours = row.Field<decimal?>("TotalHours"),
                            NonBillableHours = row.Field<decimal?>("NonBillableHours")
                        }).ToList()
                    }).ToList();

                return groupedData;
            }
            catch (Exception ex)
            {
                // Log the exception (omitted for brevity)
                return null;
            }
        }


        public async Task<List<UserBadgeViewModel>> GetAssignedUserBadges(string teamLeadId , int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetUserBadgesByEmployeeId",
                    new string[] { "@EmpId", "@DepartmentId" },
                    new string[] { teamLeadId, Convert.ToString(departmentId) });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }

                var teamLeadBadges = await _exec.DataTableToListAsync<UserBadgeViewModel>(obj);
                return teamLeadBadges;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ResponseModel<List<TraineeViewModel>>> GetTraineeListAsync(string teamLeadId)
        {
            var response = new ResponseModel<List<TraineeViewModel>>
            {
                Message = new List<string>(),
                Model = new List<TraineeViewModel>() 
            };
            var dataset = await _exec.Get_DataSetAsync(
                "GetTraineeList",
                new string[] { "@TeamLeadId" },
                new string[] { teamLeadId }
            );
            var traineeList = dataset.Tables[0].AsEnumerable()
                .Select(row => new TraineeViewModel
                {
                    EmployeeID = row["EmployeeID"].ToString(),
                    EmployeeName = row["EmployeeName"].ToString(),
                    Months = row["Months"].ToString().Split(", ").ToList()
                }).ToList();
            response.Model = traineeList;
            return response;
        }

       

    }
}
