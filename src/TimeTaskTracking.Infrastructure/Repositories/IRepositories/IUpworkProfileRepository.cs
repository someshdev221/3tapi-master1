using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface IUpworkProfileRepository
{
    Task<PaginationResponseViewModel<List<UpworkProfile>>> GetUpworkProfiles(FilterViewModel filterViewModel, int profileType);
    Task<UpworkProfile> GetUpworkProfileById(int id);
    Task<int> AddUpworkProfile(UpworkProfile UpworkProfile);
    Task<int> UpdateUpworkProfile(UpworkProfile UpworkProfile);
    Task<bool> DeleteUpworkProfile(int id);
    Task<UpworkProfile>GetAllUpworkProfileName(string name);
    Task<List<DropDownResponse<int>>> GetUpworkProfilesByDepartment(int departmentId);
}
