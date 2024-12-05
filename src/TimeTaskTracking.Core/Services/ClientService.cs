using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public ClientService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<ResponseModel<PaginationResponseViewModel<List<ClientDto>>>> GetClientsByFilter(FilterViewModel filterViewModel)
        {
            var responseModel = new ResponseModel<PaginationResponseViewModel<List<ClientDto>>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, filterViewModel.DepartmentId, string.Empty);
            filterViewModel.DepartmentId = claims.DepartmentId;
            var filteredClients = await _unitOfWork.Client.GetFilteredClients(filterViewModel);
            if (filteredClients?.results?.Any() == true)
            {
                responseModel.Model = new PaginationResponseViewModel<List<ClientDto>>
                {
                    results = _mapper.Map<List<ClientDto>>(filteredClients.results),
                    TotalCount = filteredClients.TotalCount
                };
            }
            else
            {
                responseModel.Message.Add(SharedResources.ClientsNotFound);
            }
            return responseModel;
        }

        public async Task<ResponseModel<ClientDto>> GetClientById(int clientId, int departmentId)
        {
            var response = new ResponseModel<ClientDto>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            departmentId = claims.DepartmentId;
            var clientDetailsEntity = await _unitOfWork.Client.GetClientDetailsById(clientId, departmentId);
            if (clientDetailsEntity == null)
                response.Message.Add(SharedResources.ClientNotFound);
            else
                response.Model = _mapper.Map<ClientDto>(clientDetailsEntity);
            return response;
        }

        public async Task<ResponseModel<int>> AddNewClient(ClientDto client)
        {
            var response = new ResponseModel<int>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, client.DepartmentId, string.Empty);
            if (claims.Role != "Admin")
            {
            client.DepartmentId = claims.DepartmentId;
            }

            var addClient = await _unitOfWork.Client.AddClient(_mapper.Map<Client>(client));
            if (addClient == 0)
            {
                response.Message.Add(SharedResources.ErrorWhileSaving);
            }
            else
            {
                response.Message.Add(SharedResources.ClientAddedSuccessFully);
            }

            response.Model = addClient;
            return response;
        }

        public async Task<ResponseModel<int>> UpdateClient(ClientDto client)
        {
            var response = new ResponseModel<int>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, client.DepartmentId, string.Empty);
            if (claims.Role != "Admin")
            {
            client.DepartmentId = claims.DepartmentId;
            }
            var checkIfClientExisits = await _unitOfWork.Client.GetClientDetailsById(client.Id, client.DepartmentId);
            if (checkIfClientExisits.Id != 0)
            {
                var updateExisitingClient = await _unitOfWork.Client.UpdateClientDetails(_mapper.Map<Client>(client));
                response.Message.Add(SharedResources.ClientDetailsUpdatedSuccessfully);
                response.Model = updateExisitingClient;
            }
            else
            {
                response.Message.Add(SharedResources.ClientNotFound);
            }
            return response;
        }

        public async Task<ResponseModel<bool>> RemoveClient(int clientId)
        {
            var response = new ResponseModel<bool>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);          
            var checkIfClientExisits = await _unitOfWork.Client.GetClientDetailsById(clientId, claims.DepartmentId);
            if (checkIfClientExisits.Id != 0)
            {
                var checkClientIsAssignedToAnyProject = await _unitOfWork.Project.GetProjectByClient(clientId);
                if (checkClientIsAssignedToAnyProject != null)
                {
                    response.Message.Add(SharedResources.ErrorWhileDeleting);
                    return response;
                }

                if (await _unitOfWork.Project.GetProjectByClient(clientId) != null)
                {
                    response.Message.Add(SharedResources.ClientIsAssociatedWithproject);
                    return response;
                }
                var deleteStatus = await _unitOfWork.Client.RemoveClient(clientId);
                response.Message.Add(SharedResources.ClientRemoved);
                response.Model = deleteStatus;
            }
            else
            {
                response.Message.Add(SharedResources.ClientNotFound);
            }
            return response;
        }

        public async Task<ResponseModel<List<DropDownResponse<string>>>> GetClientByTeamLeader(string teamLeaderId, int departmentId)
        {
            var response = new ResponseModel<List<DropDownResponse<string>>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            departmentId = claims.DepartmentId;
            var employeeList = await _unitOfWork.Client.GetClientNameByTL(teamLeaderId, departmentId);

            if (employeeList.Count() == 0)
            {
                response.Message.Add(SharedResources.ClientNotFound);
            }
            else
            {
                response.Model = _mapper.Map<List<DropDownResponse<string>>>(employeeList);
            }

            return response;
        }

        public async Task<ResponseModel<ClientDto>> GetClientByName(int deaprtmentId , string name)
        {
            var result = new ResponseModel<ClientDto>();
            var upworkProfile = await _unitOfWork.Client.GetClientName(deaprtmentId,name);
            if (upworkProfile == null)
            {
                result.Message.Add(SharedResources.ClientNotFound);
            }
            else
            {
                result.Model = _mapper.Map<ClientDto>(upworkProfile);
            }
            return result;
        }
        public async Task<ResponseModel<List<DropDownResponse<string>>>> GetClientListByDepartment(int departmentId)
        {
            var result = new ResponseModel<List<DropDownResponse<string>>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            departmentId = claims.DepartmentId;
            var clients = await _unitOfWork.Client.GetClientListByDepartment(departmentId);
            if (clients == null)
            {
                result.Message.Add(SharedResources.ClientNotFound);
            }
            else
            {
                result.Model = clients;
            }
            return result;
        }

        public async Task<ResponseModel<bool>> GetDepartmentById(int departmentId)
        {
            var response = new ResponseModel<bool>();
            var checkDepartmentExists = await _unitOfWork.Employee.GetDepartmentExistById(departmentId);
            if (checkDepartmentExists)
            {
                response.Model = checkDepartmentExists;
            }
            else
                response.Message.Add(SharedResources.DepartmentNotFound);
            return response;
        }
        public async Task<ResponseModel<ClientDto>> GetClientByEmail(int deaprtmentId, string email)
        {
            var result = new ResponseModel<ClientDto>();
            var upworkProfile = await _unitOfWork.Client.GetClientByEmail(deaprtmentId, email);
            if (upworkProfile == null)
            {
                result.Message.Add(SharedResources.ClientNotFound);
            }
            else
            {
                result.Model = _mapper.Map<ClientDto>(upworkProfile);
            }
            return result;
        }
         public async Task<ResponseModel<ClientDto>> GetClientByPhoneNumber(int deaprtmentId , string phoneNumber)
        {
            var result = new ResponseModel<ClientDto>();
            var upworkProfile = await _unitOfWork.Client.GetClientByPhoneNumber(deaprtmentId,phoneNumber);
            if (upworkProfile == null)
            {
                result.Message.Add(SharedResources.ClientNotFound);
            }
            else
            {
                result.Model = _mapper.Map<ClientDto>(upworkProfile);
            }
            return result;
        }
        public async Task<ResponseModel<ClientDto>> GetClient(int clientId)
        {
            var response = new ResponseModel<ClientDto>();
            var clientDetailsEntity = await _unitOfWork.Client.GetClient(clientId);
            if (clientDetailsEntity == null)
                response.Message.Add(SharedResources.ClientNotFound);
            else
                response.Model = _mapper.Map<ClientDto>(clientDetailsEntity);
            return response;
        }

    }
}

