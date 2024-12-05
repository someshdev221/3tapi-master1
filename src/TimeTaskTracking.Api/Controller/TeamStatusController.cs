using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("TeamLeadOrManagerOrAdminOrHOD")]
    public class TeamStatusController : ControllerBase
    {
        private readonly ITeamStatusService _teamStatusService;
        private readonly IValidator<WarningEmailViewModel> _validator;
        public TeamStatusController(ITeamStatusService teamStatusService, IValidator<WarningEmailViewModel> validator)
        {
            _teamStatusService = teamStatusService;
            _validator = validator;
        }

        [HttpGet("GetTeamStatusByTeamLead")]
        public async Task<IActionResult> GetTeamStatusByTeamLead([Required] DateTime filterByDate,string? teamLeadId)
        {
            var teamDetails = await _teamStatusService.GetTeamStatusByTeamLead(filterByDate, teamLeadId);          
            return Ok(teamDetails);
        }

        [HttpGet("GetAttendanceStatusList")]
        public IActionResult GetProfileType()
        {
            return Ok(SharedResources.GetEnumData<AttendanceStatus, int>());
        }
        [HttpPut("UpdateAttendanceStatus")]
        public async Task<IActionResult> UpdateAttendanceStatus([FromBody] AttendanceDetails attendanceDetails)
        {

            var updateStatus = await _teamStatusService.UpdateAttendanceStatusAsync(attendanceDetails);

            if (updateStatus.Model == null)
            {
                return NotFound(updateStatus);
            }
            if (updateStatus.Model.Contains(1))
            {
                updateStatus.Model = new List<Object>();
                return BadRequest(updateStatus);
            }
            return Ok(updateStatus);
        }


        //[HttpPost("SendWarningMailToEmployees")]
        //public async Task<IActionResult> SendWarningMailToEmployees([FromBody] TeamStatusDetails<List<string>> teamStatus)
        //{
        //    var mailResult = await _teamStatusService.SendWarningMailAsync(teamStatus);
        //    if (!mailResult.Model)
        //        return NotFound(mailResult);
        //    return Ok(mailResult);
        //}

        [HttpPost("SendWarningMailToEmployees")]
        public async Task<IActionResult> SendWarningMailToEmployees([FromBody] WarningEmailViewModel model)
        {
            var results = await _validator.ValidateAsync(model);
            if (!results.IsValid)
                return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

            var sendWarningMail = await _teamStatusService.SendWarningMailToEmployees(model);
            if (sendWarningMail.Model == true)
            {
                return Ok(sendWarningMail);
            }
            return BadRequest(sendWarningMail);
        }

       
    }
}
