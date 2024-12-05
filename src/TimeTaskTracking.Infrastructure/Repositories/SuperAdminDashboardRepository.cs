using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Infrastructure.Repositories
{


    public class SuperAdminDashboardRepository : ISuperAdminDashboardRepository
    {
        private readonly ExecuteProcedure _exec;
        public SuperAdminDashboardRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);
        }
        public async Task<List<DashboardResult>> GetDashboardResultsAsync()
        {
            try
            {
                // Execute stored procedure
                var reportsDataSet = await _exec.Get_DataSetAsync("SuperAdminDashboard", Array.Empty<string>(), Array.Empty<string>());

                if (reportsDataSet != null && reportsDataSet.Tables.Count >= 4)
                {
                    var dashboardResults = new List<DashboardResult>();

                    // Process Work In Hand from Table 0
                    var workInHandTable = reportsDataSet.Tables[0];
                    var workInHandData = workInHandTable.AsEnumerable()
                        .GroupBy(row => new { DepartmentId = row.Field<int>("DepartmentId"), DepartmentName = row.Field<string>("DepartmentName") })
                        .ToDictionary(
                            group => group.Key,
                            group => group.First().Field<decimal>("WorkInHand")
                        );

                    // Process Pending Payment from Table 1
                    var pendingPaymentTable = reportsDataSet.Tables[1];
                    var pendingPaymentData = pendingPaymentTable.AsEnumerable()
                        .GroupBy(row => new { DepartmentId = row.Field<int>("DepartmentId"), DepartmentName = row.Field<string>("DepartmentName") })
                        .ToDictionary(
                            group => group.Key,
                            group => group.First().Field<decimal>("PendingPayment")
                        );

                    // Process Total Employee Count from Table 2
                    var totalEmployeeCountTable = reportsDataSet.Tables[2];
                    var totalEmployeeCountData = totalEmployeeCountTable.AsEnumerable()
                        .GroupBy(row => new { DepartmentId = row.Field<int>("DepartmentId"), DepartmentName = row.Field<string>("DepartmentName") })
                        .ToDictionary(
                            group => group.Key,
                            group => (int)group.First().Field<long>("TotalEmployeeCount")
                        );

                    // Process Employee Details from Table 3
                    var employeeDetailsTable = reportsDataSet.Tables[3];
                    var employeeDetailsData = employeeDetailsTable.AsEnumerable()
                        .GroupBy(row => row.Field<int>("DepartmentId"))
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(row => new EmployeeDetail
                            {
                                Id = row.Field<Guid>("Id"),
                                JoiningDate = row.IsNull("JoiningDate") ? (DateTime?)null : row.Field<DateTime?>("JoiningDate"),
                                ExperienceOnJoining = row.IsNull("ExperienceOnJoining") ? (int?)null : row.Field<int>("ExperienceOnJoining"),
                                Designation = row.Field<string>("Designation")
                            }).ToList()
                        );
                    // Combine data
                    foreach (var key in workInHandData.Keys)
                    {
                        var departmentId = key.DepartmentId;
                        var departmentName = key.DepartmentName;

                        var dashboardResult = new DashboardResult
                        {
                            DepartmentId = departmentId,
                            DepartmentName = departmentName,
                            WorkInHand = workInHandData[key],
                            PendingPayment = pendingPaymentData.ContainsKey(key) ? pendingPaymentData[key] : 0,
                            TotalEmployeeCount = totalEmployeeCountData.ContainsKey(key) ? totalEmployeeCountData[key] : 0,
                            EmployeeDetails = employeeDetailsData.ContainsKey(departmentId) ? employeeDetailsData[departmentId] : new List<EmployeeDetail>()
                        };
                        dashboardResults.Add(dashboardResult);
                    }
                    return dashboardResults;
                }
                return new List<DashboardResult>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<SuperAdminOverAllPerformanceViewModel> GetSuperAdminOverAllPerformanceAsync(int departmentId, int month, int year)
        {
            try
            {
                var dataset = await _exec.Get_DataTableAsync("SuperAdminOverAllPerformance",
                    new string[] { "@DepartmentID", "@Month", "@Year" },
                    new string[] { Convert.ToString(departmentId), Convert.ToString(month), Convert.ToString(year) });

                if (dataset == null)
                {
                    return null;
                }
                var result = await _exec.DataTableToModelAsync<SuperAdminOverAllPerformanceViewModel>(dataset);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}

