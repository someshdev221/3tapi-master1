using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface IHRDashboardService
    {
        Task<ResponseModel<List<TeamDetailGroupedViewModel>>> GetTeamsByHR(int departmentId, string? teamAdminId);
    }
}
