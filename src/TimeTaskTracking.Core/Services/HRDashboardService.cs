using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services
{
    public class HRDashboardService : IHRDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HRDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel<List<TeamDetailGroupedViewModel>>> GetTeamsByHR(int departmentId, string? teamAdminId)
        {
            var result = new ResponseModel<List<TeamDetailGroupedViewModel>>();

            if (departmentId == 0)
            {
                departmentId = 1;
            }

            var checkDepartmentExists = await _unitOfWork.Employee.GetDepartmentExistById(departmentId);
            if (!checkDepartmentExists)
            {
                result.Message.Add(SharedResources.DepartmentNotFound);
                return result;
            }

            if (!string.IsNullOrEmpty(teamAdminId))
            {
                var validTeamAdminId = await _unitOfWork.Employee.GetEmployeeById(teamAdminId, departmentId, string.Empty);
                if (validTeamAdminId == null || validTeamAdminId.RoleName == "Employee" || validTeamAdminId.RoleName == "BDM" || validTeamAdminId.RoleName == "HR")
                {
                    result.Message.Add(SharedResources.NoManagerFound);
                    return result;
                }
            }

            var teamsList = await _unitOfWork.HRDashboard.GetTeamsByHRAsync(departmentId, teamAdminId);
            if (teamsList != null && teamsList.Any())
            {
                result.Model = teamsList;
            }
            else
            {
                result.Message.Add(SharedResources.NoTeamFound);
            }

            return result;
        }
    }
}
