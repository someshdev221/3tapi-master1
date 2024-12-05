using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface ICommonRepository
    {
        Task<int?> AddToDoList(ToDoListViewModel model, string assignedById, DateTime submitedOn, DateTime endTime);
        Task<int?> UpdateToDoList(ToDoListViewModel model, string assignedById, DateTime submitedOn, int taskId, DateTime endTime);
        Task<ToDoListModel> GetToDoList(string assignedTo, string Date);
        Task<List<ToDoViewModel>> GetToDoListByManager(DateTime date, ToDoRequestViewModel model);
        Task<List<ToDoViewModel>> GetToDoListByHODAndAdmin(DateTime date, ToDoRequestViewModel model);
        Task<List<TeamLeadToDoViewModel>> GetToDoListByTeamLead(DateTime date, string teamLeadId);
        Task<ToDoListModel> GetToDoListByEmployeeAsync(DateTime date, string EmployeeId);
        Task<bool> DeleteToDoListAsync(DateTime endTime);
        Task<List<ToDoListModel>> ToDoListExistsForTheDate(DateTime endTime);
        Task<bool> DeleteToDoById(int toDoId);
        Task<List<AttendaneBioMatricViewModel>> GetAttendanceLogAsync(string employeeCodeList, int month, int year);
    }
}
