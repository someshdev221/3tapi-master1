using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface ITeamStatusService
    {
        Task<ResponseModel<List<TeamStatusViewModel>>> GetTeamStatusByTeamLead(DateTime filterByDate,string? teamLeadId);
        Task<ResponseModel<List<object>>> UpdateAttendanceStatusAsync(AttendanceDetails attendanceDetails);
        Task<ResponseModel<bool>> SendWarningMailAsync(TeamStatusDetails<List<string>> teamStatus);
        Task<ResponseModel<bool>> SendWarningMailToEmployees(WarningEmailViewModel model);
    }
}
