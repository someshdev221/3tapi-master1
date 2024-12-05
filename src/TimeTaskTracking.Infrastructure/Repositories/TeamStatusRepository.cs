using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class TeamStatusRepository : ITeamStatusRepository
    {
        private readonly ExecuteProcedure _exec;
        public TeamStatusRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);
        }
        public async Task<List<TeamStatusViewModel>> GetTeamStatusByTeamLead(string teamLeadId, DateTime filterByDate)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetTeamStatusByTL",
                    new string[] { "@TLId", "@FilterDate", "@OptId" },
                    new string[] { teamLeadId, Convert.ToString(filterByDate), "2" });
                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToListAsync<TeamStatusViewModel>(obj);
                return null;
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        //public async Task<List<string>> GetEmployeesByTeamLeadIdAsync(string teamLeadId)
        //{
        //    try
        //    {
        //        var obj = await _exec.Get_DataTableAsync("GetTeamAttendanceStatusByTL",
        //            new string[] { "@OptId", "@TeamLeadId" },
        //            new string[] { "4", teamLeadId });

        //        if (obj?.Rows?.Count > 0)
        //        {
        //            return obj.AsEnumerable().Select(row => row["EmployeeId"].ToString()).ToList();
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception
        //        return null;
        //    }
        //}

        public async Task<EmployeeDetails> GetEmployeeAndTeamLeadDetails(TeamStatusDetails<string> teamStatus)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetTeamStatusByTL",
                    new string[] { "@EmpId", "@TLId", "@OptId" },
                    new string[] { teamStatus.EmployeeId, teamStatus.TeamLeadId, "4" });

                if (obj?.Rows?.Count > 0)
                {
                    var employeeDetails = _exec.DataTableToList<EmployeeDetails>(obj).FirstOrDefault();
                    return employeeDetails;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        public async Task<List<TeamStatusViewModel>> UpdateAttendanceStatus(AttendanceDetails attendanceDetails)
        {
            try
            {
            
                var attendanceStatusStr = Enum.GetName(typeof(AttendanceStatus), attendanceDetails.AttendanceStatus);

                if (attendanceStatusStr == null)
                {
                    throw new ArgumentException(SharedResources.InvalidAttendanceStatus);
                }

                var results = new List<TeamStatusViewModel>();

                foreach (var empId in attendanceDetails.EmpId)
                {
                    var obj = await _exec.Get_DataTableAsync("GetTeamAttendanceStatusByTL",
                        new string[] { "@EmpId", "@StatusDate", "@AttendanceStatus", "@OptId" },
                        new string[] { empId, Convert.ToString(attendanceDetails.FilterByDate), attendanceStatusStr, "1" });

                    var resultList = await _exec.DataTableToListAsync<TeamStatusViewModel>(obj);
                    results.AddRange(resultList);
                }

                return results;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<bool> DeleteEmployeeLeaves(string employeeId, DateTime date, string status)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("spMarkUserLeave",
                    new string[] { "@UserId", "@LeaveDate", "@AttendanceStatus", "@OptId" },
                    new string[] { employeeId, date.ToString(), status ?? string.Empty, "3" });

                if (obj?.Rows?.Count > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //public async Task<bool> DeleteEmployeeLeaveAsync(string employeeId, DateTime date, string attendanceStatus)
        //{
        //    try
        //    {
        //        var obj = await _exec.Get_DataTableAsync("GetTeamAttendanceStatusByTL",
        //            new string[] { "@EmployeeId", "@Date", "@AttendanceStatus", "@OptId" },
        //            new string[] { employeeId, date.ToString("yyyy-MM-dd"), attendanceStatus ?? string.Empty, "2" });

        //        if (obj?.Rows?.Count > 0)
        //        {                  
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}


        public async Task<bool> GetAttendanceStatusByEmployeeId(string employeeId, DateTime date)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetTeamAttendanceStatusByTL",
                    new string[] { "@EmployeeId" ,"@Date","@OptId" },
                    new string[] { Convert.ToString(employeeId), Convert.ToString(date), "1" });

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
    }
}
