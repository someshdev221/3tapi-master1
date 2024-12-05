using Microsoft.Extensions.Configuration;
using System.Data;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class ReportsRepository : IReportsRepository
{
    private readonly ExecuteProcedure _exec;

    public ReportsRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }

    public async Task<PaginationResponseViewModel<List<ProjectBillingReportViewModel>>> GetProjectReport(ProjectReportRequestModel requestModel)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetActiveProjectDetails",
            new string[] { "@PageSize", "@PageNumber", "@CreatedDate", "@HoursFrom", "@HoursTo", "@DepartmentId", "@SearchValue", "@TeamAdminId", "@SortColumn", "@SortOrder" },
            new string[] {
            requestModel.PageSize == null ? null : Convert.ToString(requestModel.PageSize),
            requestModel.PageNumber == null ? null : Convert.ToString(requestModel.PageNumber),
            requestModel.StartDate == null ? null : Convert.ToString(requestModel.StartDate),
            requestModel.HoursFrom == null ? null : Convert.ToString(requestModel.HoursFrom),
            requestModel.HoursTo == null ? null : Convert.ToString(requestModel.HoursTo),
            Convert.ToString(requestModel.DepartmentId),
            requestModel.SearchValue,
            requestModel.TeamAdminId == null ? null : Convert.ToString(requestModel.TeamAdminId),
            requestModel.SortColumn,
            requestModel.SortOrder
            });

            if (reportsList != null && reportsList.Tables?.Count > 0)
            {
                var projectReports = await _exec.DataTableToListAsync<ProjectBillingReportViewModel>(reportsList.Tables[0]);

                PaginationResponseViewModel<List<ProjectBillingReportViewModel>> paginationResponseViewModel = new();
                paginationResponseViewModel.results = projectReports;

                // Safely check if there are any rows before accessing Rows[0][8]
                if (reportsList.Tables[0].Rows.Count > 0)
                {
                    paginationResponseViewModel.TotalCount = Convert.ToInt32(reportsList.Tables[0].Rows[0][8]);
                }
                else
                {
                    paginationResponseViewModel.TotalCount = 0; // If no rows are present, set TotalCount to 0
                }

                return paginationResponseViewModel;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    public async Task<List<MonthlyHoursViewModel>> MonthlyHoursReport(ReportsViewModel model)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetMonthlyHours",
            new string[] { "@From_Date", "@To_Date", "@EmpID", "@ClientId", "@ProjectId" },
                new string[] { Convert.ToString(model.FromDate), Convert.ToString(model.ToDate), model.EmployeeId ?? string.Empty, Convert.ToString(model.ClientId), Convert.ToString(model.ProjectId) });

            if (reportsList?.Tables?.Count > 0)
            {
                return await _exec.DataTableToListAsync<MonthlyHoursViewModel>(reportsList.Tables[0]);

            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<ReportsResponseViewModel>> ReportsDetailByTeamLeader(ReportsViewModel model)
    {
        try
        {
            var reportsList = await _exec.Get_DataTableAsync("GetReports",
                new string[] { "@From_Date", "@To_Date", "@EmpID", "@ClientId", "@ProjectId", "@PageNumber", "@PageSize" },
                new string[] { Convert.ToString(model.FromDate), Convert.ToString(model.ToDate), model.EmployeeId ?? string.Empty, Convert.ToString(model.ClientId), Convert.ToString(model.ProjectId), Convert.ToString(0) ?? "0", Convert.ToString(0) ?? "0" });

            if (reportsList != null && reportsList.Rows.Count > 0)
            {
                return await _exec.DataTableToListAsync<ReportsResponseViewModel>(reportsList);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<PaginationResponseViewModel<List<EmployeeReportViewModel>>> GetActiveEmployeeReport(EmployeeReportRequestModel requestModel)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetActiveEmployeeBillingDetails",
                new string[] { "@PageSize", "@PageNumber", "@FromDate", "@ToDate", "@DepartmentId", "@SearchValue", "@TeamAdminId", "@SortColumn", "@SortOrder" },
                new string[] { Convert.ToString(requestModel.PageSize) ?? string.Empty,
                Convert.ToString(requestModel.PageNumber) ?? string.Empty,
                Convert.ToString(requestModel.From), Convert.ToString(requestModel.To),
                Convert.ToString(requestModel.DepartmentId), requestModel.SearchValue ?? string.Empty,
                requestModel.TeamAdminId == null ? null: Convert.ToString(requestModel.TeamAdminId),
                   requestModel.SortColumn,
                  requestModel.SortOrder
                     });
            if (reportsList == null || reportsList?.Tables.Count == 0)
            {
                return null;
            }

            PaginationResponseViewModel<List<EmployeeReportViewModel>> paginationResponseViewModel = new();
            paginationResponseViewModel.results = await _exec.DataTableToListAsync<EmployeeReportViewModel>(reportsList.Tables[0]);
            paginationResponseViewModel.TotalCount = Convert.ToInt32(reportsList.Tables[1].Rows[0][0]);
            return paginationResponseViewModel;

        }
        catch (Exception ex)
        {
            return null;
        }
    }


    public async Task<List<ProjectReportViewModel>> GetEmployeeProjectReport(EmployeeProjectRequestModel requestModel)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetProjectReportByEmployee",
            new string[] { "@EmployeeID", "@FromDate", "@ToDate" },
                new string[] { Convert.ToString(requestModel.EmployeeId), Convert.ToString(requestModel.From), Convert.ToString(requestModel.To) });

            if (reportsList?.Tables?.Count > 0)
            {
                var dataTable = reportsList.Tables[0];
                var projectReports = dataTable.AsEnumerable()
                    .GroupBy(row => new
                    {
                        ProjectName = row.Field<string>("ProjectName"),
                        ClientName = row.Field<string>("ClientName")
                    })
                    .Select(group => new ProjectReportViewModel
                    {
                        ProjectName = group.Key.ProjectName,
                        ClientName = group.Key.ClientName,
                        ModuleList = group.Select(row => new ProjectModuleViewModel
                        {
                            ModuleName = row.Field<string>("ModuleName"),
                            ModuleStatus = row.Field<string>("ModuleStatus"),
                            FixedHours = row.Field<decimal?>("FixedHours"),
                            UpworkHours = row.Field<decimal?>("UpworkHours"),
                            OfflineHours = row.Field<decimal?>("OfflineHours"),
                            BillingHours = row.Field<decimal?>("BillingHours")
                        }).ToList()
                    }).ToList();

                return projectReports;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<PaginationResponseViewModel<List<EmployeeAttendanceReportViewModel>>> GetEmployeesAttendanceReport(EmployeeAttendanceRequestViewModel requestViewModel)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetHRReports",
                    new string[] { "@Month", "@Year", "@PageNumber", "@PageSize", "@SearchValue", "@DepartmentId", "@TeamAdminId", "@OptId" },
                    new string[] { Convert.ToString(requestViewModel.Month), Convert.ToString(requestViewModel.Year),
                    Convert.ToString(requestViewModel.PageNumber) ?? "0",
                    Convert.ToString(requestViewModel.PageSize) ?? "0",
                    Convert.ToString(requestViewModel.SearchValue) ?? string.Empty,
                    requestViewModel.departmentId == null ? null:Convert.ToString(requestViewModel.departmentId),
                    Convert.ToString(requestViewModel.TeamAdminId)?? string.Empty,
                    "1" });

            if (reportsList?.Tables?.Count > 0)
            {
                PaginationResponseViewModel<List<EmployeeAttendanceReportViewModel>> paginationResponseViewModel = new();
                var dataTable = reportsList.Tables[0];
                var projectReports = dataTable.AsEnumerable()
                    .GroupBy(row => new
                    {
                        EmployeeId = row.Field<string>("EmployeeId"),
                        EmployeeName = row.Field<string>("EmployeeName"),
                        EmployeeNumber = row.Field<string>("EmployeeNumber")
                    })
                    .Select(group => new EmployeeAttendanceReportViewModel
                    {
                        EmployeeId = group.Key.EmployeeId,
                        EmployeeName = group.Key.EmployeeName,
                        EmployeeNumber = group.Key.EmployeeNumber,
                        attendanceReports = group.Select(row => new AttendanceReportViewModel
                        {
                            AttendanceStatus = row.Field<string>("AttendanceStatus"),
                            DayHours = row.Field<decimal>("DayHours"),
                            Day = row.Field<int>("Day")
                        }).ToList()
                    }).OrderBy(x=>x.EmployeeNumber).ToList();

                paginationResponseViewModel.results = projectReports;
                paginationResponseViewModel.TotalCount = Convert.ToInt32(reportsList.Tables[1].Rows[0][0]);
                return paginationResponseViewModel;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<PaginationResponseViewModel<List<EmployeeMonthlyLeaveReportsViewModel>>> GetEmployeesMonthlyLeaveReportByHR(EmployeesMonthlyLeaveRequestViewModel requestViewModel)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetHRReports",
             new string[] { "@Month", "@Year", "@PageNumber", "@PageSize", "@SearchValue", "@DepartmentId", "@OptId", "@TeamAdminId" },
                 new string[] { Convert.ToString(requestViewModel.Month), Convert.ToString(requestViewModel.Year),
                    Convert.ToString(requestViewModel.PageNumber),
                    Convert.ToString(requestViewModel.PageSize),
                    Convert.ToString(requestViewModel.SearchValue) ?? string.Empty,
                    requestViewModel.departmentId ==null?null:Convert.ToString(requestViewModel.departmentId),
                    "2", requestViewModel.TeamAdminId  });


            if (reportsList?.Tables?.Count > 0)
            {
                PaginationResponseViewModel<List<EmployeeMonthlyLeaveReportsViewModel>> paginationResponseViewModel = new()
                {
                    results = await _exec.DataTableToListAsync<EmployeeMonthlyLeaveReportsViewModel>(reportsList.Tables[0]),
                    TotalCount = Convert.ToInt32(reportsList.Tables[1].Rows[0][0])
                };
                return paginationResponseViewModel;
            }

            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<TeamReportResponseModel>> GetTeamReportByDepartment(TeamReportRequestModel requestModel)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetAllTeamReportsByDepartment",
             new string[] { "@DepartmentId", "@FromDate", "@ToDate", "@SearchValue", "@TeamAdminId" },
                 new string[] { Convert.ToString(requestModel.DepartmentId),
                 Convert.ToString(requestModel.From), Convert.ToString(requestModel.To), requestModel.SearchValue,
                 requestModel.TeamAdminId == null ? null:Convert.ToString(requestModel.TeamAdminId) });

            if (reportsList?.Tables?.Count > 0)
            {
                var dataTable = reportsList.Tables[0];
                var teamReport = dataTable.AsEnumerable()
                    .GroupBy(row => new
                    {
                        TeamLeaderName = row.Field<string>("TeamLeaderName"),
                    })
                    .Select(group => new TeamReportResponseModel
                    {
                        TeamLeaderName = group.Key.TeamLeaderName,
                        EmployeeDetails = group.Select(row => new EmployeeBillingDetails
                        {
                            Id = row.Field<string>("EmployeeId"),
                            Name = row.Field<string>("Name"),
                            FixedHours = row.Field<decimal?>("FixedHours"),
                            UpworkHours = row.Field<decimal?>("UpworkHours"),
                            OfflineHours = row.Field<decimal?>("OfflineHours"),
                            BillingHours = row.Field<decimal?>("BillingHours")
                        }).ToList()
                    }).ToList();

                return teamReport;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<PaginationResponseViewModel<List<EmployeeReportResponseModel>>> GetEmployeesReportByDepartmentManager(EmployeeReportRequestViewModel requestModel)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetActiveEmployeeDetailsWithProjects",
                new string[] { "@DepartmentId", "@PageNumber", "@PageSize", "@SearchValue", "@TeamAdminId", "@SortColumn", "@SortOrder" },
            new string[] { Convert.ToString(requestModel.DepartmentId),
            requestModel.PageNumber == null ? null : Convert.ToString(requestModel.PageNumber),
            requestModel.PageNumber == null ? null : Convert.ToString(requestModel.PageSize), requestModel.SearchValue,
            requestModel.TeamAdminId==null?null :Convert.ToString(requestModel.TeamAdminId),
             requestModel.SortColumn,
             requestModel.SortOrder
               });

            if (reportsList?.Tables?.Count > 0)
            {
                var teamReportData = await _exec.DataTableToListAsync<EmployeeReportResponseModel>(reportsList.Tables[1]);
                PaginationResponseViewModel<List<EmployeeReportResponseModel>> paginationResponseViewModel = new();
                paginationResponseViewModel.results = teamReportData;
                paginationResponseViewModel.TotalCount = Convert.ToInt32(reportsList.Tables[0].Rows[0][0]);
                return paginationResponseViewModel;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> CheckTeamAdminAndHodDepartment(int DepartmentId, string TeamAdminId)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("CheckTeamAdminForDepartmentExists",
                new string[] { "@DepartmentId", "@TeamAdminId" },
            new string[] { Convert.ToString(DepartmentId), TeamAdminId.ToString() });

            if (reportsList?.Tables?.Count > 0)
            {
                var response = Convert.ToBoolean(reportsList.Tables[0].Rows[0][0]);
                return response;
            }
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<List<ProjectPaymentStatus>> GetPaymentStatusReportAsync(string teamAdminId, int departmentId, string searchText)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetPaymentStatusReport",
                new string[] { "@TeamAdminId", "@DepartmentId", "@SearchText" },
                new string[] { teamAdminId == null ? null : Convert.ToString(teamAdminId), departmentId.ToString(), searchText ?? string.Empty });

            if (reportsList?.Tables?.Count > 0)
            {
                var dataTable = reportsList.Tables[0];
                var paymentStatusList = dataTable.AsEnumerable()
                    .GroupBy(row => new
                    {
                        ProjectId = row.Field<Int32>("ProjectId"),
                        ProjectName = row.Field<string>("ProjectName")
                    })
                    .Select(group => new ProjectPaymentStatus
                    {
                        ProjectId = Convert.ToInt32(group.Key.ProjectId),
                        ProjectName = group.Key.ProjectName,
                        ModulesList = group.Select(row => new ProjectModuleReport
                        {
                            ModuleId = row.Field<string>("ModuleId"),
                            ModuleName = row.Field<string>("ModuleName"),
                            BillingType = row.Field<int?>("BillingType"),
                            HiringStatus = row.Field<int?>("HiringStatus"),
                            DeadlineDate = row.Field<DateTime?>("DeadlineDate"),
                            ApprovedHours = row.Field<int?>("ApprovedHours"),
                            BillingHours = row.Field<decimal?>("BillingHours"),
                            LeftHours = row.Field<decimal?>("LeftHours"),
                            ModuleStatus = row.Field<string?>("ModuleStatus"),
                            PaymentStatus = row.Field<string?>("PaymentStatus")
                        }).ToList()
                    }).ToList();

                return paymentStatusList;
            }

            return new List<ProjectPaymentStatus>();
        }
        catch (Exception ex)
        {
            return null; ;
        }
    }

    public async Task<List<ClientsReportViewModel>> GetClientsBillingReport(ClientReportRequestViewModel requestModel)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetClientBillingHours",
                new string[] { "@FromDate", "@ToDate", "@DepartmentId", "@TeamAdminId" },
                new string[] { Convert.ToString(requestModel.FromDate),
                               Convert.ToString(requestModel.ToDate),
                               Convert.ToString(requestModel.DepartmentId) ?? string.Empty,
                               Convert.ToString(requestModel.TeamAdminId) ?? string.Empty});

            if (reportsList?.Tables?.Count > 0)
            {
                var clientsReport = await _exec.DataTableToListAsync<ClientsReportViewModel>(reportsList.Tables[0]);
                return clientsReport;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<WorkInHandReportViewModel>> GetWorkInHandReportAsync(string? teamAdminId, int departmentId, string? searchText)
    {
        try
        {
            var reportsList = await _exec.Get_DataTableAsync("WorkInHandReport",
                new string[] { "@DepartmentId", "@TeamAdminId", "@SearchText" },
                new string[] { Convert.ToString(departmentId),
                               Convert.ToString(teamAdminId),
                               Convert.ToString(searchText)
                               });
            if (reportsList != null && reportsList.Rows?.Count > 0)
                return await _exec.DataTableToListAsync<WorkInHandReportViewModel>(reportsList);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<GroupFullReportViewModel>> GetFullReportAsync(FullReportRequestViewModel model)
    {
        try
        {
            var reportsList = await _exec.Get_DataTableAsync("GetFullReportByManager",
                new string[] { "@OptId", "@TeamAdminId", "@FromDate", "@ToDate", "@EmployeeId", "@ProjectId", "@ClientId", "@DepartmentId" },
                new string[] { "1", model.TeamAdminId ?? string.Empty, Convert.ToString(model.FromDate), Convert.ToString(model.ToDate),
                           Convert.ToString(model.EmployeeId ?? string.Empty), Convert.ToString(model.ProjectId) ?? "0", Convert.ToString(model.ClientId) ?? "0", Convert.ToString(model.DepartmentId) });

            if (reportsList == null || reportsList.Rows.Count == 0)
            {
                return null;
            }

            var groupedData = reportsList.AsEnumerable()
                    .GroupBy(row => new
                    {
                        Date = row.Field<DateTime>("Date")
                    })
                    .Select(g => new GroupFullReportViewModel
                    {
                        Date = g.Key.Date,
                        ReportViewModel = g.Select(row => new FullReportViewModel
                        {
                            EmployeeId = row.Field<string>("EmployeeId"),
                            Employee = row.Field<string>("Employee"),
                            ProjectId = row.Field<int>("ProjectId"),
                            ProjectName = row.Field<string>("ProjectName"),
                            ModuleId = row.Field<Guid>("ModuleId"),
                            Module = row.Field<string>("Module"),
                            ClientId = row.Field<int>("ClientId"),
                            Client = row.Field<string>("Client"),
                            ProfileId = row.Field<int>("ProfileId"),
                            Profile = row.Field<string>("Profile"),
                            Memo = row.Field<string>("memo"),
                            UpworkHours = row.Field<decimal>("UpworkHours"),
                            FixedHours = row.Field<decimal>("FixedHours"),
                            NonBillableHours = row.Field<decimal>("NonBillableHours"),
                        }).ToList()
                    }).ToList();

            return groupedData;
        }
        catch (Exception ex)
        {
            // Handle exception (log it, rethrow it, etc.)
            return null;
        }
    }

    public async Task<List<DropDownResponse<string>>> GetEmployeeListForFullReportAsync(string teamAdminId, int departmentId)
    {
        try
        {
            var reportsList = await _exec.Get_DataTableAsync("GetFullReportByManager",
                new string[] { "@OptId", "@DepartmentId", "@TeamAdminId" },
                new string[] { "2", Convert.ToString(departmentId),
                               Convert.ToString(teamAdminId ?? string.Empty),});

            if (reportsList != null && reportsList.Rows?.Count > 0)
            {
                var employeeList = await _exec.DataTableToListAsync<DropDownResponse<string>>(reportsList);
                return employeeList;
            }

            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<DropDownResponse<int>>> GetProjectListForFullReportAsync(string teamAdminId, int departmentId)
    {
        try
        {
            var projectList = await _exec.Get_DataTableAsync("GetFullReportByManager",
                new string[] { "@OptId", "@DepartmentId", "@TeamAdminId" },
                new string[] { "3", Convert.ToString(departmentId),
                               Convert.ToString(teamAdminId ?? string.Empty),});

            if (projectList != null && projectList.Rows?.Count > 0)
            {
                var employeeList = await _exec.DataTableToListAsync<DropDownResponse<int>>(projectList);
                return employeeList;
            }

            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<DropDownResponse<int>>> GetClientListForFullReportAsync(string teamAdminId, int departmentId)
    {
        try
        {
            var projectList = await _exec.Get_DataTableAsync("GetFullReportByManager",
                new string[] { "@OptId", "@DepartmentId", "@TeamAdminId" },
                new string[] { "4", Convert.ToString(departmentId),
                               Convert.ToString(teamAdminId ?? string.Empty),});
            if (projectList != null && projectList.Rows?.Count > 0)
            {
                var employeeList = await _exec.DataTableToListAsync<DropDownResponse<int>>(projectList);
                return employeeList;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<MonthlyHourFullReportViewModel>> GetMonthlyHoursListForFullReportAsync(MonthlyHourFullReportRequestViewModel model)
    {
        try
        {
            var result = await _exec.Get_DataTableAsync("GetFullReportByManager",
                new string[] { "@OptId", "@EmployeeId", "@FromDate", "@ToDate", "@ProjectId", "@ClientId" },
                new string[] { "5", Convert.ToString(model.EmployeeId), Convert.ToString(model.FromDate), Convert.ToString(model.ToDate), Convert.ToString(model.ProjectId), Convert.ToString(model.ClientId)});

            if (result != null && result.Rows?.Count > 0)
            {
                var monthlyHourReportList = await _exec.DataTableToListAsync<MonthlyHourFullReportViewModel>(result);
                return monthlyHourReportList;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<PaginationResponseViewModel<List<ProfileReportViewModel>>> GetProfileReportAsync(ProfileReportRequestViewModel model)
    {
        try
        {
            var fromDate = model.FromDate.HasValue ? Convert.ToString(model.FromDate) : null;
            var toDate = model.ToDate.HasValue ? Convert.ToString(model.ToDate) : null;
            var pageSize = model.PageSize.HasValue ? Convert.ToString(model.PageSize) : null;
            var pageNumber = model.PageNumber.HasValue ? Convert.ToString(model.PageNumber) : null;

            var result = await _exec.Get_DataSetAsync("GetProfileReport",
                new string[] { "@FromDate", "@ToDate", "@PageSize", "@PageNumber" },
                new string[] { fromDate, toDate, pageSize, pageNumber });

      
            var paginationResponseViewModel = new PaginationResponseViewModel<List<ProfileReportViewModel>>();     
            if (result != null && result.Tables.Count > 0)
            {           
                paginationResponseViewModel.results = await _exec.DataTableToListAsync<ProfileReportViewModel>(result.Tables[0]);
               
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    paginationResponseViewModel.TotalCount = Convert.ToInt32(result.Tables[1].Rows[0][0]);
                }
                else
                {
                    paginationResponseViewModel.TotalCount = 0; 
                }
            }
            else
            {
                paginationResponseViewModel.results = new List<ProfileReportViewModel>(); 
                paginationResponseViewModel.TotalCount = 0;
            }

            return paginationResponseViewModel;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

}
