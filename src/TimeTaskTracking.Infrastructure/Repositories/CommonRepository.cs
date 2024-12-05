using Microsoft.Extensions.Configuration;
using System.Data;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class CommonRepository : ICommonRepository
    {
        private readonly ExecuteProcedure _exec;
        private readonly ExecuteProcedureBioMatric _execBioMatric;

        public CommonRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);
            _execBioMatric = new ExecuteProcedureBioMatric(configuration);
        }
        public async Task<int?> AddToDoList(ToDoListViewModel model, string assignedById, DateTime submitedOn, DateTime endTime)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                new string[] { "@OptId", "@ToDoTask", "@DateTime", "@AssignedTo", "@AssignedBy", "@IsActive", "@Priority", "@EndTime" },
                new string[] { "1", model.ToDo, Convert.ToString(submitedOn), model.AssignedToId, assignedById, "0", "0", Convert.ToString(endTime) });

                if (obj?.Rows?.Count > 0)
                {
                    var addToDo = await _exec.DataTableToModelAsync<ToDoListModel>(obj);
                    return addToDo.Id;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to add the client.");
            }
        }

        public async Task<List<ToDoViewModel>> GetToDoListByManager(DateTime date, ToDoRequestViewModel model)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                    new string[] { "@OptId", "@TeamAdminId","@DepartmentId", "@EndTime" },
                    new string[] { "2", model.TeamAdminId, Convert.ToString(model.DepartmentId), Convert.ToString(date) });

                if (obj?.Rows?.Count > 0)
                {
                    var result = obj.AsEnumerable()
                        .GroupBy(row => new
                        {
                            EmployeeId = row.Field<string>("EmployeeId"),
                            EmployeeName = row.Field<string>("EmployeeName"),
                            TeamLeadId = row.Field<string>("TeamLeadId"),
                            TeamLeadName = row.Field<string>("TeamLeadName"),
                        })
                        .Select(group => new ToDoViewModel
                        {
                            EmployeeId = group.Key.EmployeeId,
                            EmployeeName = group.Key.EmployeeName,
                            TeamLeadId = group.Key.TeamLeadId,
                            TeamLeadName = group.Key.TeamLeadName,
                            ToDoList = group.Where(row => row.Field<int?>("Id") != null)
                                            .Select(row => new ToDoListModel
                                            {
                                                Id = row.Field<int>("Id"),
                                                Name = row.Field<string>("Name"),
                                                DateTime = row.Field<DateTime>("DateTime"),
                                                To = row.Field<string>("To"),
                                                ApplicationUsersId = row.Field<string>("ApplicationUsersId"),

                                            }).FirstOrDefault()
                        }).ToList();

                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to get the to-do list.", ex);
            }
        }

        public async Task<List<ToDoViewModel>> GetToDoListByHODAndAdmin(DateTime date, ToDoRequestViewModel model)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetToDoListsByHODAndAdmin",
                    new string[] { "@TeamAdminId", "@DepartmentId", "@EndTime" },
                    new string[] { model.TeamAdminId, Convert.ToString(model.DepartmentId), Convert.ToString(date) });

                if (obj?.Rows?.Count > 0)
                {
                    var result = obj.AsEnumerable()
                        .GroupBy(row => new
                        {
                            EmployeeId = row.Field<string>("EmployeeId"),
                            EmployeeName = row.Field<string>("EmployeeName"),
                            TeamLeadId = row.Field<string>("TeamLeadId"),
                            TeamLeadName = row.Field<string>("TeamLeadName"),
                        })
                        .Select(group => new ToDoViewModel
                        {
                            EmployeeId = group.Key.EmployeeId,
                            EmployeeName = group.Key.EmployeeName,
                            TeamLeadId = group.Key.TeamLeadId,
                            TeamLeadName = group.Key.TeamLeadName,
                            ToDoList = group.Where(row => row.Field<int?>("Id") != null)
                                            .Select(row => new ToDoListModel
                                            {
                                                Id = row.Field<int>("Id"),
                                                Name = row.Field<string>("Name"),
                                                DateTime = row.Field<DateTime>("DateTime"),
                                                To = row.Field<string>("To"),
                                                ApplicationUsersId = row.Field<string>("ApplicationUsersId"),

                                            }).FirstOrDefault()
                        }).ToList();

                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to get the to-do list.", ex);
            }
        }


        public async Task<ToDoListModel> GetToDoList(string assignedTo, string Date)
        {

            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                new string[] { "@OptId", "@DateTime", "@AssignedTo" },
                new string[] { "3", Convert.ToString(Date), assignedTo});

                if (obj?.Rows?.Count > 0)
                {
                    var addToDoTask = await _exec.DataTableToModelAsync<ToDoListModel>(obj);
                    return addToDoTask;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to add the client.");
            }
        }

        public async Task<int?> UpdateToDoList(ToDoListViewModel model, string assignedById, DateTime submitedOn, int taskId, DateTime endTime)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                new string[] { "@OptId", "@ToDoId", "@ToDoTask", "@DateTime", "@AssignedTo", "@AssignedBy", "@IsActive", "@Priority", "@EndTime" },
                new string[] { "4", Convert.ToString(taskId), model.ToDo, Convert.ToString(submitedOn), model.AssignedToId, assignedById, "0", "0", Convert.ToString(endTime) });

                if (obj?.Rows?.Count > 0)
                {
                    var addToDo = await _exec.DataTableToModelAsync<ToDoListModel>(obj);
                    return addToDo.Id;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to add the client.");
            }
        }

        public async Task<List<TeamLeadToDoViewModel>> GetToDoListByTeamLead(DateTime date, string teamLeadId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                    new string[] { "@OptId", "@TeamLeadId", "@EndTime" },
                    new string[] { "5", teamLeadId, Convert.ToString(date) });

                if (obj?.Rows?.Count > 0)
                {
                    var result = obj.AsEnumerable()
                        .GroupBy(row => new
                        {
                            EmployeeId = row.Field<string>("EmployeeId"),
                            EmployeeName = row.Field<string>("EmployeeName")
                        })
                        .Select(group => new TeamLeadToDoViewModel
                        {
                            EmployeeId = group.Key.EmployeeId,
                            EmployeeName = group.Key.EmployeeName,
                            ToDoList = group.Where(row => row.Field<int?>("Id") != null)
                                            .Select(row => new ToDoListModel
                                            {
                                                Id = row.Field<int>("Id"),
                                                Name = row.Field<string>("Name"),
                                                DateTime = row.Field<DateTime>("DateTime"),
                                                To = row.Field<string>("To"),
                                                ApplicationUsersId = row.Field<string>("ApplicationUsersId")
                                            }).FirstOrDefault()
                        }).ToList();

                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to get the to-do list.", ex);
            }
        }

        public async Task<ToDoListModel> GetToDoListByEmployeeAsync(DateTime date, string employeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                    new string[] { "@OptId", "@AssignedTo", "@EndTime" },
                    new string[] { "6", employeeId, Convert.ToString(date) });

                if (obj?.Rows?.Count > 0)
                {
                    var addToDoTask = await _exec.DataTableToModelAsync<ToDoListModel>(obj);
                    return addToDoTask;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to get the to-do list.", ex);
            }
        }

        public async Task<bool> DeleteToDoListAsync(DateTime endTime)
        {

            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                    new string[] { "@OptId", "@EndTime" },
                    new string[] { "7", Convert.ToString(endTime) });

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while Deleting to get the to-do list.", ex);
            }
        }

        public async Task<List<ToDoListModel>> ToDoListExistsForTheDate(DateTime endTime)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                    new string[] { "@OptId", "@EndTime" },
                    new string[] { "8", Convert.ToString(endTime) });

                if (obj?.Rows?.Count > 0)
                {
                    var toDoListForTheDate = await _exec.DataTableToListAsync<ToDoListModel>(obj);
                    return toDoListForTheDate;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to get the to-do list.", ex);
            };
        }

        public async Task<bool> DeleteToDoById(int toDoId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteToDo",
                    new string[] { "@OptId", "@ToDoId" },
                    new string[] { "9", Convert.ToString(toDoId) });

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while Deleting to get the to-do list.", ex);
            }
        }

        public async Task<List<AttendaneBioMatricViewModel>> GetAttendanceLogAsync(string employeeCodeList, int month, int year)
        {
            var obj = await _execBioMatric.Get_DataTableAsync("GetEmployeeAttendanceLogs",
                        new string[] { "@EmployeeCodes", "@Month", "@Year" },
                        new string[] { employeeCodeList, Convert.ToString(month), Convert.ToString(year) });

            if (obj?.Rows?.Count > 0)
            {
                var attendanceResponse = await _execBioMatric.DataTableToListAsync<AttendaneBioMatricViewModel>(obj);
                return attendanceResponse;
            }
            else
            {
                return null;
            }
        }
    }
}

