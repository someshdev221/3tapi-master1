using Microsoft.AspNetCore.Mvc;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface IEmployeeService
    {
        Task<ResponseModel<dynamic>> GetEmployeesByFilter(EmployeeModel employeeModel);
        Task<ResponseModel<PaginationResponseViewModel<List<UserDetailsViewModel>>>> GetAllUsers(UserViewModel model);

        Task<ResponseModel<dynamic>> GetEmployeeProfileDetails(string employeeId, int departmentId);
        //Task<ResponseModel<bool>> GetEmployeesById(string employeeId, int departmentId);
        Task<ResponseModel<bool>> GetAwardById(int BadgeId);
        Task<ResponseModel<List<DropDownResponse<int>>>> GetAllDesignations(int departmentId);
        Task<ResponseModel<ManagerTeamLeadBAAndBDListViewModel>> GetProjectManagerOrTeamLeadOrBDMListByDepartment(int departmentId);
        Task<ResponseModel<List<DropDownResponse<int>>>> GetAllDepartments();
        Task<ResponseModel<List<DropDownResponse<string>>>> GetManagerList(int departmentId);
        Task<ResponseModel<List<DropDownResponse<string>>>> GetEmployeeByTeamLeader(string teamLeaderId);
        Task<ResponseModel<bool>> DeleteEmployeeById(string employeeId, int departmentId);
        //Task<ResponseModel<string>> UpdateEmployeeStatusById(string employeeId, int isActive);
        Task<ResponseModel<string>> UpdateEmployeeManagerAndStatus(UpdateManagerandStatusDto updateManagerandStatusModel);
        Task<ResponseModel<string>> UpdateEmployeeProfileByManager(UpdateEmployeeProfileByManagerViewModel profileModel);
        Task<ResponseModel<EmployeeDto>> GetEmployee(string employeeId, int departmentId);
        Task<ResponseModel<int>> AssignEmployeeAward(AssignAwardViewModel awardModel);
        Task<ResponseModel<bool>> GetDepartmentById(int deparmentId);
        Task<ResponseModel<bool>> DeleteAssignAwards(string employeeId, int badgeId);
        Task<ResponseModel<int>> AddMonthlyFeedbackForm(MonthlyFeedbackFormDto monthlyFeedbackForm);
        Task<ResponseModel<dynamic>> GetMonthlyTraineeFeedback(string employeeId, int departmentId);
        Task<ResponseModel<int>> UpdateMonthlyFeedbackForm(MonthlyFeedbackFormDto monthlyFeedbackForm);
        Task<ResponseModel<MonthlyFeedbackFormDto>> GetTraineeFeedbackById(int id, string employeeId);
        Task<ResponseModel<MonthlyFeedbackFormDto>> GetTraineeFeedbackByDate(int month, int year, string employeeId);
        Task<ResponseModel<bool>> DeleteTraineeFeedback(int feedBackId);
        Task<ResponseModel<List<UserBadgeDto>>> GetAllAwardsAsync();
        Task<ResponseModel<EmployeeListResponse>> GetListOfNewRequestAsync(string searchText, int departmentId);
        Task<ResponseModel<int>> AddOnRollFeedbackForm(AddOnRollFeedbackFormDto addOnRollFeedback);
        Task<ResponseModel<int>> UpdateOnRollFeedbackForm(AddOnRollFeedbackFormDto addOnRollFeedbackForm);
        Task<ResponseModel<MonthlyTraineeFeedbackViewModel>> GetMonthlyTraineeFeedbackById(int feedbackId, string employeeId);
        Task<ResponseModel<string>> RemoveEmployeeByManagerFromManagerTeamList(string employeeId);
        Task<ResponseModel<List<SalesPersonViewModel>>> GetSalesPersonsList([FromQuery] int departmentId);
        Task<ResponseModel<EmployeeDto>> GetEmployeeDetail(string employeeId);
        Task<ResponseModel<List<ManagerTeamLeadBAAndBDListDepartmentsViewModel>>> GetProjectManagerOrTeamLeadOrBDMListByDepartments(List<int> departmentIds);
    }

}
