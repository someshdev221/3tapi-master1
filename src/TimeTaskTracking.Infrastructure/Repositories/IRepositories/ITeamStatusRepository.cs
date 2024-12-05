using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface ITeamStatusRepository
    {
        Task<List<TeamStatusViewModel>> GetTeamStatusByTeamLead(string TLId, DateTime filterByDate);
        Task<List<TeamStatusViewModel>> UpdateAttendanceStatus(AttendanceDetails attendanceDetails);
        Task<EmployeeDetails> GetEmployeeAndTeamLeadDetails(TeamStatusDetails<string> teamStatus);
        Task<bool> DeleteEmployeeLeaves(string employeeId, DateTime date, string status);
        Task<bool> GetAttendanceStatusByEmployeeId(string employeeId, DateTime date);

    }
}
