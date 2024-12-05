using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using System.Data;
using System.Reflection;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ExecuteProcedure _exec;
        
        public EmployeeRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);
        }

        public async Task<PaginationResponseViewModel<List<EmployeeViewModel>>> GetFilteredEmployees(EmployeeModel employeeModel, string role)
        {
            try
            {
                if (employeeModel.IsActive == 3)
                    employeeModel.IsActive = null;

                var dataset = await _exec.Get_DataSetAsync("GetFilteredEmployees",
                new string[] { "@OptID", "@DepartmentID", "@Designation", "@IsActive", "@PageSize", "@SearchText", "@SortColumn", "@SortOrder", "@PageNumber", "@TeamAdminId", "@Role" },
                new string[] { "1", Convert.ToString(employeeModel.DepartmentId) ?? "0", employeeModel.Designation, Convert.ToString(employeeModel.IsActive),
        Convert.ToString(employeeModel.PageSize) ?? string.Empty, Convert.ToString(employeeModel.SearchValue) ?? string.Empty, Convert.ToString(employeeModel.SortColumn) ?? string.Empty,
        Convert.ToString(employeeModel.SortOrder) ?? string.Empty, Convert.ToString(employeeModel.PageNumber) ?? string.Empty, employeeModel.TeamAdminId, role });

                if (dataset?.Tables.Count == 0 || dataset == null)
                {
                    return null;
                }
                PaginationResponseViewModel<List<EmployeeViewModel>> paginationResponseViewModel = new();
                paginationResponseViewModel.results = await _exec.DataTableToListAsync<EmployeeViewModel>(dataset.Tables[0]);
                paginationResponseViewModel.TotalCount = Convert.ToInt32(dataset.Tables[1].Rows[0][0]);
                return paginationResponseViewModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<Register> GetEmployeeById(string employeeId, int departmentId, string teamAdminId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetFilteredEmployees",
                new string[] { "@EmpId", "@OptId", "@DepartmentID", "@TeamAdminId" },
                new string[] { Convert.ToString(employeeId), "4", departmentId.ToString(), Convert.ToString(teamAdminId) });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<Register>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<EmployeeProfileViewModel> GetEmployeeProfileById(string employeeId, int departmnetId, string teamAdminId, string roleName)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("GetFilteredEmployees",
                new string[] { "@EmpId", "@OptId", "@DepartmentID", "@TeamAdminId", "@Role" },
                new string[] { Convert.ToString(employeeId), "5", departmnetId.ToString(), Convert.ToString(teamAdminId), roleName });

                if (obj == null || obj.Tables.Count == 0 || obj.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                // Map the first table to EmployeeViewModel
                var employeeDetails = _exec.DataTableToModel<EmployeeProfileViewModel>(obj.Tables[0]);


                if (employeeDetails.EmployeeID == "" || employeeDetails.EmployeeID == null)
                {
                    return null;
                }

                // Map the second table to a list of EmployeeProjectsViewModel
                var employeeProjects = await _exec.DataTableToListAsync<EmployeeProjectsViewModel>(obj.Tables[1]);
                employeeDetails.Projects = employeeProjects;

                if (obj.Tables.Count >= 0)
                {
                    var employeeUserTools = await _exec.DataTableToListAsync<EmployeeToolsViewModel>(obj.Tables[2]);
                    employeeDetails.UserTools = employeeUserTools;
                }

                if (obj.Tables.Count >= 0)
                {
                    var userBadges = await _exec.DataTableToListAsync<UserBadgeViewModel>(obj.Tables[3]);
                    employeeDetails.UserBadges = userBadges;
                }

                if (obj.Tables.Count >= 0 && roleName is not "HR")
                {
                    var awardList = await _exec.DataTableToListAsync<AwardListViewModel>(obj.Tables[4]);
                    awardList.RemoveAll(a => a.Id == 4);
                    employeeDetails.AwardList = awardList;
                }

                return employeeDetails;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<List<DropDownResponse<int>>> GetDesignationList(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetFilteredEmployees",
                    new string[] { "@OptID", "@DepartmentId" },
                    new string[] { "2", Convert.ToString(departmentId) });

                if (obj == null || obj?.Rows.Count == 0)
                {
                    return null;
                }
                return await _exec.DataTableToListAsync<DropDownResponse<int>>(obj);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ManagerTeamLeadBAAndBDListViewModel> GetProjectManagerOrTeamLeadOrBDMListByDepartment(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("GetEmployeesByMultipleDesignations",
                    new string[] { "@DepartmentId" },
                    new string[] { Convert.ToString(departmentId) });

                if (obj?.Tables?.Count > 0 && obj.Tables[0] != null)
                {
                    ManagerTeamLeadBAAndBDListViewModel teamLeadBAAndBDListViewModel = new();
                    teamLeadBAAndBDListViewModel.Manager = await _exec.DataTableToListAsync<DropDownResponse<string>>(obj.Tables[0]);
                    teamLeadBAAndBDListViewModel.TeamLead = await _exec.DataTableToListAsync<DropDownResponse<string>>(obj.Tables[1]);
                    teamLeadBAAndBDListViewModel.BDM = await _exec.DataTableToListAsync<DropDownResponse<string>>(obj.Tables[2]);
                    return teamLeadBAAndBDListViewModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DropDownResponse<int>>> GetDepartmentList()
        {
            try
            {
                // Call a method, possibly from a data access library, to execute a database query.
                var obj = await _exec.Get_DataTableAsync("GetDepartments",
                new string[] { "@OptId" },
                new string[] { "1" });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }
                return _exec.DataTableToList<DropDownResponse<int>>(obj);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteEmployeeById(string employeeId, int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployee",
                    new string[] { "@OptID", "@EmployeeID", "@DepartmentId", },
                    new string[] { "1", employeeId, Convert.ToString(departmentId) });

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GetHODByDepartmentIdAsync(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetHODAndManagerByDepartmentId",
                    new string[] { "@OptID", "@DepartmentId" },
                    new string[] { "1",  Convert.ToString(departmentId) });

                if (obj?.Rows?.Count > 0)
                {
                    var employeeDetails = await _exec.DataTableToModelAsync<Register>(obj);
                    return employeeDetails.Id;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Register>> GetManagerListByDepartmentIdAsync(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetHODAndManagerByDepartmentId", 
                    new string[] { "@OptID", "@DepartmentId" },
                    new string[] { "2", Convert.ToString(departmentId) });

                if (obj?.Rows?.Count > 0)
                {
                    var employeeDetails = await _exec.DataTableToListAsync<Register>(obj);
                    return employeeDetails;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> UpdateEmployeeStatusById(string employeeId, int? isActive, int departmentId, string teamAdminId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployee",
                    new string[] { "@OptID", "@EmployeeID", "@IsActive", "@DepartmentId", "@TeamAdminId" },
                    new string[] { "2", employeeId ?? string.Empty, Convert.ToString(isActive), Convert.ToString(departmentId), Convert.ToString(teamAdminId) });

                if (obj?.Rows?.Count > 0)
                {
                    return employeeId;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> UpdateEmployeeManagerAsync(string employeeId, int departmentId, string? teamAdminId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployee",
                    new string[] { "@OptID", "@EmployeeID", "@DepartmentId", "@TeamAdminId" },
                    new string[] { "3", employeeId ?? string.Empty, Convert.ToString(departmentId), teamAdminId });

                if (obj?.Rows?.Count > 0)
                {
                    var updateemployeeDetails = await _exec.DataTableToModelAsync<EmployeeViewModel>(obj);
                    updateemployeeDetails.EmployeeID = employeeId;
                    return updateemployeeDetails.EmployeeID;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<string> UpdateEmployeeProfileByManagerAsync(UpdateEmployeeProfileByManagerViewModel profileModel)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("UpdateEmployeeProfileByManager",
                    new string[] { "@UserId", "@FirstName", "@LastName", "@Designation", "@DepartmentId", "@Email", "@PhoneNumber", "@Skills", "@SkypeMail", "@IsActive", "@JoiningDate", "@CanEditStatus", "@Address" },
                    new string[] {
                profileModel.EmployeeId,
                profileModel.FirstName,
                profileModel.LastName,
                profileModel.Designation,
                profileModel.DepartmentId.ToString(),
                profileModel.Email,
                profileModel.PhoneNumber,
                profileModel.Skills,
                profileModel.SkypeMail,
                profileModel.IsActive.ToString(),
                profileModel.JoiningDate.ToString("yyyy-MM-dd"), // Ensure date format
                profileModel.CanEditStatus.ToString(),
                profileModel.Address
                    });

                if (obj?.Rows?.Count > 0)
                {
                    var updateemployeeDetails = await _exec.DataTableToModelAsync<UpdateEmployeeProfileByManagerViewModel>(obj);
                    updateemployeeDetails.EmployeeId = profileModel.EmployeeId;
                    return updateemployeeDetails.EmployeeId;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }


        public async Task<int> EmployeeCountInTeam(string teamLeadId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetEmployee",
                    new string[] { "@OptID", "@teamLeadId" },
                    new string[] { "4", teamLeadId });

                if (obj?.Rows?.Count > 0)
                {
                    var employeeCountInTeam = Convert.ToInt32(obj.Rows[0][0]);
                    return employeeCountInTeam;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<DropDownResponse<string>>> GetEmployeeNameByTL(string TeamleadId)
        {
            try
            {
                var reportsList = await _exec.Get_DataSetAsync("GetEmployeeIdByTeamLeadId",
                    new string[] { "@OptID", "@TeamLeaderId" },
                    new string[] { "1", Convert.ToString(TeamleadId) });

                if (reportsList?.Tables?.Count > 0 && reportsList.Tables[0] != null)
                {

                    return await _exec.DataTableToListAsync<DropDownResponse<string>>(reportsList.Tables[0]);
                }

                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<List<DropDownResponse<string>>> GetManagerListAsync(int departmentId)
        {
            try
            {
                // Call a method, possibly from a data access library, to execute a database query.
                var obj = await _exec.Get_DataTableAsync("GetFilteredEmployees",
                new string[] { "@OptId", "@DepartmentID" },
                new string[] { "6", departmentId.ToString() });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }
                return _exec.DataTableToList<DropDownResponse<string>>(obj);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> AssignAwardToEmployee(AssignAwardViewModel awardModel, DateOnly dateRecieved, string submittedBy)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AssignUserBadge",
                    new string[] { "@OptId", "@BadgeId", "@UserId", "@BadgeDescription", "@DateReceived", "@SubmittedBy" },
                    new string[] { "1", Convert.ToString(awardModel.BadgeId), awardModel.UserId, awardModel.BadgeDescription, Convert.ToString(dateRecieved), submittedBy });

                if (obj?.Rows?.Count > 0)
                {
                    var assignedAward = await _exec.DataTableToModelAsync<AssignAwardViewModel>(obj);
                    return assignedAward.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to assign award to the Employee.");
            }
        }

        public async Task<bool> GetBadgeExistanceById(int badgeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AssignUserBadge",
                new string[] { "@OptId", "@BadgeId" },
                new string[] { "2", badgeId.ToString() });

                if (obj != null && obj.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<TeamLeadEmployeeProfileViewModel> GetEmployeeDetailsIfAssigned(string employeeId, string teamLeaderId)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("GetEmployeeIdByTeamLeadId",
                    new string[] { "@OptID", "@TeamLeaderId", "@EmployeeId" },
                    new string[] { "2", Convert.ToString(teamLeaderId), Convert.ToString(employeeId) });

                if (obj == null || obj.Tables.Count == 0 || obj.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                var employeeDetails = _exec.DataTableToModel<TeamLeadEmployeeProfileViewModel>(obj.Tables[0]);

                if (string.IsNullOrEmpty(employeeDetails.EmployeeID))
                {
                    return null;
                }
                var employeeProjects = await _exec.DataTableToListAsync<EmployeeProjectsViewModel>(obj.Tables[1]);
                employeeDetails.Projects = employeeProjects;
                if (obj.Tables.Count > 0)
                {
                    var employeeUserTools = await _exec.DataTableToListAsync<EmployeeToolsViewModel>(obj.Tables[2]);
                    employeeDetails.UserTools = employeeUserTools;
                }

                if (obj.Tables.Count > 0)
                {
                    var userBadges = await _exec.DataTableToListAsync<UserBadgeViewModel>(obj.Tables[3]);
                    employeeDetails.UserBadges = userBadges;
                }

                if (obj.Tables.Count > 0)
                {
                    var awardList = await _exec.DataTableToListAsync<AwardListViewModel>(obj.Tables[4]);
                    awardList.RemoveAll(a => a.Id == 4 || a.Id == 6);
                    employeeDetails.AwardList = awardList;
                }

                return employeeDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> GetDepartmentExistById(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetDepartmentById",
                new string[] { "@DepartmentId" },
                new string[] { departmentId.ToString() });

                if (obj != null && obj.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAssignAwards(string employeeId, int badgeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AssignUserBadge",
                    new string[] { "@OptID", "@UserId", "@BadgeId", },
                    new string[] { "3", employeeId, Convert.ToString(badgeId) });

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<AssignAwardViewModel> GetAssignedBadgeDetails(string employeeId, int badgeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AssignUserBadge",
                    new string[] { "@OptID", "@UserId", "@BadgeId", },
                    new string[] { "5", employeeId, Convert.ToString(badgeId) });

                if (obj?.Rows?.Count > 0)
                {
                    var response  = await _exec.DataTableToModelAsync<AssignAwardViewModel>(obj);
                    return response;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<PaginationResponseViewModel<List<EmployeeDetailsViewModel>>> GetEmployeeDetailsByTeamleaders(EmployeeModel employee, string teamLeaderId)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("GetEmployeeByTeamLeaders",
                    new string[] { "@OptID", "@TeamLeaderId", "@SearchText" },
                    new string[] { "1", Convert.ToString(teamLeaderId), Convert.ToString(employee.SearchValue) });

                if (obj?.Tables?.Count > 2 && obj.Tables[0] != null && obj.Tables[1] != null && obj.Tables[2] != null)
                {


                    var employeeDetails = await _exec.DataTableToListAsync<EmployeeDetailsViewModel>(obj.Tables[0]);
                    var badges = await _exec.DataTableToListAsync<BadgeViewModel>(obj.Tables[1]);

                    var badgesLookup = badges.ToLookup(b => b.UserId);
                    employeeDetails.ForEach(employee => employee.Badges = badgesLookup[employee.Id].ToList());

                    PaginationResponseViewModel<List<EmployeeDetailsViewModel>> paginationResponseViewModel = new();
                    {
                        paginationResponseViewModel.results = employeeDetails;
                        paginationResponseViewModel.TotalCount = Convert.ToInt32(obj.Tables[2].Rows[0]["TotalCount"]);
                    };

                    return paginationResponseViewModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return null;
            }
        }


        public async Task<List<EmployeeDetailsViewModel>> GetEmployeeDetailsByTeamleader(string teamLeaderId)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("GetEmployeeByTeamLead",
                    new string[] { "@OptID", "@TeamLeaderId" },
                    new string[] { "1", Convert.ToString(teamLeaderId) });

                if (obj?.Tables?.Count > 1 && obj.Tables[0] != null && obj.Tables[1] != null)
                {
                    var employeeDetails = await _exec.DataTableToListAsync<EmployeeDetailsViewModel>(obj.Tables[0]);
                    var badges = await _exec.DataTableToListAsync<BadgeViewModel>(obj.Tables[1]);

                    var badgesLookup = badges.ToLookup(b => b.UserId);
                    employeeDetails.ForEach(employee => employee.Badges = badgesLookup[employee.Id].ToList());


                    return employeeDetails;
                }
                return null;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return null;
            }
        }

        public async Task<PaginationResponseViewModel<List<EmployeeViewModel>>> GetAllEmployeeByHR(EmployeeModel filterViewModel)
        {
            try
            {
                var dataset = await _exec.Get_DataSetAsync("GetAllEmployeesByHr",
                new string[] { "@TeamAdminId", "@Designation", "@DepartmentID", "@PageSize", "@PageNumber", "@SearchKeyword", "@SortColumn", "@SortOrder", "@IsActive" },
                new string[] { Convert.ToString(filterViewModel.TeamAdminId),Convert.ToString(filterViewModel.Designation),Convert.ToString(filterViewModel.DepartmentId) , Convert.ToString(filterViewModel.PageSize) ?? "0", Convert.ToString(filterViewModel.PageNumber) ?? "0" , Convert.ToString(filterViewModel.SearchValue) ,
                 Convert.ToString(filterViewModel.SortColumn) , Convert.ToString(filterViewModel.SortOrder), Convert.ToString(filterViewModel.IsActive)

                });
                if (dataset?.Tables.Count == 0 || dataset == null)
                {
                    return null;
                }
                PaginationResponseViewModel<List<EmployeeViewModel>> paginationResponseViewModel = new();
                paginationResponseViewModel.results = await _exec.DataTableToListAsync<EmployeeViewModel>(dataset.Tables[0]);
                paginationResponseViewModel.TotalCount = Convert.ToInt32(dataset.Tables[1].Rows[0][0]);
                return paginationResponseViewModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<OnRollFeedbackDetailsViewModel>> GetOnRollFeedbackDetails(string id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetOnRollFeedBackFormsDetails",
                new string[] { "@Id" },
                new string[] { id });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }
                return _exec.DataTableToList<OnRollFeedbackDetailsViewModel>(obj);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> AddMonthlyTraineeFeedback(FeedbackForm form)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AddFeedbackForm",
                    new string[] {  "@ApplicationUserId", "@Name", "@Designation", "@Department", "@DOJ", "@AssessmentMonth","@MentorName",  "@SkillSet",
                        "@Comments" , "@Performance"
                    },
                new string[] {form.ApplicationUserId , form.Name , form.Designation , form.Department , Convert.ToString(form.DOJ) ,
                   Convert.ToString(form.AssessmentMonth) , form.MentorName , form.SkillSet , form.Comments , Convert.ToString(form.Performance)
                });

                if (obj?.Rows?.Count > 0)
                {
                    var feedbackData = await _exec.DataTableToModelAsync<FeedbackForm>(obj);
                    return feedbackData.FeedBackId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<DropDownResponse<int>> GetDepartmentNameById(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetDepartmentById",
                new string[] { "@DepartmentId" },
                new string[] { departmentId.ToString() });

                if (obj != null && obj.Rows.Count > 0)
                {
                    var response = await _exec.DataTableToModelAsync<DropDownResponse<int>>(obj);
                    return response;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<MonthlyTraineeFeedbackViewModel>> GetMonthlyTraineeFeedbackDetails(string employeeId, string department)
        {

            try
            {
                var obj = await _exec.Get_DataTableAsync("GetMonthlyTraineeFeedback",
                new string[] { "@EmployeeId", "@Department" },
                new string[] { employeeId, department });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }
                return _exec.DataTableToList<MonthlyTraineeFeedbackViewModel>(obj);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> UpdateMonthlyTraineeFeedback(FeedbackForm form)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("UpdateFeedbackForm",
                    new string[] {  "@ApplicationUserId", "@Name", "@Designation", "@Department", "@DOJ", "@AssessmentMonth","@MentorName",  "@SkillSet",
                        "@Comments" , "@Performance" ,"@FeedBackId" ,"@OptId"
                    },
                new string[] {form.ApplicationUserId , form.Name , form.Designation , form.Department , Convert.ToString(form.DOJ) ,
                   Convert.ToString(form.AssessmentMonth) , form.MentorName , form.SkillSet , form.Comments , Convert.ToString(form.Performance), Convert.ToString(form.FeedBackId)
                   ,"1"
                });

                if (obj?.Rows?.Count > 0)
                {
                    var feedbackData = await _exec.DataTableToModelAsync<FeedbackForm>(obj);
                    return feedbackData.FeedBackId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<FeedbackForm> GetTraineeFeedbackFormById(int feedBackId, string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("UpdateFeedbackForm",
                    new string[] { "@FeedBackId", "@OptId", "@EmployeeId" },
                    new string[] { Convert.ToString(feedBackId), "2", employeeId });

                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToModelAsync<FeedbackForm>(obj);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<FeedbackForm> GetTraineeFeedbackFormByDate(int month, int year, string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("UpdateFeedbackForm",
                    new string[] { "@Month", "@Year", "@EmployeeId", "@OptId" },
                    new string[] { Convert.ToString(month), Convert.ToString(year), Convert.ToString(employeeId), "3" });

                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToModelAsync<FeedbackForm>(obj);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteTraineeFeedbackForm(int feedBackId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("UpdateFeedbackForm",
                    new string[] { "@OptID", "@FeedBackId" },
                    new string[] { "4", Convert.ToString(feedBackId) });

                if (obj.Rows.Count > 0)
                {
                    var isDeleted = Convert.ToBoolean(obj.Rows[0]["IsDeleted"]);
                    var returnedId = Convert.ToInt32(obj.Rows[0]["FeedBackId"]);

                    if (isDeleted && returnedId != 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> AddOnRollFeedback(FeedbackForm form)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AddOnRollFeedbackForm",
                    new string[] {  "@ApplicationUserId", "@Name", "@Designation", "@Department", "@DOJ", "@AssessmentMonth","@MentorName", "@StartSalary",  "@SkillSet",
                        "@Comments" ,"@Strength" , "@Weakness" , "@TimePeriod","@OptId"
                    },
                new string[] {form.ApplicationUserId , form.Name , form.Designation , form.Department , Convert.ToString(form.DOJ) ,
                   Convert.ToString(form.AssessmentMonth) , form.MentorName , Convert.ToString(form.StartSalary) , form.SkillSet , form.Comments,
                   form.Strength , form.Weakness , form.TimePeriod ,"1"
                });

                if (obj?.Rows?.Count > 0)
                {
                    var feedbackData = await _exec.DataTableToModelAsync<FeedbackForm>(obj);
                    return feedbackData.FeedBackId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> UpdateOnRollFeedback(FeedbackForm form)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AddOnRollFeedbackForm",
                    new string[] {  "@ApplicationUserId", "@Name", "@Designation", "@Department", "@DOJ", "@AssessmentMonth","@MentorName",  "@SkillSet", "@StartSalary",
                        "@Comments"  ,"@FeedBackId" ,"@Strength" , "@Weakness" , "@TimePeriod" ,"@OptId"
                    },
                new string[] {form.ApplicationUserId , form.Name , form.Designation , form.Department , Convert.ToString(form.DOJ) ,
                   Convert.ToString(form.AssessmentMonth) , form.MentorName , form.SkillSet ,Convert.ToString(form.StartSalary) , form.Comments , Convert.ToString(form.FeedBackId)
                   ,form.Strength , form.Weakness , form.TimePeriod , "2"
                });

                if (obj?.Rows?.Count > 0)
                {
                    var feedbackData = await _exec.DataTableToModelAsync<FeedbackForm>(obj);
                    return feedbackData.FeedBackId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<FeedbackCountViewModel> GetFeedbackCount(string applicationUserId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AddOnRollFeedbackForm",
                    new string[] { "@ApplicationUserId", "@OptId" },
                    new string[] { Convert.ToString(applicationUserId), "3" });

                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToModelAsync<FeedbackCountViewModel>(obj);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<FeedbackCountViewModel> GetFeedbackCountForTimePeriod(string applicationUserId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AddOnRollFeedbackForm",
                    new string[] { "@ApplicationUserId", "@OptId" },
                    new string[] { Convert.ToString(applicationUserId), "4" });

                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToModelAsync<FeedbackCountViewModel>(obj);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<UserBadge>> GetAllUserBadgesAsync()
        {
            try
            {
                var dataset = await _exec.Get_DataSetAsync("AssignUserBadge",
                    new string[] { "@OptId" }, new string[] { "4" });

                if (dataset?.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                {
                  
                    var userBadges = await _exec.DataTableToListAsync<UserBadge>(dataset.Tables[0]);
                    return userBadges;
                }
                return new List<UserBadge>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public async Task<PaginationResponseViewModel<List<UserDetailsViewModel>>> GetAllUsersAsync(UserViewModel model)
        {
            try
            {
                var dataset = await _exec.Get_DataSetAsync("GetAllUsers",
                    new string[] { "@DepartmentId", "@PageNumber", "@PageSize", "@SearchText" },
                    new string[] { model.DepartmentId.ToString(), model.PageNumber.ToString() ?? "0", model.PageSize.ToString() ?? "0", model.SearchValue ?? string.Empty });

                if (dataset != null || dataset?.Tables.Count != 0)
                {
                    PaginationResponseViewModel<List<UserDetailsViewModel>> paginationResponseViewModel = new();
                    paginationResponseViewModel.results = await _exec.DataTableToListAsync<UserDetailsViewModel>(dataset.Tables[0]);
                    paginationResponseViewModel.TotalCount = Convert.ToInt32(dataset.Tables[1].Rows[0][0]);

                    foreach (var user in paginationResponseViewModel.results)
                    {
                        var awards = await GetAwardsForUserAsync(user.Id);
                        user.Awards = awards;
                    }
                    return paginationResponseViewModel;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<List<UserBadgeViewModel>> GetAwardsForUserAsync(string userId)
        {
            var awards = await _exec.Get_DataTableAsync("GetUserBadgesByEmployeeId",
                new string[] { "@EmpId" },
                new string[] { userId });

            if (awards != null)
            {
                return _exec.DataTableToList<UserBadgeViewModel>(awards);
            }
            return new List<UserBadgeViewModel>();
        }
        public async Task<(List<EmployeeViewModel> Employees, int TotalCount)> GetListOfNewRequestAsync(int departmentId, string searchText)
        {
            try
            {
                var isActive = string.IsNullOrEmpty(searchText) ? 2 : 1;

                var dataset = await _exec.Get_DataSetAsync("GetFilteredEmployees",
                    new string[] { "@OptID", "@SearchText", "@DepartmentId" },
                    new string[] { "10", searchText ?? string.Empty, departmentId.ToString() });

                if (dataset?.Tables.Count == 0 || dataset == null)
                {
                    return (null, 0);
                }
                var employees = await _exec.DataTableToListAsync<EmployeeViewModel>(dataset.Tables[0]);
                int totalCount = dataset.Tables[0].Rows.Count > 0 ? Convert.ToInt32(dataset.Tables[0].Rows[0]["TotalCount"]) : 0;

                if (string.IsNullOrEmpty(searchText))
                {
                    employees = employees.Where(e => e.EmpStatus == isActive).ToList();
                }
                return (employees, totalCount);
            }
            catch (Exception ex)
            {
                return (null, 0);
            }
        }

        public async Task<MonthlyTraineeFeedbackViewModel> GetMonthlyTraineeFeedbackById(int feedbackId, string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetMonthlyFeedbackById",
                new string[] { "@FeedbackId", "@EmployeeId" },
                new string[] { Convert.ToString(feedbackId), employeeId });

                if (obj == null || obj.Rows.Count == 0)
                {
                    return null;
                }
                return _exec.DataTableToModel<MonthlyTraineeFeedbackViewModel>(obj);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<EmployeeDetailsViewModel>> GetEmployeeListByManager(string teamAdminId, int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetEmployeesListByManagerId",
                new string[] { "@OptId", "@TeamAdminId", "@DepartmentId" },
                new string[] { "1", teamAdminId, Convert.ToString(departmentId) });

                if (obj?.Rows.Count > 0)
                {
                    var response = await _exec.DataTableToListAsync<EmployeeDetailsViewModel>(obj);
                    return response;
                }

                return new List<EmployeeDetailsViewModel>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<EmployeeDetailsViewModel>> GetEmployeeListByDepartmentId(int DepartmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetEmployeesListByManagerId",
                new string[] { "@OptId", "@DepartmentId" },
                new string[] { "2", Convert.ToString(DepartmentId) });

                if (obj?.Rows.Count > 0)
                {
                    var response = await _exec.DataTableToListAsync<EmployeeDetailsViewModel>(obj);
                    return response;

                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool?> GetEmployeesCanEditStatus(string employeeId, int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetEmployeeCanEditStatus",
                    new string[] { "@EmpId", "@DepartmentId" },
                    new string[] { employeeId, Convert.ToString(departmentId) });

                if (obj?.Rows.Count > 0)
                {
                    // Assuming CanEditStatus is the specific column we need
                    var canEditStatus = Convert.ToBoolean(obj.Rows[0]["CanEditStatus"]);
                    if (canEditStatus)
                    {
                        return true;
                    }
                    return false;
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log exception (if necessary)
                return null;
            }
        }

        public async Task<string> RemoveEmployeeByManagerFromManagerTeamListAsync(string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("RemoveEmployeeManagerAndUpdateIsActive",
                    new string[] { "@OptId","@EmpId" },
                    new string[] { "1", employeeId});

                if (obj?.Rows?.Count > 0)
                {
                    var result = await _exec.DataTableToModelAsync<Register>(obj);
                    return result.Id;
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log exception (if necessary)
                return null;
            }
        }

        public async Task<bool> RemoveEmployeeFromTeamAsync(string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("RemoveEmployeeManagerAndUpdateIsActive",
                    new string[] { "@OptId", "@EmpId" },
                    new string[] { "2", employeeId });

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<TeamleadIdViewModel> GetTeamLeadIdByEmployeeId(string employeeId)
        {
            var obj = await _exec.Get_DataTableAsync("GetTeamLeadIdByEmployeeId",
                        new string[] { "@EmployeeId" },
                        new string[] { employeeId });

            if (obj?.Rows?.Count > 0)
            {
                var teamleadId = await _exec.DataTableToModelAsync<TeamleadIdViewModel>(obj);
                return teamleadId;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<SalesPersonViewModel>> GetSalesPersonsList(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteEmployee",
                       new string[] { "@OptID", "@DepartmentId" },
                       new string[] { "4", Convert.ToString(departmentId) });

                if (obj?.Rows?.Count > 0)
                {
                    var salesPersons = await _exec.DataTableToListAsync<SalesPersonViewModel>(obj);
                    return salesPersons;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ManagerTeamLeadBAAndBDListDepartmentsViewModel> GetProjectManagerOrTeamLeadOrBDMListByDepartments(List<int> departmentIds)
        {
            try
            {
                string departmentIdsCommaSeparated = string.Join(",", departmentIds);
                var obj = await _exec.Get_DataSetAsync("ExecuteEmployee",
                    new string[] { "@OptID","@DepartmentIdsCommaSeparated" },
                    new string[] { "5",Convert.ToString(departmentIdsCommaSeparated) });

                if (obj?.Tables?.Count > 0 && obj.Tables[0] != null)
                {
                    ManagerTeamLeadBAAndBDListDepartmentsViewModel teamLeadBAAndBDListViewModel = new();
                    teamLeadBAAndBDListViewModel.Manager = await _exec.DataTableToListAsync<DropDownResponseDepartment<string>>(obj.Tables[0]);
                    teamLeadBAAndBDListViewModel.TeamLead = await _exec.DataTableToListAsync<DropDownResponseDepartment<string>>(obj.Tables[1]);
                    teamLeadBAAndBDListViewModel.BDM = await _exec.DataTableToListAsync<DropDownResponseDepartment<string>>(obj.Tables[2]);
                    return teamLeadBAAndBDListViewModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<EmployeeListForBioMatric>> GetEmployeeListForBioMatric(string employeeId, string teamLeadId, string teamAdminId, int departmentId, int? PageNumber, int? PageSize, string SearchValue)
        {
            var obj = await _exec.Get_DataTableAsync("GetEmployeeListForBioMatric",
                       new string[] { "@EmployeeId", "@TeamLeadId", "@TeamAdminId", "@DepartmentId","@PageNumber", "@PageSize", "@SearchString" },
                       new string[] {  Convert.ToString(employeeId), Convert.ToString(teamLeadId), Convert.ToString(teamAdminId), Convert.ToString(departmentId), Convert.ToString(PageNumber), Convert.ToString(PageSize), SearchValue});

            if (obj?.Rows?.Count > 0)
            {
                var employeeDetails = await _exec.DataTableToListAsync<EmployeeListForBioMatric>(obj);
                return employeeDetails;
            }
            else
            {
                return null;
            }
        }
    }
}


