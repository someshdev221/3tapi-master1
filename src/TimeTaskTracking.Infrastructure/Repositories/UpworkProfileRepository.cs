using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class UpworkProfileRepository : IUpworkProfileRepository
{
    private readonly ExecuteProcedure _exec;
    public UpworkProfileRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }
    public async Task<PaginationResponseViewModel<List<UpworkProfile>>> GetUpworkProfiles(FilterViewModel filterViewModel, int ProfileType)
    {
        try
        {
            var dataset = await _exec.Get_DataSetAsync("GetProfiles",
            new string[] { "@DepartmentId", "@PageNumber", "@PageSize", "@SearchKeyword", "@SortColumn", "@SortOrder", "@ProfileType", "@OptId" },
            new string[] { Convert.ToString(filterViewModel.DepartmentId) ?? "0", Convert.ToString(filterViewModel.PageNumber)?? string.Empty, Convert.ToString(filterViewModel.PageSize)?? string.Empty, Convert.ToString(filterViewModel.SearchValue) ?? string.Empty,
                 Convert.ToString(filterViewModel.SortColumn) ?? string.Empty, Convert.ToString(filterViewModel.SortOrder) ?? string.Empty, ProfileType==0?null:Convert.ToString(ProfileType) ,"1" });

            if (dataset?.Tables.Count == 0 || dataset == null)
            {
                return null;
            }
            PaginationResponseViewModel<List<UpworkProfile>> paginationResponseViewModel = new();
            paginationResponseViewModel.results = await _exec.DataTableToListAsync<UpworkProfile>(dataset.Tables[0]);
            paginationResponseViewModel.TotalCount = Convert.ToInt32(dataset.Tables[1].Rows[0][0]);
            return paginationResponseViewModel;
        }
        catch (Exception ex)
        {
            return null;
        }

    }
    public async Task<UpworkProfile> GetUpworkProfileById(int id)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetProfiles",
                new string[] { "@Id", "@OptId", "@DepartmentId" },
                new string[] { Convert.ToString(id), "2","0"});

            if (obj != null && obj.Rows.Count > 0)
            {
                return await _exec.DataTableToModelAsync<UpworkProfile>(obj);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<int> AddUpworkProfile(UpworkProfile UpworkProfile)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteProfiles",
            new string[] { "@Name", "@Description", "@DepartmentId", "@ProfileType", "@OptId" },
            new string[] { Convert.ToString(UpworkProfile.Name), Convert.ToString(UpworkProfile.Description), Convert.ToString(UpworkProfile.DepartmentId), Convert.ToString(UpworkProfile.ProfileType), "1" });
            if (obj?.Rows?.Count > 0)
            {
                var UpworkProfileData = await _exec.DataTableToModelAsync<UpworkProfile>(obj);
                return UpworkProfileData.Id;
            }
            return 0;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<int> UpdateUpworkProfile(UpworkProfile UpworkProfile)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteProfiles",
             new string[] { "@Id", "@Name", "@Description", "@DepartmentId", "@ProfileType", "@OptId" },
            new string[] { Convert.ToString(UpworkProfile.Id), Convert.ToString(UpworkProfile.Name), Convert.ToString(UpworkProfile.Description), Convert.ToString(UpworkProfile.DepartmentId) ,Convert.ToString(UpworkProfile.ProfileType), "2" });
            if (obj?.Rows?.Count > 0)
            {
                var upworkProfileData = await _exec.DataTableToModelAsync<UpworkProfile>(obj);
                return upworkProfileData.Id;
            }
            return 0;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<bool> DeleteUpworkProfile(int id)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteProfiles",
                new string[] { "@Id", "@OptID" },
                new string[] { Convert.ToString(id), "3" });

            if (obj?.Rows?.Count > 0)
            {
                var status = Convert.ToInt32(obj.Rows[0]["Status"]);
                return status == 1;
            }

            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<UpworkProfile> GetAllUpworkProfileName(string name)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetProfiles",
                new string[] { "@Name", "@OptID" },
                new string[] { Convert.ToString(name), "4" });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToModelAsync<UpworkProfile>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<List<DropDownResponse<int>>> GetUpworkProfilesByDepartment(int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataSetAsync("GetProfiles",
                new string[] { "@DepartmentId", "@OptID" },
                new string[] { Convert.ToString(departmentId), "5" });

            if (obj?.Tables?.Count > 0)
            {
                return await _exec.DataTableToListAsync<DropDownResponse<int>>(obj.Tables[0]);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
