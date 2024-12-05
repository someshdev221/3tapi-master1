using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface ISuperAdminDashboardService
    {
        Task<ResponseModel<List<DashboardResult>>> GetDashboardResultsAsync();
        Task<ResponseModel<List<SuperAdminOverAllPerformanceViewModel>>> GetSuperAdminOverAllPerformance(int month, int year);
    }
}
