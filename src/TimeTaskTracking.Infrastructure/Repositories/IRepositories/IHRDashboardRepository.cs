using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface IHRDashboardRepository
    {
        Task<List<TeamDetailGroupedViewModel>> GetTeamsByHRAsync(int departmentId, string teamAdminId);
    }
}
