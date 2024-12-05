using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services
{
    public class SuperAdminDashboardService : ISuperAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public SuperAdminDashboardService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }


        public async Task<ResponseModel<List<DashboardResult>>> GetDashboardResultsAsync()
        {
            var response = new ResponseModel<List<DashboardResult>>();

            var dashboardResults = await _unitOfWork.SuperAdminDashboard.GetDashboardResultsAsync();
            if (dashboardResults != null)
            {
                response.Model = dashboardResults;
            }
            return response;
        }

        public async Task<ResponseModel<List<SuperAdminOverAllPerformanceViewModel>>> GetSuperAdminOverAllPerformance(int month, int year)
        {
            var response = new ResponseModel<List<SuperAdminOverAllPerformanceViewModel>>
            {
                Model = new List<SuperAdminOverAllPerformanceViewModel>()
            };
            try
            {
                var departmentIdList = await _unitOfWork.Employee.GetDepartmentList();

                if(departmentIdList?.Any() == true)
                {
                    foreach (var department in departmentIdList)
                    {
                        var result = await _unitOfWork.SuperAdminDashboard.GetSuperAdminOverAllPerformanceAsync(department.Id, month, year);
                        if (result != null)
                        {
                            response.Model.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message.Add($"An error occurred: {ex.Message}");
            }
            return response;
        }
    }

}

