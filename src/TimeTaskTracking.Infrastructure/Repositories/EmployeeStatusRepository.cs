using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class EmployeeStatusRepository : IEmployeeStatusRepository
    {
        private readonly ExecuteProcedure _exec;

        public EmployeeStatusRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);
        }

        public async Task<EmployeeLeavesStatus> AddEmployeeLeave(string userId, bool? isPresent, DateTime leaveDate, string attendanceStatus, string teamLeadId, string teamAdminId, string submittedBy)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("spMarkUserLeave",
                    new string[] { "@OptId", "@UserId", "@IsPresent", "@LeaveDate", "@AttendanceStatus", "@TeamLeadId", "@TeamAdminId", "@SubmittedBy" },
                    new string[] { "1", Convert.ToString(userId), Convert.ToString(isPresent), Convert.ToString(leaveDate), Convert.ToString(attendanceStatus), teamLeadId, teamAdminId, submittedBy });

                if (obj != null && obj.Rows.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<EmployeeLeavesStatus>(obj);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<EmployeeStatus> AddNewEmployeeStatus(EmployeeStatus employeeStatus, string teamLeadId, string teamAdminId)
        {
            try
            {
                //foreach (var employeeStatus in employeeStatusList)
                //{
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployeeStatus",
                             new string[]
                        {
                            "@OptID", "@ProjectID", "@ApplicationUsersId", "@Date", "@ProfileName",
                            "@UpworkHours", "@FixedHours", "@OfflineHours", "@Memo",
                            "@IsSVNUpdated", "@UpdatedClient", "@ModuleName" , "@ModuleId","@ProfileId", "@teamLeadId", "@teamAdminId"
                        },
                        new string[]
                        {
                            "2",
                            Convert.ToString(employeeStatus.ProjectID) ,
                            Convert.ToString(employeeStatus.ApplicationUsersId) ,
                            Convert.ToString(employeeStatus.Date) ,
                              ( employeeStatus.ProfileName ),
                            Convert.ToString(employeeStatus.UpworkHours) ,
                            Convert.ToString(employeeStatus.FixedHours) ,
                            Convert.ToString(employeeStatus.OfflineHours) ,
                                employeeStatus.Memo ,
                            Convert.ToString(employeeStatus.IsSVNUpdated),
                            Convert.ToString(employeeStatus.UpdatedClient) ,
                            employeeStatus.ModuleName ,
                            Convert.ToString(employeeStatus.ModuleId),
                            Convert.ToString(employeeStatus.ProfileId),
                            teamLeadId,
                            teamAdminId


                 });

                if (obj?.Rows?.Count > 0)
                {
                    var employeeStatusModel = await _exec.DataTableToModelAsync<EmployeeStatus>(obj);
                    employeeStatusModel.Id = Convert.ToInt32(obj.Rows[0]["Id"]);

                    return employeeStatusModel;
                }
                else
                {
                    return null;
                }
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteEmployeeStatus(EmployeeStatusFilterViewModel getAndDeleteEmployeeStatusViewModel)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployeeStatus",
                    new string[] { "@Date", "@ApplicationUsersId", "@OptID" },
                    new string[] { Convert.ToString(getAndDeleteEmployeeStatusViewModel.ToDate), Convert.ToString(getAndDeleteEmployeeStatusViewModel.UserProfileId), "1" });

                if (obj?.Rows?.Count > 0 && obj != null)
                {
                    var status = Convert.ToInt32(obj.Rows[0]["IsDeleted"]);
                    var data = status == 1;
                    return data;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmloyeeStatusForTheDate(string employeeId, DateTime date)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetTeamAttendanceStatusByTL",
                    new string[] { "@Date", "@EmployeeId", "@OptID" },
                    new string[] { Convert.ToString(date), employeeId, "2" });

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> DeleteEmployeeStatusByID(int id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployeeStatus",
                    new string[] { "@Id", "@OptID" },
                    new string[] { Convert.ToString(id), "4" });

                if (obj.Rows.Count > 0)
                {
                    var isDeleted = Convert.ToBoolean(obj.Rows[0]["IsDeleted"]);
                    var returnedId = Convert.ToInt32(obj.Rows[0]["Id"]);

                    if (isDeleted && returnedId != 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<PaginationResponseViewModel<List<EmployeeStatus>>> GetEmployeeStatusList(EmployeeStatusPaginationViewModel employeeStatusFilterViewModel)
        {
            try
            {
                var statusList = await _exec.Get_DataSetAsync("GetEmployeeStatus",
                    new string[] { "@OptID", "@From_Date", "@To_Date", "@ApplicationUsersId", "@PageNumber", "@PageSize" },
                    new string[] { "1", Convert.ToString(employeeStatusFilterViewModel.FromDate), Convert.ToString(employeeStatusFilterViewModel.ToDate), employeeStatusFilterViewModel.UserProfileId, Convert.ToString(employeeStatusFilterViewModel.PageNumber) ?? "0", Convert.ToString(employeeStatusFilterViewModel.PageSize) ?? "0" });
                if (statusList?.Tables?.Count == 0 || statusList == null)
                {
                    return null;
                }
                PaginationResponseViewModel<List<EmployeeStatus>> paginationResponseViewModel = new();
                paginationResponseViewModel.results = await _exec.DataTableToListAsync<EmployeeStatus>(statusList.Tables[0]);
                paginationResponseViewModel.TotalCount = Convert.ToInt32(statusList.Tables[1].Rows[0][0]);
                return paginationResponseViewModel;


            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<EmployeeStatus>> GetEmpStatusByDate(string date, string userProfileID)
        {
            try
            {
                var statusList = await _exec.Get_DataTableAsync("GetEmployeeStatus",
                    new string[] { "@OptID", "@From_Date", "@ApplicationUsersId" },
                    new string[] { "2", Convert.ToString(date), Convert.ToString(userProfileID) });
                if (statusList.Rows.Count > 0)
                {
                    return await _exec.DataTableToListAsync<EmployeeStatus>(statusList);
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<EstimatedHour> GetProjectModuleByIdAsync(string moduleId, int id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployeeStatus",
                    new string[] { "@ModuleId", "@OptId", "@Id" },
                    new string[] { Convert.ToString(moduleId), "5", Convert.ToString(id) });

                if (obj != null && obj.Rows.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<EstimatedHour>(obj);
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<EmployeeStatus> UpdateEmployeeStatus(EmployeeStatus employeeStatus)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployeeStatus",
                    new string[] { "@OptID", "@ProjectID", "@Date", "@ProfileName", "@ApplicationUsersId", "@UpworkHours", "@FixedHours",
                        "@OfflineHours", "@Memo", "@IsSVNUpdated", "@UpdatedClient", "@ModuleName", "@Id", "@ModuleId","@ProfileId"  },
                    new string[] { "3", Convert.ToString(employeeStatus.ProjectID),
                        Convert.ToString(employeeStatus.Date), employeeStatus.ProfileName,Convert.ToString (employeeStatus.ApplicationUsersId), Convert.ToString(employeeStatus.UpworkHours),
                        Convert.ToString(employeeStatus.FixedHours), Convert.ToString(employeeStatus.OfflineHours),
                        employeeStatus.Memo, Convert.ToString(employeeStatus.IsSVNUpdated),
                        Convert.ToString(employeeStatus.UpdatedClient), employeeStatus.ModuleName, Convert.ToString(employeeStatus.Id), Convert.ToString(employeeStatus.ModuleId),
                        Convert.ToString(employeeStatus.ProfileId) });
                if (obj?.Rows?.Count > 0)
                {
                    // Check if the 'IsUpdated' column exists
                    if (obj.Columns.Contains("IsUpdated"))
                    {
                        bool isUpdated = Convert.ToBoolean(obj.Rows[0]["IsUpdated"]);
                        if (isUpdated)
                        {
                            // Convert the DataTable to the EmployeeStatus model
                            var employeeStatusModel = await _exec.DataTableToModelAsync<EmployeeStatus>(obj);
                            return employeeStatus;
                        }
                        else if (obj.Columns.Contains("ErrorMessage"))
                        {
                            string errorMessage = obj.Rows[0]["ErrorMessage"].ToString();
                            return null;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<EmployeeLeavesStatus>> CheckIfUserAlreadyAppliedLeave(string employeeId, DateTime date)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("spMarkUserLeave",
                    new string[] { "@UserId", "@LeaveDate", "@OptId" },
                    new string[] { Convert.ToString(employeeId), date.ToString(), "2" });

                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToListAsync<EmployeeLeavesStatus>(obj);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<EmployeeLeavesStatus> CheckForExistingLeave(string employeeId, DateTime date)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("spMarkUserLeave",
                    new string[] { "@UserId", "@LeaveDate", "@OptId" },
                    new string[] { Convert.ToString(employeeId), date.ToString(), "2" });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<EmployeeLeavesStatus>(obj);
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<EmployeeStatus> GetEmployeeStatusById(int id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployeeStatus",
                    new string[] { "@Id", "@OptId" },
                    new string[] { Convert.ToString(id), "6" });

                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToModelAsync<EmployeeStatus>(obj);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<EmployeeStatusViewModel>> GetEmployeeStatusListByManager(int Month, int Year, int departmentId, string EmployeeId)
        {
            try
            {
                var statusList = await _exec.Get_DataTableAsync("GetEmployeeStatusByManagers",
                    new string[] { "@Month", "Year", "@DepartmentId", "@ApplicationUsersId" },
                    new string[] { Convert.ToString(Month) , Convert.ToString(Year), Convert.ToString(departmentId) , Convert.ToString(EmployeeId)
                   });
                if (statusList?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToListAsync<EmployeeStatusViewModel>(statusList);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DropDownResponse<string>>> GetOpenProjectModulesListByProjectIdAsync(int projectId, int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("UserProjects",
                    new string[] { "@ProjectId", "@DepID", "@OptID" },
                    new string[] { projectId.ToString(), Convert.ToString(departmentId), "34" });

                if (obj?.Tables?.Count > 0)
                {
                    return await _exec.DataTableToListAsync<DropDownResponse<string>>(obj.Tables[0]);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateEmployeeLeaveForTheEmployee(string employeeId, DateTime date, string leaveStatus, int existingLeaveId, string submittedBy)
        {
            try
            {
                var updateLeave = await _exec.Get_DataTableAsync("GetTeamAttendanceStatusByTL",
                    new string[] { "@OptId", "@EmployeeId", "@Date", "IsPresent", "@AttendanceStatus", "@LeaveId", "@SubmittedBy" },
                    new string[] { "3", employeeId, Convert.ToString(date), "0", leaveStatus, Convert.ToString(existingLeaveId), submittedBy });

                if (updateLeave?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> GetEmployeeStatusByModuleId(string Id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployeeStatus",
                    new string[] { "@OptId", "@ModuleId" },
                    new string[] { "7", Id });

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> GetEmployeeStatusByUpworkProfileId(int id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteProfiles",
                    new string[] { "@OptId", "@Id" },
                   new string[] { "4", id.ToString() });
                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<EmployeeStatus> GetEmployeeStatusByIdAsync(int id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("[GetEmployeeStatus]",
                    new string[] { "@OptId", "@Id",  },
                    new string[] { "3" , Convert.ToString(id)
                   });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<EmployeeStatus>(obj);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
