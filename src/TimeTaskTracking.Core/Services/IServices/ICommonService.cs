using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface ICommonService
    {
        Task<ResponseModel<int?>> AddToDoList(ToDoListViewModel model);
        Task<ResponseModel<List<dynamic>>> GetToDoListByManagerAndTeamLead(ToDoRequestViewModel model);
        Task<ResponseModel<ToDoListModel>> GetToDoListByEmployee();
        Task<ResponseModel<List<dynamic>>> GetBioMatricAttendanceLogs(BioMatricRequestViewModel model);
        //Task<ResponseModel<bool>> DeleteToDoList();
    }
}
