using Microsoft.AspNetCore.Mvc;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Validations;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Validators;
using TimeTaskTracking.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels.Dashboard;
using TimeTaskTracking.Core.Services;
using FluentValidation;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Shared.Enums;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ClientController : ControllerBase
    {
        public readonly IClientService _clientService;
        private readonly IValidator<ClientDto> _validator;

        public ClientController(IClientService clientService , IValidator<ClientDto> validator)
        {
            _clientService = clientService;
            _validator = validator;
        }

        [HttpGet("GetAllClientsDetail")]
        [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> GetFilteredClients([FromQuery] FilterViewModel filterViewModel)
        {
            var filteredClients = await _clientService.GetClientsByFilter(filterViewModel);       
            return Ok(filteredClients);
        }

        [HttpGet("GetClientDetailById")]
        [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> GetClientById([Required] int clientId, int departmentId)
        {
            var clientDetails = await _clientService.GetClientById(clientId, departmentId);          
            return Ok(clientDetails);
        }

        [HttpPost("AddClient")]
        [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> AddClient([FromBody] ClientDto client)
        {
            var results = await _validator.ValidateAsync(client, (options) =>
            {
                options.IncludeRuleSets(OperationType.Create.ToString(), "default");
            });
            if (!results.IsValid)
                return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

            var addNewClient = await _clientService.AddNewClient(client);
            if (addNewClient.Model == 0)
                return BadRequest(addNewClient);
            return Ok(addNewClient);
        }

        [HttpPut("UpdateClient")]
        [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> UpdateClient([FromBody] ClientDto client)
        {
            var results = await _validator.ValidateAsync(client, (options) =>
            {
                options.IncludeRuleSets(OperationType.Update.ToString(), "default");
            });
            if (!results.IsValid)
                return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));
            var updateExisitingClient = await _clientService.UpdateClient(client);
            if (updateExisitingClient.Model == 0)
                return NotFound(updateExisitingClient);
            return Ok(updateExisitingClient);
        }

        [HttpDelete("DeleteClient")]
        [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> DeleteClient([Required] int clientId)
        {
            var response = await _clientService.RemoveClient(clientId);
            if (!response.Model)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpGet("GetClientListByTeamLeadId")]
        [Authorize(Policy = "TeamLeadOrManagerOrAdmin")]
        public async Task<IActionResult> GetClientByTL([Required] string teamLeaderId,int departmentId)
        {
            var getEmployeeDetails = await _clientService.GetClientByTeamLeader(teamLeaderId, departmentId);          
            return Ok(getEmployeeDetails);
        }

        [HttpGet("GetClientListByDepartment")]
        [Authorize(Policy = "ManagerOrTeamLeadOrBDMOrAdminOrHOD")]
        public async Task<IActionResult> GetClientListByDepartment(int departmentId)
        {
            var getEmployeeDetails = await _clientService.GetClientListByDepartment(departmentId);
            return Ok(getEmployeeDetails);
        }
    }
}
