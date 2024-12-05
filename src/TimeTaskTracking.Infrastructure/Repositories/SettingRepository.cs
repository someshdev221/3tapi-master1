using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ExecuteProcedure _exec;
        public SettingRepository(IConfiguration configuration) 
        {
            _exec = new ExecuteProcedure(configuration);
        }

        public async Task<int> AddTeamMemberToTeam(TeamAssignmentViewModel<string> teamAssignment)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                new string[] { "@OptId", "@TeamleadId", "@EmployeeId" },
                new string[] { "8", Convert.ToString(teamAssignment.TeamLeaderId), Convert.ToString(teamAssignment.EmployeeId) });

                if (obj?.Rows?.Count > 0)
                {
                    var assignedTeam = await _exec.DataTableToModelAsync<TeamAssignmentViewModel<string>>(obj);
                    return assignedTeam.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> AddTeamMemberInProjectAsync(ProjectAssignmentViewModel<string, int> teamAssignment)
        {
            try
            {
                var result = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@OptId", "@ProjectId", "@EmployeeId" },
                    new string[] { "10", Convert.ToString(teamAssignment.ProjectId), Convert.ToString(teamAssignment.EmployeeId) });

                if (result?.Rows?.Count > 0)
                {
                    var assignedProject = await _exec.DataTableToModelAsync<ProjectAssignmentViewModel<string, int>>(result);
                    return assignedProject.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<UsersByRoleViewModel>> GetUsersByRole(int departmentId, string roleId, string? searchKeyword)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@OptId", "@RoleId", "@DeptID", "@SearchFilter" },
                    new string[] { "9", roleId, Convert.ToString(departmentId), searchKeyword });
                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToListAsync<UsersByRoleViewModel>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ProjectTeamViewModel>> GetProjectMembersByProjectIdAsync(int projectId, int departmentId)
        {
            try
            {
                var obj = _exec.Get_DataTable("UserProjects",
                    new string[] { "@ProjectID", "@OptID", "@DepID" },
                    new string[] { Convert.ToString(projectId), "19", departmentId.ToString() });
                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToListAsync<ProjectTeamViewModel>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DropDownResponse<string>>> GetAssignedUserByName(string employeeId)
        {
            try
            {
                var reportsList = await _exec.Get_DataSetAsync("GetFilteredEmployees",
                    new string[] { "@EmpId", "@OptID" },
                    new string[] { Convert.ToString(employeeId) ,"8" });

                if (reportsList?.Tables?.Count > 0 && reportsList.Tables[0] != null)
                {

                    return await _exec.DataTableToListAsync<DropDownResponse<string>>(reportsList.Tables[0]);
                }

                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<List<EmployeeResponse>> GetEmployeeByDepartmentId(string? teamAdminId, int departmentId)
        {
            try
            {
                var reportsList = await _exec.Get_DataSetAsync("GetFilteredEmployees",
                    new string[] { "@OptID", "@DepartmentID", "@TeamAdminId" },
                    new string[] { "7", Convert.ToString(departmentId),Convert.ToString(teamAdminId) });

                if (reportsList?.Tables?.Count > 0 && reportsList.Tables[0] != null)
                {
                    return await _exec.DataTableToListAsync<EmployeeResponse>(reportsList.Tables[0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> IsProjectAlreadyAssigned(int projectId, string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@ProjectId", "@EmployeeId", "@OptID" },
                    new string[] { Convert.ToString(projectId), Convert.ToString(employeeId) ,"13" });
                if (obj?.Rows?.Count > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> IsTeamAlreadyAssigned(string teamLeadId, string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@TeamleadId", "@EmployeeId", "@OptID" },
                    new string[] { Convert.ToString(teamLeadId), Convert.ToString(employeeId), "14" });
                if (obj?.Rows?.Count > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> CheckUsersDepartmentMatch(string teamLeadId, string employeeId ,string teamAdminId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@TeamleadId", "@EmployeeId", "@OptID", "@TeamAdminId" },
                    new string[] { Convert.ToString(teamLeadId), Convert.ToString(employeeId), "15", Convert.ToString(teamAdminId) });
                if (obj?.Rows?.Count > 0)
                {
                    var status = Convert.ToInt32(obj.Rows[0]["Status"]);
                    return status == 1;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> CheckUsersAndProjectDepartmentMatch(int projectId, string employeeId, string teamAdminId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@ProjectId", "@EmployeeId", "@OptID", "@TeamAdminId" },
                    new string[] { Convert.ToString(projectId), Convert.ToString(employeeId), "16", teamAdminId });
                if (obj?.Rows?.Count > 0)
                {
                    var status = Convert.ToInt32(obj.Rows[0]["Status"]);
                    return status == 1;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<TeamLeadBAAndBDListViewModel> GetTeamLeadBAAndBDListByDepartmentId(string? teamAdminId, int departmentId)
        {
            try
            {
                var reportsList = await _exec.Get_DataSetAsync("GetFilteredEmployees",
                    new string[] { "@OptID", "@DepartmentID", "@TeamAdminId" },
                    new string[] { "9", Convert.ToString(departmentId), Convert.ToString(teamAdminId) });

                if (reportsList?.Tables?.Count > 0 && reportsList.Tables[0] != null)
                {
                    TeamLeadBAAndBDListViewModel teamLeadBAAndBDListViewModel = new();
                    teamLeadBAAndBDListViewModel.TeamLead = await _exec.DataTableToListAsync<DropDownResponse<string>>(reportsList.Tables[0]);
                    teamLeadBAAndBDListViewModel.BDM = await _exec.DataTableToListAsync<DropDownResponse<string>>(reportsList.Tables[1]);
                    return teamLeadBAAndBDListViewModel;
                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }



        public async Task<List<ProductiveHoursViewModel>> GetEmployeeListWhoseProductiveHoursIsHigh(int totalWorkingDays, int departmentId , int month , int year  , int badgeId )
        {

            try
            {
                var obj = await _exec.Get_DataSetAsync("GetUserProductivityByManager",
                    new string[] { "@TotalWorkingDays", "@DepartmentID" , "@Month"  , "@Year"  , "@BadgeId" },
                    new string[] { Convert.ToString(totalWorkingDays), Convert.ToString(departmentId)  , Convert.ToString(month) , Convert.ToString(year) , Convert.ToString(badgeId) });
                if (obj?.Tables?.Count > 0 && obj.Tables[0] != null)
                {

                    return await _exec.DataTableToListAsync<ProductiveHoursViewModel>(obj.Tables[0]);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<bool> RemoveEmployeeFromTeam(string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                new string[] { "@EmployeeId", "@OptID"  },
                new string[] { employeeId, "17" });

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

        public async Task<bool> UpdateCanEditStatusByTeamAdminIdOrDepartmentId(int departmentId, string teamAdminId, bool? canEditStatus)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("UpdateCanEditStatus",
                    new string[] { "@DepartmentId", "@TeamAdminId", "@CanEditStatus" },
                    new string[] { Convert.ToString(departmentId) , Convert.ToString(teamAdminId), Convert.ToString(canEditStatus) });

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
