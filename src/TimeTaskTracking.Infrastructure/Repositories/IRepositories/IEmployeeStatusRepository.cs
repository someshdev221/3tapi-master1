using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{

    public interface IEmployeeStatusRepository
    {
        Task<PaginationResponseViewModel<List<EmployeeStatus>>> GetEmployeeStatusList(EmployeeStatusPaginationViewModel employeeStatusFilterViewModel);
        Task<bool> DeleteEmployeeStatus(EmployeeStatusFilterViewModel getAndDeleteEmployeeStatusViewModel);
        Task<EmployeeStatus> AddNewEmployeeStatus(EmployeeStatus employeeStatus,string teamLeadId,string teamAdminId);
        Task<EmployeeStatus> UpdateEmployeeStatus(EmployeeStatus employeeStatus);
        Task<bool> DeleteEmployeeStatusByID(int id);
        Task<bool> DeleteEmloyeeStatusForTheDate(string employeeId, DateTime date);
        Task<EstimatedHour> GetProjectModuleByIdAsync(string moduleId, int id);
        Task<EmployeeLeavesStatus> AddEmployeeLeave(string userId, bool? isPresent, DateTime Date, string attendanceStatus, string teamLeadId, string teamAdminId, string submittedBy);
        Task<List<EmployeeLeavesStatus>> CheckIfUserAlreadyAppliedLeave(string employeeId, DateTime date);
        Task <EmployeeLeavesStatus> CheckForExistingLeave (string employeeId, DateTime date);
        Task<EmployeeStatus> GetEmployeeStatusById(int id);
        Task<List<EmployeeStatusViewModel>> GetEmployeeStatusListByManager( int Month , int Year , int departmentId , string EmployeeId);
        Task<List<DropDownResponse<string>>> GetOpenProjectModulesListByProjectIdAsync(int projectId, int departmentId);
        Task<bool> UpdateEmployeeLeaveForTheEmployee(string employeeId, DateTime date, string leaveStatus, int existingLeaveId, string submittedBy);
        Task<bool> GetEmployeeStatusByModuleId(string Id);
        Task<bool> GetEmployeeStatusByUpworkProfileId(int id);
        Task<EmployeeStatus> GetEmployeeStatusByIdAsync(int id);

    }
}
