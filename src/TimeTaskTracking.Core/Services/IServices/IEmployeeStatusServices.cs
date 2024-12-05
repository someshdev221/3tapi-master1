using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface IEmployeeStatusServices
    {
        Task<ResponseModel<PaginationResponseViewModel<List<EmployeeStatusResponseDto>>>> GetEmployeeStatusListAsync(EmployeeStatusPaginationViewModel employeeStatusPaginationViewModel);
        Task<ResponseModel<bool>> DeleteEmployeeStatusByIDAsync(EmployeeStatusFilterViewModel getAndDeleteEmployeeStatusViewModel);
        Task<ResponseModel<EmployeeStatusDto>> AddNewEmployeeStatusAsync(EmployeeStatusDto employeeStatusList);
        Task<ResponseModel<int>> UpdateEmployeeStatusAsync(EmployeeStatusDto editStatus);
        Task<ResponseModel<bool>> DeleteStatusAsync(int id);
        Task<ResponseModel<EstimatedHoursDto>> GetProjectModuleByIdAsync(string moduleId);
        Task<ResponseModel<int>> AddEmployeeLeaveAsync(string userId, bool? isPresent, DateTime Date, string attendanceStatus);
        Task<ResponseModel<int>> CheckIfUserAlreadyAppliedLeave(EmployeeStatusDto status);
        Task<ResponseModel<EmployeeStatusResponseDto>> GetEmployeeStatusById(int id);
        Task<ResponseModel<List<EmployeeStatusViewModel>>> GetEmployeeStatusListByManager(int Month, int Year , string employeeId, int departmentId);
        Task<ResponseModel<List<DropDownResponse<string>>>> GetOpenProjectModulesListByProjectId(int projectId);

    }
}
