using System.Reflection;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface IEmployeeRepository
    {
        Task<PaginationResponseViewModel<List<EmployeeViewModel>>> GetFilteredEmployees(EmployeeModel employeeModel, string role);
        Task<PaginationResponseViewModel<List<UserDetailsViewModel>>> GetAllUsersAsync(UserViewModel model);
        Task<Register> GetEmployeeById(string employeeId, int departmentId, string teamAdminId);
        Task<bool> GetBadgeExistanceById(int badgeId);
        Task<EmployeeProfileViewModel> GetEmployeeProfileById(string employeeId, int departmentId, string teamAdminId, string roleName);
        Task<List<DropDownResponse<int>>> GetDesignationList(int departmentId);
        Task<ManagerTeamLeadBAAndBDListViewModel> GetProjectManagerOrTeamLeadOrBDMListByDepartment(int departmentId);
        Task<List<DropDownResponse<int>>> GetDepartmentList();
        Task<List<DropDownResponse<string>>> GetManagerListAsync(int departmentId);
        Task<bool> DeleteEmployeeById(string employeeId, int departmentId);
        Task<string> GetHODByDepartmentIdAsync(int departmentId);
        Task <List<Register>> GetManagerListByDepartmentIdAsync(int departmentId);
        Task<string> UpdateEmployeeManagerAsync(string employeeId, int departmentId, string? teamAdminId);
        Task<string> UpdateEmployeeProfileByManagerAsync(UpdateEmployeeProfileByManagerViewModel profileModel);
        Task<string> UpdateEmployeeStatusById(string employeeId, int? isActive, int departmentId, string teamAdminId);
        Task<int> EmployeeCountInTeam(string teamLeadId);
        Task<List<DropDownResponse<string>>> GetEmployeeNameByTL(string TeamleadId);
        Task<TeamLeadEmployeeProfileViewModel> GetEmployeeDetailsIfAssigned(string employeeId, string teamLeaderId);
        Task<int> AssignAwardToEmployee(AssignAwardViewModel awardModel, DateOnly dateRecieved, string submittedBy);
        Task<bool> GetDepartmentExistById(int departmentId);
        Task<bool> DeleteAssignAwards(string employeeId, int badgeId);
        Task<AssignAwardViewModel> GetAssignedBadgeDetails(string employeeId, int badgeId);
        Task<PaginationResponseViewModel<List<EmployeeDetailsViewModel>>> GetEmployeeDetailsByTeamleaders(EmployeeModel employee, string teamLeaderId);
        Task<PaginationResponseViewModel<List<EmployeeViewModel>>> GetAllEmployeeByHR(EmployeeModel filterViewModel);
        Task<List<OnRollFeedbackDetailsViewModel>> GetOnRollFeedbackDetails(string id);
        Task<int> AddMonthlyTraineeFeedback(FeedbackForm form);
        Task<DropDownResponse<int>> GetDepartmentNameById(int departmentId);
        Task<List<MonthlyTraineeFeedbackViewModel>> GetMonthlyTraineeFeedbackDetails(string employeeId, string department);
        Task<int> UpdateMonthlyTraineeFeedback(FeedbackForm form);
        Task<FeedbackForm> GetTraineeFeedbackFormById(int feedBackId, string employeeId);
        Task<FeedbackForm> GetTraineeFeedbackFormByDate (int month , int year , string employeeId);
        Task<bool> DeleteTraineeFeedbackForm ( int feedBackId);
        Task<int> AddOnRollFeedback (FeedbackForm form);
        Task<int> UpdateOnRollFeedback(FeedbackForm form);
        Task<FeedbackCountViewModel> GetFeedbackCount(string applicationUserId);
        Task<FeedbackCountViewModel> GetFeedbackCountForTimePeriod(string applicationUserId);
        Task<List<UserBadge>> GetAllUserBadgesAsync();
        Task<(List<EmployeeViewModel> Employees, int TotalCount)> GetListOfNewRequestAsync(int departmentId, string searchText);
        Task<List<EmployeeDetailsViewModel>> GetEmployeeDetailsByTeamleader(string teamLeaderId);
        Task<MonthlyTraineeFeedbackViewModel> GetMonthlyTraineeFeedbackById(int feedbackId, string employeeId);
        Task<List<EmployeeDetailsViewModel>> GetEmployeeListByManager(string teamAdminId, int DepartmentId);
        Task<List<EmployeeDetailsViewModel>> GetEmployeeListByDepartmentId(int DepartmentId);
        Task<bool?> GetEmployeesCanEditStatus(string employeeId, int depratmentId);
        Task<string> RemoveEmployeeByManagerFromManagerTeamListAsync(string employeeId);
        Task<bool> RemoveEmployeeFromTeamAsync(string employeeId);
        Task<TeamleadIdViewModel> GetTeamLeadIdByEmployeeId(string employeeId);
        Task<List<SalesPersonViewModel>> GetSalesPersonsList(int departmentId);
        Task<ManagerTeamLeadBAAndBDListDepartmentsViewModel> GetProjectManagerOrTeamLeadOrBDMListByDepartments(List<int> departmentIds);
        Task<List<EmployeeListForBioMatric>> GetEmployeeListForBioMatric(string employeeId, string teamLeadId, string teamAdminId, int departmentId, int? PageNumber, int? PageSize, string SearchValue);
    }
}
