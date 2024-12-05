using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class HRDashboardRepository :IHRDashboardRepository
    {
        private readonly ExecuteProcedure _exec;
        public HRDashboardRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);
        }

        public async Task<List<TeamDetailGroupedViewModel>> GetTeamsByHRAsync(int departmentId, string teamAdminId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetTeamDataByHR",
                    new string[] { "@DepartmentID", "@TeamAdminId" },
                    new string[] { departmentId.ToString(), teamAdminId });

                if (obj?.Rows?.Count > 0)
                {
                    var teamsData = await _exec.DataTableToListAsync<TeamsDetailViewModel>(obj);

                    var groupedTeams = teamsData
                        .GroupBy(t => new
                        {
                            t.TeamLeadId,
                            t.TeamLeadName,
                            t.TeamLeadProfileImage
                        })
                        .Select(g => new TeamDetailGroupedViewModel
                        {
                            TeamLeadId = g.Key.TeamLeadId,
                            TeamLeadName = g.Key.TeamLeadName,
                            TeamLeadProfileImage = g.Key.TeamLeadProfileImage,
                            JoiningDate = g.First().JoiningDate,               // Get the JoiningDate from the first employee in the group
                            ExperienceOnJoining = g.First().ExperienceOnJoining, // Get the ExperienceOnJoining from the first employee in the group

                            Employees = g.Select(e => new TeamEmployeeViewModel
                            {
                                EmployeeId = e.EmployeeId,
                                EmployeeName = e.EmployeeName,
                                Designation = e.Designation,
                                ProfileImage = e.ProfileImage,
                                JoiningDate = e.JoiningDate,
                                ExperienceOnJoining = e.ExperienceOnJoining
                            }).ToList()
                        }).ToList();

                    return groupedTeams;
                }
                return null;
            }
            catch (Exception ex)
            {
                // Handle exception (you can log it or return a meaningful error message)
                return null;
            }
        }


    }

}

