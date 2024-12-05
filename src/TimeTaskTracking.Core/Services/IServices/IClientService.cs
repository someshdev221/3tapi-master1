using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices
{
    public interface IClientService
    {
        Task<ResponseModel<PaginationResponseViewModel<List<ClientDto>>>> GetClientsByFilter(FilterViewModel FilterViewModel);
        Task<ResponseModel<ClientDto>> GetClientById(int clientId, int departmentId);
        Task<ResponseModel<ClientDto>> GetClientByName(int departmentId, string name);
        Task<ResponseModel<int>> AddNewClient(ClientDto client);
        Task<ResponseModel<int>> UpdateClient(ClientDto client);
        Task<ResponseModel<bool>> RemoveClient(int clientId);
        Task<ResponseModel<List<DropDownResponse<string>>>> GetClientByTeamLeader(string teamLeaderId, int departmentId);
        Task<ResponseModel<List<DropDownResponse<string>>>> GetClientListByDepartment(int departmentId);
        Task<ResponseModel<bool>> GetDepartmentById(int deparmentId);
        Task<ResponseModel<ClientDto>> GetClientByEmail(int departmentId, string email);
        Task<ResponseModel<ClientDto>> GetClientByPhoneNumber(int departmentId, string phoneNumber);
        Task<ResponseModel<ClientDto>> GetClient(int clientId);
    }
}
