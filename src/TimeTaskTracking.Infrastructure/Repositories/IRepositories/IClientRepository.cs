using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories
{
    public interface IClientRepository
    {
        public void Add(IClientRepository clientRepository)
        {
            List<string> ids = new List<string>();
        }
        Task<PaginationResponseViewModel<List<Client>>> GetFilteredClients(FilterViewModel filterViewModel);
        Task<Client> GetClientDetailsById(int clientId, int departmentId);
        Task<Client> GetClientName(int departmentId, string name);
        Task<int> AddClient(Client client);
        Task<int> UpdateClientDetails(Client client);
        Task<bool> RemoveClient(int clientId);
        Task<List<DropDownResponse<string>>> GetClientNameByTL(string teamLeaderId, int departmentId);
        Task<List<DropDownResponse<string>>> GetClientListByDepartment(int departmentId);
        Task<Client> GetClientByPhoneNumber(int departmentId, string name);
        Task<Client> GetClientByEmail(int departmentId, string name);
        Task<Client> GetClient(int clientId);
    }
}
