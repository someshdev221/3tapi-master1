using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface ISuperAdminDashboardRepository
    {
        Task<List<DashboardResult>> GetDashboardResultsAsync();
        Task<SuperAdminOverAllPerformanceViewModel> GetSuperAdminOverAllPerformanceAsync(int departmentId, int month, int year);
    }
}
