using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Core.Services
{
    public class CommonService : ICommonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public CommonService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }
        public async Task<ResponseModel<int?>> AddToDoList(ToDoListViewModel model)
        {
            var response = new ResponseModel<int?>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var checkEmployeeExists = new Register();

            if (claims.Role == "HOD")
            {
                var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(model.AssignedToId, claims_HOD.DepartmentId, string.Empty);
            }
            if (claims.Role == "Admin")
            {
                checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(model.AssignedToId, 0, string.Empty);
            }
            else
            {
                checkEmployeeExists = await _unitOfWork.Employee.GetEmployeeById(model.AssignedToId, claims.DepartmentId, string.Empty);
            }

            if (checkEmployeeExists == null || checkEmployeeExists.IsActive != 1)
            {
                response.Message.Add(SharedResources.InvalidEmployee);
                response.Model = 0;
                return response;
            }

            var forDate = DateTime.Now.Date;

            var endTime = forDate.AddDays(1).AddTicks(-1);

            var checkForExisitingToDo = await _unitOfWork.CommonRepository.GetToDoList(model.AssignedToId, Convert.ToString(forDate));

            if (checkForExisitingToDo == null && model.ToDo == string.Empty)
            {
                response.Message.Add(SharedResources.ToDoCannotBeEmptyOrNull);
                return response;
            }

            if (claims.Role == "HOD")
            {
                if (checkForExisitingToDo != null && model.ToDo == string.Empty)
                {
                    var deleteToDo = await _unitOfWork.CommonRepository.DeleteToDoById(checkForExisitingToDo.Id);
                    if (deleteToDo)
                    {
                        response.Model = 1;
                        response.Message.Add(SharedResources.ToDoAddedSuccessfully);
                        return response;
                    }
                    else
                        response.Message.Add(SharedResources.ErrorWhileSavingToDo);
                    return response;
                }

                if (checkForExisitingToDo != null)
                {
                    return await UpdateToDoList(model, claims.LoggedInUserId, checkForExisitingToDo.Id, endTime, response);
                }
                return await SaveToDoList(model, claims.LoggedInUserId, endTime, response);
            }

            if (claims.Role == "Admin")
            {
                if (checkForExisitingToDo != null && model.ToDo == string.Empty)
                {
                    var deleteToDo = await _unitOfWork.CommonRepository.DeleteToDoById(checkForExisitingToDo.Id);
                    if (deleteToDo)
                    {
                        response.Model = 1;
                        response.Message.Add(SharedResources.ToDoAddedSuccessfully);
                        return response;
                    }
                    else
                        response.Message.Add(SharedResources.ErrorWhileSavingToDo);
                    return response;
                }

                if (checkForExisitingToDo != null)
                {
                    return await UpdateToDoList(model, claims.LoggedInUserId, checkForExisitingToDo.Id, endTime, response);
                }
                return await SaveToDoList(model, claims.LoggedInUserId, endTime, response);
            }

            if (claims.Role == "Project Manager" && claims.UserId != model.AssignedToId)
            {
                if (checkEmployeeExists.TeamAdminId != claims.UserId)
                {
                    response.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                    return response;
                }

                if (checkForExisitingToDo != null && model.ToDo == string.Empty)
                {
                    var deleteToDo = await _unitOfWork.CommonRepository.DeleteToDoById(checkForExisitingToDo.Id);
                    if (deleteToDo)
                    {
                        response.Model = 1;
                        response.Message.Add(SharedResources.ToDoAddedSuccessfully);
                        return response;
                    }
                    else
                        response.Message.Add(SharedResources.ErrorWhileSavingToDo);
                    return response;
                }

                if (checkForExisitingToDo != null)
                {
                    return await UpdateToDoList(model, claims.UserId, checkForExisitingToDo.Id, endTime, response);
                }
                return await SaveToDoList(model, claims.UserId, endTime, response);
            }

            if (claims.Role == "Team Lead" && claims.UserId != model.AssignedToId)
            {
                if (checkEmployeeExists.RoleName == "Project Manager")
                {
                    response.Message.Add(SharedResources.CannotAssignToManager);
                    return response;
                }

                var teamLeadEmployeList = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.UserId);
                if (!teamLeadEmployeList.Any(x => x.Id == model.AssignedToId))
                {
                    response.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                    return response;
                }

                if (checkForExisitingToDo != null && model.ToDo == string.Empty)
                {
                    var deleteToDo = await _unitOfWork.CommonRepository.DeleteToDoById(checkForExisitingToDo.Id);
                    if (deleteToDo)
                    {
                        response.Model = 1;
                        response.Message.Add(SharedResources.ToDoAddedSuccessfully);
                        return response;
                    }
                    else
                        response.Message.Add(SharedResources.ErrorWhileSavingToDo);
                    return response;
                }

                if (checkForExisitingToDo != null)
                {
                    return await UpdateToDoList(model, claims.UserId, checkForExisitingToDo.Id, endTime, response);
                }
                return await SaveToDoList(model, claims.UserId, endTime, response);
            }

            if (checkEmployeeExists.Id == claims.UserId)
            {
                if (checkForExisitingToDo != null && model.ToDo == string.Empty)
                {
                    var deleteToDo = await _unitOfWork.CommonRepository.DeleteToDoById(checkForExisitingToDo.Id);
                    if (deleteToDo)
                    {
                        response.Model = 1;
                        response.Message.Add(SharedResources.ToDoAddedSuccessfully);
                        return response;
                    }
                    else
                        response.Message.Add(SharedResources.ErrorWhileSavingToDo);
                    return response;
                }

                if (checkForExisitingToDo != null)
                {
                    return await UpdateToDoList(model, claims.UserId, checkForExisitingToDo.Id, endTime, response);
                }
                return await SaveToDoList(model, claims.UserId, endTime, response);
            }

            response.Message.Add(SharedResources.YouCanNotSubmitForOthers);
            return response;
        }


        public async Task<ResponseModel<List<dynamic>>> GetToDoListByManagerAndTeamLead(ToDoRequestViewModel model)
        {
            var response = new ResponseModel<List<dynamic>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var forDate = DateTime.Now.Date;
            var endTime = forDate.AddDays(1).AddTicks(-1);

            if (claims.Role == "Project Manager")
            {
                model.TeamAdminId = claims.UserId;
                model.DepartmentId = claims.DepartmentId;
                var getToDoListByManager = await _unitOfWork.CommonRepository.GetToDoListByManager(endTime, model);
                response.Model = getToDoListByManager.Cast<dynamic>().ToList();
            }

            if (claims.Role == "Team Lead")
            {
                var getToDoListByTeamLead = await _unitOfWork.CommonRepository.GetToDoListByTeamLead(endTime, claims.UserId);
                response.Model = getToDoListByTeamLead.Cast<dynamic>().ToList();
            }

            if (claims.Role == "HOD")
            {
                var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                model.DepartmentId = claims_HOD.DepartmentId;
                var getToDoListByHOD = await _unitOfWork.CommonRepository.GetToDoListByHODAndAdmin(endTime, model);
                response.Model = getToDoListByHOD.Cast<dynamic>().ToList();
            }

            if (claims.Role == "Admin")
            {
                if (model.DepartmentId == null)
                {
                    response.Message.Add(SharedResources.DepartmentIdIsRequired);
                    return response;
                }
                else
                {
                    var getToDoListByAdmin = await _unitOfWork.CommonRepository.GetToDoListByHODAndAdmin(endTime, model);
                    response.Model = getToDoListByAdmin.Cast<dynamic>().ToList();
                }
            }
            return response;
        }


        public async Task<ResponseModel<ToDoListModel>> GetToDoListByEmployee()
        {
            var response = new ResponseModel<ToDoListModel>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var forDate = DateTime.Now.Date;
            var endTime = forDate.AddDays(1).AddTicks(-1);
            var toDoList = await _unitOfWork.CommonRepository.GetToDoListByEmployeeAsync(endTime, claims.UserId);
            if (toDoList != null)
            {
                response.Model = toDoList;
            }
            else
                response.Model = new ToDoListModel();
            return response;
        }

        //public async Task<ResponseModel<bool>> DeleteToDoList()
        //{
        //    var response = new ResponseModel<bool>();
        //    var today = DateTime.Today;
        //    var currentDateEndTime = today.AddDays(1).AddTicks(-1);


        //    var checkToDoListExists = await _unitOfWork.CommonRepository.ToDoListExistsForTheDate(currentDateEndTime);
        //    if (checkToDoListExists == null)
        //    {
        //        response.Message.Add(SharedResources.NoToDoListFound);
        //        return response;
        //    }

        //    var deleteToDoListForYesterday = await _unitOfWork.CommonRepository.DeleteToDoListAsync(currentDateEndTime);
        //    if (deleteToDoListForYesterday)
        //    {
        //        response.Message.Add(SharedResources.ToDoDeletedSuccessfully);
        //        response.Model = deleteToDoListForYesterday;
        //    }
        //    else
        //    {
        //        response.Message.Add(SharedResources.SomethingWentWrongWhileDeletingToDo);
        //        response.Model = deleteToDoListForYesterday;
        //    }

        //    return response;
        //}

        private async Task<ResponseModel<int?>> SaveToDoList(ToDoListViewModel model, string AssignedById, DateTime endDate, ResponseModel<int?> response)
        {

            var SubmitedOn = DateTime.Now.Date;

            var addToDo = await _unitOfWork.CommonRepository.AddToDoList(model, AssignedById, SubmitedOn, endDate);
            if (addToDo == 0)
            {
                response.Message.Add(SharedResources.ErrorWhileSavingToDo);
            }
            else
            {
                response.Message.Add(SharedResources.ToDoAddedSuccessfully);
                response.Model = addToDo;
            }

            return response;
        }

        private async Task<ResponseModel<int?>> UpdateToDoList(ToDoListViewModel model, string AssignedById, int TaskId, DateTime endDate, ResponseModel<int?> response)
        {

            var SubmitedOn = DateTime.Now.Date;

            var addToDo = await _unitOfWork.CommonRepository.UpdateToDoList(model, AssignedById, SubmitedOn, TaskId, endDate);
            if (addToDo == 0)
            {
                response.Message.Add(SharedResources.ErrorWhileSavingToDo);
            }
            else
            {
                response.Message.Add(SharedResources.ToDoAddedSuccessfully);
                response.Model = addToDo;
            }

            return response;
        }

        //public async Task<ResponseModel<List<dynamic>>> GetBioMatricAttendanceLogs(BioMatricRequestViewModel model)
        //{
        //    var response = new ResponseModel<List<dynamic>>();
        //    var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

        //    string employeeCodeList = string.Empty;
        //    var employeeList = new List<EmployeeDetailsViewModel>();

        //    if (claims.Role == "Employee" || claims.Role == "BDM")
        //    {
        //        var getEmployeeDetails = await _unitOfWork.Employee.GetEmployeeById(claims.UserId, claims.DepartmentId, string.Empty);
        //        var employeeDetails = new EmployeeDetailsViewModel
        //        {
        //            EmployeeNumber = getEmployeeDetails.EmployeeNumber,
        //        };

        //        employeeList.Add(employeeDetails);
        //    }
        //    if (claims.Role == "Team Lead")
        //    {
        //        employeeList = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.UserId);
        //    }

        //    if (claims.Role == "Project Manager")
        //    {
        //        employeeList = await _unitOfWork.Employee.GetEmployeeListByManager(claims.UserId, claims.DepartmentId);
        //    }

        //    if (claims.Role == "HOD")
        //    {
        //        var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
        //        employeeList = await _unitOfWork.Employee.GetEmployeeListByDepartmentId(claims_HOD.DepartmentId);
        //        if (model.TeamAdminId == null)
        //        {
        //            employeeList = await _unitOfWork.Employee.GetEmployeeListByDepartmentId(model.DepartmentId);
        //        }
        //        else
        //        {
        //            employeeList = await _unitOfWork.Employee.GetEmployeeListByManager(model.TeamAdminId, claims_HOD.DepartmentId);
        //        }
        //    }

        //    if (claims.Role == "Admin" || claims.Role == "HR")
        //    {
        //        if (model.DepartmentId == 0)
        //        {
        //            response.Message.Add(SharedResources.DepartmentIdIsRequired);
        //            return response;
        //        }
        //        if (model.TeamAdminId == null)
        //        {
        //            employeeList = await _unitOfWork.Employee.GetEmployeeListByDepartmentId(model.DepartmentId);
        //        }
        //        else
        //        {
        //            employeeList = await _unitOfWork.Employee.GetEmployeeListByManager(model.TeamAdminId, model.DepartmentId);
        //        }
        //    }

        //    if (employeeList?.Any()==false)
        //    {
        //        response.Message.Add(SharedResources.NoEmployeeFound);
        //        response.Model = new List<dynamic> { };
        //        return response;
        //    }

        //    var employeeDetailsList = employeeList.Select(e => new EmployeeAttendanceAndPunchViewModel
        //    {
        //        Id = e.Id,
        //        EmployeeNumber = e.EmployeeNumber,
        //        EmployeeName = $"{e.FirstName} {e.LastName}"
        //    }).ToList();

        //    employeeCodeList = string.Join(", ", employeeDetailsList.Select(e => e.EmployeeNumber));

        //    var employeeAttendanceList = await _unitOfWork.CommonRepository.GetAttendanceByEmployeeIds(employeeCodeList, model.Month, model.Year);

        //    if (employeeAttendanceList?.Any() == true)
        //    {
        //        response.Model = employeeAttendanceList.Cast<dynamic>().ToList();
        //    }
        //    else
        //    {
        //        response.Message.Add(SharedResources.NoAttendanceLogFound);
        //        response.Model = new List<dynamic> { };
        //    }

        //    return response;
        //}


        public async Task<ResponseModel<List<dynamic>>> GetBioMatricAttendanceLogs(BioMatricRequestViewModel model)
        {
            var response = new ResponseModel<List<dynamic>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, model.DepartmentId, model.TeamAdminId);

            var employeeDetailsList = await _unitOfWork.Employee.GetEmployeeListForBioMatric(
                                        string.IsNullOrEmpty(model.EmployeeId) ? string.Empty : claims.LoggedInUserId,
                                        model.TeamLeadId,
                                        string.IsNullOrEmpty(model.TeamAdminId) ? string.Empty : model.TeamAdminId,  // Pass empty string if TeamAdminId is null, otherwise pass TeamAdminId
                                        claims.DepartmentId, model.PageNumber, model.PageSize, model.SearchValue);
            if (employeeDetailsList == null)
                response.Message.Add(SharedResources.EmployeesNotFound);
            else
            {
                var employeeCodeList = string.Join(", ", employeeDetailsList.Select(e => e.EmployeeNumber));
                var employeeAttendanceDetailList = await _unitOfWork.CommonRepository.GetAttendanceLogAsync(employeeCodeList, model.Month, model.Year);
                if (employeeAttendanceDetailList?.Any() == true)
                    response.Model = employeeAttendanceDetailList
                .OrderBy(e => e.EmployeeCode) // Sort by Employee Number
                .Cast<dynamic>()
                .ToList();
                else
                {
                    response.Message.Add(SharedResources.NoAttendanceLogFound);
                    response.Model = new List<dynamic>();
                }
            }

            return response;
        }


        //public async Task<ResponseModel<List<dynamic>>> GetBioMatricAttendanceLogs(BioMatricRequestViewModel model)
        //{
        //    var response = new ResponseModel<List<dynamic>>();
        //    var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, model.DepartmentId, model.TeamAdminId);

        //    var employeeList = new List<EmployeeDetailsViewModel>();

        //    var getTeamLeadDetails = new EmployeeDetailsViewModel();

        //    switch (claims.Role)
        //    {
        //        case "Employee":
        //        case "BDM":
        //            var employee = await _unitOfWork.Employee.GetEmployeeById(claims.UserId, claims.DepartmentId, string.Empty);
        //            employeeList.Add(new EmployeeDetailsViewModel
        //            {
        //                EmployeeNumber = employee.EmployeeNumber,
        //            });
        //            break;

        //        case "Team Lead":
        //            employeeList = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.UserId);
        //          getTeamLeadDetails = await _unitOfWork.Employee.GetTeamLeadDetails(claims.UserId);
        //            employeeList.Add(getTeamLeadDetails);
        //            break;

        //        case "Project Manager":
        //            if (model.TeamLeadId == null)
        //            {
        //                response.Message.Add(SharedResources.TeamLeadIdIsRequired);
        //                return response;
        //            }
        //            else
        //            {
        //                getTeamLeadDetails = await _unitOfWork.Employee.GetTeamLeadDetails(model.TeamLeadId);
        //                if (getTeamLeadDetails == null)
        //                {
        //                    response.Message.Add(SharedResources.TeamLeadNotFound);
        //                    return response;
        //                }
        //                if (getTeamLeadDetails.DepartmentId != claims.DepartmentId)
        //                {
        //                    response.Message.Add(SharedResources.InValidTeamLeadId);
        //                    return response;
        //                }
        //                else
        //                {
        //                    employeeList = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(model.TeamLeadId);
        //                    employeeList.Add(getTeamLeadDetails);
        //                }
        //            }
        //            break;

        //        case "HOD":
        //            var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
        //            employeeList = model.TeamAdminId == null
        //                ? await _unitOfWork.Employee.GetEmployeeListByDepartmentId(claims_HOD.DepartmentId)
        //                : await _unitOfWork.Employee.GetEmployeeListByManager(model.TeamAdminId, claims_HOD.DepartmentId);
        //            break;

        //        case "Admin":
        //        case "HR":
        //            if (model.DepartmentId == 0)
        //            {
        //                response.Message.Add(SharedResources.DepartmentIdIsRequired);
        //                return response;
        //            }

        //            if (model.TeamLeadId != null)
        //            {
        //                employeeList = await _unitOfWork.Employee.GetEmployeeDetailsByTeamleader(claims.UserId);
        //                getTeamLeadDetails = await _unitOfWork.Employee.GetTeamLeadDetails(claims.UserId);
        //                employeeList.Add(getTeamLeadDetails);
        //            }
        //            else
        //            {
        //                employeeList = model.TeamAdminId == null
        //                ? await _unitOfWork.Employee.GetEmployeeListByDepartmentId(model.DepartmentId)
        //                : await _unitOfWork.Employee.GetEmployeeListByManager(model.TeamAdminId, model.DepartmentId);
        //            }
        //            break;
        //    }

        //    if (employeeList?.Any() == false)
        //    {
        //        response.Message.Add(SharedResources.NoEmployeeFound);
        //        response.Model = new List<dynamic>();
        //        return response;
        //    }

        //    var employeeDetailsList = employeeList.Select(e => new EmployeeAttendanceAndPunchViewModel
        //    {
        //        Id = e.Id,
        //        EmployeeNumber = e.EmployeeNumber,
        //        EmployeeName = $"{e.FirstName} {e.LastName}"
        //    }).ToList();

        //    var employeeCodeList = string.Join(", ", employeeDetailsList.Select(e => e.EmployeeNumber));

        //    var employeeAttendanceDetailList = await _unitOfWork.CommonRepository.GetAttendanceLogAsync(employeeCodeList, model.Month, model.Year);

        //    if (employeeAttendanceDetailList?.Any() == true)
        //    {
        //        response.Model = employeeAttendanceDetailList.Cast<dynamic>().ToList();
        //    }
        //    else
        //    {
        //        response.Message.Add(SharedResources.NoAttendanceLogFound);
        //        response.Model = new List<dynamic>();
        //    }

        //    return response;
        //}
    }
}
