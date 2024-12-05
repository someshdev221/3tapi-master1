
using Org.BouncyCastle.Crypto.Engines;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices;

public interface IUpworkProfileService
{
    Task<ResponseModel<PaginationResponseViewModel<List<UpworkProfileDto>>>> GetUpworkProfiles(FilterViewModel FilterViewModel, int ProfileType);
    Task<ResponseModel<UpworkProfileDto>> GetUpworkProfileById(int id);
    Task<ResponseModel<UpworkProfileDto>> GetUpworkProfileByName(string name);
    Task<ResponseModel<int>> AddUpworkProfile(UpworkDetailsProfileDto upworkProfile);
    Task<ResponseModel<int>> UpdateUpworkProfile(UpworkDetailsProfileDto upworkProfile);
    Task<ResponseModel<bool>> DeleteUpworkProfile(int id);
    Task<ResponseModel<List<DropDownResponse<int>>>> GetUpworkProfilesByDepartment();
}
