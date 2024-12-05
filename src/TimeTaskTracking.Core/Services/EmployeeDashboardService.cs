using Microsoft.AspNetCore.Http;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Core.Services
{
    public class EmployeeDashboardService : IEmployeeDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public EmployeeDashboardService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }
        public async Task<ResponseModel<EmployeeDashboardViewModel>> GetEmployeeDashboardDetails(int month, int year)
        {
            var response = new ResponseModel<EmployeeDashboardViewModel>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var employeeDashoardDetails = await _unitOfWork.EmployeeDashboard.GetEmployeeDashboardDetails(claims.UserId, month, year);
            if (employeeDashoardDetails != null)
                response.Model = employeeDashoardDetails;
            else
                response.Message.Add(SharedResources.EmployeeDashboardDetailsNotFound);
            return response;
        }
    }
}
