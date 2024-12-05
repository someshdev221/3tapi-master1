using AutoMapper;
using Microsoft.AspNetCore.Http;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services;

public class UpworkProfileService : IUpworkProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UpworkProfileService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<ResponseModel<PaginationResponseViewModel<List<UpworkProfileDto>>>> GetUpworkProfiles(FilterViewModel FilterViewModel,int ProfileType)
    {
        var response = new ResponseModel<PaginationResponseViewModel<List<UpworkProfileDto>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_httpContextAccessor.HttpContext, FilterViewModel.DepartmentId, string.Empty);
        FilterViewModel.DepartmentId = claims.DepartmentId;
        var upworkProfiles = await _unitOfWork.UpworkProfile.GetUpworkProfiles(FilterViewModel, ProfileType);
        if (upworkProfiles?.results != null)
            response.Model = new PaginationResponseViewModel<List<UpworkProfileDto>>
            {
                results = _mapper.Map<List<UpworkProfileDto>>(upworkProfiles.results),
                TotalCount = upworkProfiles.TotalCount
            };
        else
            response.Message.Add(SharedResources.UpworkProfilesNotFound);
        return response;
    }
    public async Task<ResponseModel<UpworkProfileDto>> GetUpworkProfileById(int id)
    {
        var result = new ResponseModel<UpworkProfileDto>();
        var claims = await SharedResources.GetDepartmentFromClaims(_httpContextAccessor.HttpContext, 0,string.Empty);
        var upworkProfile = await _unitOfWork.UpworkProfile.GetUpworkProfileById(id);
        if (upworkProfile == null)
            result.Message.Add(SharedResources.ProfileNotFound);
        else
            result.Model = _mapper.Map<UpworkProfileDto>(upworkProfile);
        return result;
    }
    public async Task<ResponseModel<int>> AddUpworkProfile(UpworkDetailsProfileDto upworkProfile)
    {
        var result = new ResponseModel<int>();

        // Validate ProfileType
        if (!Enum.IsDefined(typeof(ProfileType), upworkProfile.ProfileType))
        {
            result.Message.Add(SharedResources.InvalidProfileType);
            return result;
        }

        var upworkProfileData = await _unitOfWork.UpworkProfile.AddUpworkProfile(_mapper.Map<UpworkProfile>(upworkProfile));
        if (upworkProfileData == 0)
        {
            result.Message.Add(SharedResources.ErrorWhileSaveUpworkProfile);
        }
        else
        {
            result.Message.Add(SharedResources.SaveMessage);
        }
        result.Model = upworkProfileData;
        return result;
    }

    public async Task<ResponseModel<int>> UpdateUpworkProfile(UpworkDetailsProfileDto upworkProfile)
    {
        var result = new ResponseModel<int>();

        // Validate ProfileType
        if (!Enum.IsDefined(typeof(ProfileType), upworkProfile.ProfileType))
        {
            result.Message.Add(SharedResources.InvalidProfileType);
            return result;
        }

        var upworkProfileData = await _unitOfWork.UpworkProfile.UpdateUpworkProfile(_mapper.Map<UpworkProfile>(upworkProfile));

        if (upworkProfileData == 0)
        {
            result.Message.Add(SharedResources.ErrorWhileUpdateUpworkProfile);
        }
        else
        {
            result.Message.Add(SharedResources.UpdatedMessage);
        }

        result.Model = upworkProfileData;
        return result;
    }

    public async Task<ResponseModel<bool>> DeleteUpworkProfile(int id)
    {
        var result = new ResponseModel<bool>();
        var checkUpworkProfileAssignedToAnyProject = await _unitOfWork.EmployeeStatus.GetEmployeeStatusByUpworkProfileId(id);
        if (checkUpworkProfileAssignedToAnyProject)
        {
            result.Message.Add(SharedResources.ErrorwhileDeletingThisUpworkProfile);
            return result;  
        }
        var upworkProfileData = await _unitOfWork.UpworkProfile.DeleteUpworkProfile(id);
        if (!upworkProfileData)
            result.Message.Add(SharedResources.ErrorWhileUpdateUpworkProfile);
        else
            result.Message.Add(SharedResources.DeletedMessage);
        result.Model = upworkProfileData;
        return result;
    }

    public async Task<ResponseModel<UpworkProfileDto>> GetUpworkProfileByName(string name)
    {
        var result = new ResponseModel<UpworkProfileDto>();
        var upworkProfile = await _unitOfWork.UpworkProfile.GetAllUpworkProfileName(name);
        if (upworkProfile == null)
            result.Message.Add(SharedResources.ProfileNotFound);
        else
            result.Model = _mapper.Map<UpworkProfileDto>(upworkProfile);
        return result;
    }
    public async Task<ResponseModel<List<DropDownResponse<int>>>> GetUpworkProfilesByDepartment()
    {
        var result = new ResponseModel<List<DropDownResponse<int>>>();
        var claims = await SharedResources.GetDepartmentFromClaims(_httpContextAccessor.HttpContext, 0,string.Empty);
        var upworkProfiles = await _unitOfWork.UpworkProfile.GetUpworkProfilesByDepartment(claims.DepartmentId);
        if (upworkProfiles?.Any() != true)
            result.Message.Add(SharedResources.UpworkProfilesNotFound);
        else
            result.Model = upworkProfiles;
        return result;
    }
}
