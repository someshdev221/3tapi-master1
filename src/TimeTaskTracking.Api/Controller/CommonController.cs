using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonService _commonService;
        private readonly IValidator<ToDoListViewModel> _validator;
        public CommonController(ICommonService commonService, IValidator<ToDoListViewModel> validator)
        {
            _commonService = commonService;
            _validator = validator;
        }
        [HttpPost("AddToDo")]
        [Authorize]
        public async Task<IActionResult> AddToDo([FromBody] ToDoListViewModel model)
        {
            var results = await _validator.ValidateAsync(model);

            if (!results.IsValid)
                return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

            var addNewToDo = await _commonService.AddToDoList(model);
            if (addNewToDo.Model == 0)
            {
                addNewToDo.Model = null;
                return NotFound(addNewToDo);
            }

            if (addNewToDo.Model == null)
                return BadRequest(addNewToDo);

            if (addNewToDo.Model == 1)
            {
                addNewToDo.Model = null;
                return Ok(addNewToDo);
            }
            return Ok(addNewToDo);
        }

        [HttpGet("GetToDoListByManagerAndTeamLead")]
        [Authorize("TeamLeadOrManagerOrAdminOrHOD")]
        public async Task<IActionResult> GetToDoListByManagerAndTeamLead([FromQuery]ToDoRequestViewModel model)
        {
            var toDoList = await _commonService.GetToDoListByManagerAndTeamLead(model);
            if (toDoList.Model == null)
            {
                return BadRequest(toDoList);
            }
            return Ok(toDoList);
        }

        [HttpGet("GetToDoListByEmployee")]
        [Authorize("EmployeeOrTeamLeadOrManagerOrBDM")]
        public async Task<IActionResult> GetToDoListByEmployee()
        {
            var toDoList = await _commonService.GetToDoListByEmployee();
            return Ok(toDoList);
        }

        [HttpGet("GetBioMatricAttendanceLogs")]
        [Authorize("ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHODOrHR")]
        public async Task<IActionResult> GetBioMatricAttendanceLogs([FromQuery] BioMatricRequestViewModel model)
        {
            var result = await _commonService.GetBioMatricAttendanceLogs(model);
            if (result.Model == null )
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //[HttpDelete("DeleteToDoList")]
        //[Authorize]
        //public async Task<IActionResult> DeleteToDoList()
        //{
        //    var result = await _commonService.DeleteToDoList();
        //    if(result.Model == false)
        //    {
        //        return NotFound(result);
        //    }
        //    return Ok(result);
        //}
    }
}
