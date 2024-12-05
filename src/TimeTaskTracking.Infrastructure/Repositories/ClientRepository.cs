using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ExecuteProcedure _exec;

        public ClientRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);
        }

        public async Task<PaginationResponseViewModel<List<Client>>> GetFilteredClients(FilterViewModel filterViewModel)
        {
            try
            {
                var dataset = await _exec.Get_DataSetAsync("GetClient",
                new string[] { "@OptId", "@DeptID", "@PageNumber", "@PageSize", "@SearchText", "@SortColumn", "@SortOrder" },
                new string[] { "1", Convert.ToString(filterViewModel.DepartmentId) ?? "0", Convert.ToString(filterViewModel.PageNumber)?? "0", Convert.ToString(filterViewModel.PageSize) ?? "0", Convert.ToString(filterViewModel.SearchValue) ?? string.Empty,
                 filterViewModel.SortColumn, filterViewModel.SortOrder });

                if (dataset == null || dataset?.Tables.Count == 0)
                {
                    return null;
                }
                PaginationResponseViewModel<List<Client>> paginationResponseViewModel = new();
                paginationResponseViewModel.results = await _exec.DataTableToListAsync<Client>(dataset.Tables[0]);
                paginationResponseViewModel.TotalCount = Convert.ToInt32(dataset.Tables[1].Rows[0][0]);
                return paginationResponseViewModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Client> GetClientDetailsById(int clientId, int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetClient",
                new string[] { "@Id", "@OptId", "@DeptID" },
                new string[] { Convert.ToString(clientId), "2", Convert.ToString(departmentId) });

                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToModelAsync<Client>(obj);

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return null;
            }
        }

        public async Task<int> AddClient(Client client)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteClient",
                new string[] { "@Name", "@Email", "@Skypeid", "@PhoneNumber", "@DepartmentId", "@BillingAddress", "@Country", "@ClientCompanyName", "@OptId" },
                new string[] { client.Name, client.Email, client.Skypeid, client.PhoneNumber, Convert.ToString(client.DepartmentId) ?? "0", client.BillingAddress, client.Country, client.ClientCompanyName, "1" });

                if (obj?.Rows?.Count > 0)
                {
                    var addClient = await _exec.DataTableToModelAsync<Client>(obj);
                    return addClient.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to add the client.");
            }
        }

        public async Task<int> UpdateClientDetails(Client client)
        {
            try
            {
                // Update code 
                var obj = await _exec.Get_DataTableAsync("ExecuteClient",
                new string[] { "@Id", "@Name", "@Email", "@Skypeid", "@PhoneNumber", "@DepartmentId", "@BillingAddress", "@Country", "@OptId", "@ClientCompanyName" },
                new string[] { client.Id.ToString(), client.Name, client.Email, client.Skypeid, client.PhoneNumber, client.DepartmentId.ToString(), client.BillingAddress, client.Country, "2", client.ClientCompanyName });

                if (obj?.Rows?.Count > 0)
                {
                    var updateClientDetails = await _exec.DataTableToModelAsync<Client>(obj);
                    return updateClientDetails.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while trying to update the client details.");
            }
        }

        public async Task<bool> RemoveClient(int clientId)
        {
            try
            {
                // Call a method, possibly from a data access library, to execute a database query.
                var obj = await _exec.Get_DataTableAsync("ExecuteClient",
                new string[] { "@Id", "@OptID" },
                new string[] { Convert.ToString(clientId) ?? string.Empty, "3" });

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Internal Server Error. An error occurred while deleting client .");
            }
        }

        public async Task<List<DropDownResponse<string>>> GetClientNameByTL(string teamLeaderId, int departmentId)
        {
            try
            {
                var reportsList = await _exec.Get_DataSetAsync("GetClientNameByTeamLead",
                    new string[] { "@TeamLeaderId", "@DepartmentId" },
                    new string[] { Convert.ToString(teamLeaderId), Convert.ToString(departmentId) });

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

        public async Task<Client> GetClientName(int departmentId, string name)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteClient",
                    new string[] { "@Name", "@DepartmentId", "@OptID" },
                    new string[] { Convert.ToString(name), departmentId.ToString(), "4" });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<Client>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<DropDownResponse<string>>> GetClientListByDepartment(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteClient",
                    new string[] { "@DepartmentId", "@OptID" },
                    new string[] { Convert.ToString(departmentId), "5" });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToListAsync<DropDownResponse<string>>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<Client> GetClientByPhoneNumber(int departmentId, string phoneNumber)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteClient",
                    new string[] { "@PhoneNumber", "@DepartmentId", "@OptID" },
                    new string[] { Convert.ToString(phoneNumber), departmentId.ToString(), "7" });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<Client>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Client> GetClientByEmail(int departmentId, string email)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteClient",
                    new string[] { "@Email", "@DepartmentId", "@OptID" },
                    new string[] { Convert.ToString(email), departmentId.ToString(), "6" });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<Client>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Client> GetClient(int clientId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetClient",
                new string[] { "@Id", "@OptId",  },
                new string[] { Convert.ToString(clientId), "5" });

                if (obj?.Rows?.Count > 0)
                    return await _exec.DataTableToModelAsync<Client>(obj);

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return null;
            }
        }
    }
}
