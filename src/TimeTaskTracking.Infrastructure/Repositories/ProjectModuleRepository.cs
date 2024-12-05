using Microsoft.Extensions.Configuration;
using System.Data;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class ProjectModuleRepository : IProjectModuleRepository
{
    private readonly ExecuteProcedure _exec;
    public ProjectModuleRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }
    public async Task<ProjectDetailsViewModel> GetProjectDetailsById(ProjectDetailFilterViewModel projectDetailFilterViewModel)
    {
        try
        {
            var moduleStatuses = projectDetailFilterViewModel.ModuleStatus != null
                                ? string.Join(",", projectDetailFilterViewModel.ModuleStatus)
                                : string.Empty;
            var paymentStatuses = projectDetailFilterViewModel.PaymentStatus != null
                                ? string.Join(",", projectDetailFilterViewModel.PaymentStatus)
                                : string.Empty;
            //var moduleStauses = string.Join(",", projectDetailFilterViewModel.ModuleStatus);
            //var paymentStauses = string.Join(",", projectDetailFilterViewModel.PaymentStatus);
            var obj = await _exec.Get_DataSetAsync("GetProjectDetails",
                new string[] { "@ProjectId", "@ModuleStatus", "@PaymentStatus", "@StartDate", "@EndDate", "@DepartmentId" },
                new string[] { Convert.ToString(projectDetailFilterViewModel.ProjectId),
                    moduleStatuses ?? string.Empty,
                    paymentStatuses ?? string.Empty,
                    Convert.ToString(projectDetailFilterViewModel.StartDate),
                    Convert.ToString(projectDetailFilterViewModel.EndDate),
                    projectDetailFilterViewModel.DepartmentId.ToString()
                });
            if (obj?.Tables?.Count > 0)
            {
                ProjectDetailsViewModel projectDetails = new();
                projectDetails.ProjectModels = await _exec.DataTableToModelAsync<ProjectViewModel>(obj?.Tables[0]);
                projectDetails.ModuleDetails = await _exec.DataTableToListAsync<ModuleDetailsViewModel>(obj.Tables[1]);
                projectDetails.BillingDetails = await _exec.DataTableToListAsync<BillingDetailsViewModel>(obj.Tables[2]);
                projectDetails.EmployeeDetails = await _exec.DataTableToListAsync<ReportsResponseViewModel>(obj.Tables[3]);
                projectDetails.ProjectAssignedDetails = await _exec.DataTableToListAsync<DropDownResponse<string>>(obj.Tables[4]);
                return projectDetails;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<ProjectViewModel> GetProjectBasicDetailsByIdAsync(int projectId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataSetAsync("GetProjectDetailsById",
               new string[] { "@OptId", "@ProjectId", "@DepartmentId" },
               new string[] { "1", projectId.ToString(), departmentId.ToString() });

            if (obj != null && obj?.Tables.Count > 0)
            {
                var projectDetails = _exec.DataTableToModel<ProjectViewModel>(obj.Tables[0]);
                if (obj.Tables.Count > 1 && obj.Tables[1].Rows.Count > 0)
                {
                   
                    projectDetails.ProjectDepartmentIds = obj.Tables[1].AsEnumerable()
                                                        .Select(row => Convert.ToInt32(row[0]))
                                                        .ToList();  // Convert to List<int>
                }

                if (obj.Tables.Count > 2 && obj.Tables[2].Rows.Count > 0)
                {
                    projectDetails.ProjectProfileDetails = obj.Tables[2].AsEnumerable()
                        .Select(row => new DropDownResponse<int>
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString()
                        })
                        .ToList();
                }

                return projectDetails;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    public async Task<PaginationResponseViewModel<List<ModuleDetailsViewModel>>> GetProjectModuleDetailsByProjectIdAsync(ProjectModuleDetailsViewModel projectModuleModel)
    {
        try
        {
            var moduleStatuses = projectModuleModel.ModuleStatus != null
                                ? string.Join(",", projectModuleModel.ModuleStatus)
                                : string.Empty;
            var paymentStatuses = projectModuleModel.PaymentStatus != null
                                ? string.Join(",", projectModuleModel.PaymentStatus)
                                : string.Empty;

            var obj = await _exec.Get_DataSetAsync("GetProjectDetailsById",
                new string[] { "@OptId", "@ProjectId", "@ModuleStatus", "@PaymentStatus", "@DepartmentId", "@PageSize", "@PageNumber", "@SearchValue" },
                new string[] { "2", projectModuleModel.ProjectId.ToString(), moduleStatuses ?? string.Empty, paymentStatuses ?? string.Empty, projectModuleModel.DepartmentId.ToString(),
                                projectModuleModel.PageSize==null?null:Convert.ToString(projectModuleModel.PageSize), projectModuleModel.PageNumber==null?null:Convert.ToString(projectModuleModel.PageNumber),
                                projectModuleModel.SearchValue==null?null: Convert.ToString(projectModuleModel.SearchValue)});

         
            if (obj != null && obj.Tables?.Count > 0)
            {
                PaginationResponseViewModel<List<ModuleDetailsViewModel>> paginationResponseViewModel = new();
                paginationResponseViewModel.results = await _exec.DataTableToListAsync<ModuleDetailsViewModel>(obj.Tables[0]);

                foreach (var module in paginationResponseViewModel.results)
                {
                    module.BilledHours = module.UpworkHours + module.FixedHours;
                }
                paginationResponseViewModel.TotalCount = Convert.ToInt32(obj.Tables[1].Rows[0][0]);
                return paginationResponseViewModel;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<BillingDetailsViewModel>> GetProjectBillingDetailsByProjectIdAsync(ProjectBillingDetailsViewModel projectBillingModel)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetProjectDetailsById",
                new string[] { "@OptId", "@ProjectId", "@StartDate", "@EndDate", "@DepartmentId" },
                new string[] { "3", projectBillingModel.ProjectId.ToString(), projectBillingModel.StartDate.ToString(), projectBillingModel.EndDate.ToString(), projectBillingModel.DepartmentId.ToString() });

            if (obj != null && obj.Rows?.Count > 0)
            {
                var projectBillingDetails = await _exec.DataTableToListAsync<BillingDetailsViewModel>(obj);
                return projectBillingDetails;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<ReportsResponseViewModel>> GetProjectEmployeeDetailsByProjectIdAsync(ProjectBillingDetailsViewModel projectBillingModel)
    {
        try
        {

            var obj = await _exec.Get_DataTableAsync("GetProjectDetailsById",
                new string[] { "@OptId", "@ProjectId", "@StartDate", "@EndDate", "@DepartmentId" },
                new string[] { "4", projectBillingModel.ProjectId.ToString(), projectBillingModel.StartDate.ToString(), projectBillingModel.EndDate.ToString(), projectBillingModel.DepartmentId.ToString() });

            if (obj != null && obj.Rows?.Count > 0)
            {
                var projectBillingDetails = await _exec.DataTableToListAsync<ReportsResponseViewModel>(obj);
                return projectBillingDetails;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<DropDownResponse<string>>> GetProjectTeamMembersByProjectIdAsync(int projectId)
    {
        try
        {

            var obj = await _exec.Get_DataTableAsync("GetProjectDetailsById",
                new string[] { "@OptId", "@ProjectId" },
                new string[] { "5", projectId.ToString() });

            if (obj != null && obj.Rows?.Count > 0)
            {
                var projectBillingDetails = await _exec.DataTableToListAsync<DropDownResponse<string>>(obj);
                return projectBillingDetails;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    /// <summary>
    /// GetProjectModule By ProjectID and Status
    /// </summary>
    /// <param name="projID"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<List<ProjectModule>> GetProjectModules(int projectId, string? status, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
                new string[] { "@ProjectId", "@Status", "@OptId", "@DepID" },
                new string[] { Convert.ToString(projectId), status, "10", departmentId.ToString() });

            if (obj != null && obj.Rows?.Count > 0)
                return await _exec.DataTableToListAsync<ProjectModule>(obj);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    /// <summary>
    /// UpdateProjectModuleById
    /// </summary>
    /// <param name="projectModule"></param>
    /// <returns></returns>
    public async Task<string> UpdateProjectModule(ProjectModule projectModule)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
                new string[] { "@ProjModuleId", "@Name", "@ProjectId", "@UpdatedBy", "@IsActiveBool", "@EstimatedHours", "@Deadline", "@ApprovalDate", "@PaymentStatus", "@ApprovedBy", "@ModuleStatus", "@ModuleNotes", "@OptID" },
                new string[] { Convert.ToString(projectModule.Id) ?? "0",projectModule.Name ?? string.Empty, Convert.ToString(projectModule.ProjectId) ?? string.Empty,  Convert.ToString(projectModule.UpdatedBy), Convert.ToString(projectModule.IsActive) ?? string.Empty,
                        Convert.ToString(projectModule.EstimatedHours) ?? string.Empty, Convert.ToString(projectModule.Deadline) ?? string.Empty,Convert.ToString(projectModule.ApprovalDate), projectModule.PaymentStatus ?? string.Empty,
                        projectModule.ApprovedBy ?? string.Empty, projectModule.ModuleStatus,projectModule.ModuleNotes, "16" });

            if (obj?.Rows?.Count > 0)
            {
                var updateProjectModule = await _exec.DataTableToModelAsync<ProjectModule>(obj);
                return updateProjectModule.Id ?? string.Empty;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> DeleteProjectModule(string moduleId)
    {
        try
        {
            var obj = _exec.Get_DataTable("UserProjects",
            new string[] { "@ProjModuleId", "@OptID" },
            new string[] { moduleId, "14" });

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

    public async Task<string> AddProjectModule(ProjectModule projectModule)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
                new string[] { "@Name", "@ProjectId", "@IsActiveBool", "@CreatedBy", "@EstimatedHours", "@Deadline", "@ApprovalDate", "@PaymentStatus", "@ApprovedBy", "@ModuleStatus", "@ModuleNotes", "@OptID" },
                new string[] { projectModule.Name ?? string.Empty, Convert.ToString(projectModule.ProjectId) ?? string.Empty,Convert.ToString(projectModule.IsActive) ?? string.Empty, Convert.ToString(projectModule.CreatedBy),
                        Convert.ToString(projectModule.EstimatedHours) ?? string.Empty, Convert.ToString(projectModule.Deadline) ?? string.Empty,Convert.ToString(projectModule.ApprovalDate), projectModule.PaymentStatus ?? string.Empty,
                        projectModule.ApprovedBy ?? string.Empty, projectModule.ModuleStatus,projectModule.ModuleNotes, "15" });

            if (obj?.Rows?.Count > 0)
            {
                var addProjectModule = await _exec.DataTableToModelAsync<ProjectModule>(obj);
                return addProjectModule.Id;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<ProjectModule> GetProjectModule(string moduleId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
            new string[] { "@ProjModuleId", "@OptId" },
            new string[] { Convert.ToString(moduleId), "20"});
            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToModelAsync<ProjectModule>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            return null;
        }
    }

    public async Task<ProjectModule> GetProjectModuleName(int projectId, string name)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
            new string[] { "@ProjectId", "@Name", "@OptId" },
            new string[] { projectId.ToString(), Convert.ToString(name), "21" });
            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToModelAsync<ProjectModule>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            return null;
        }
    }
    public async Task<List<DropDownResponse<string>>> GetProjectModulesByProject(int projectId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataSetAsync("UserProjects",
                new string[] { "@ProjectId", "@DepID", "@OptID" },
                new string[] { projectId.ToString(), Convert.ToString(departmentId), "23" });

            if (obj?.Tables?.Count > 0)
            {
                return await _exec.DataTableToListAsync<DropDownResponse<string>>(obj.Tables[0]);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> CheckModuleExistsInProjectAsync(int projectId, string moduleId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetProjectModuleDetailsByProjectId",
            new string[] { "@ProjectId", "@ModuleId" },
            new string[] { projectId.ToString(), moduleId });
            if (obj?.Rows?.Count > 0)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> DeleteProjectModulesByProjectId(int projectId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
            new string[] { "@ProjectId", "@OptId" },
            new string[] { projectId.ToString(), "25" });

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

    public async Task<string> UpdateProjectModulePaymentStatusAsync(string moduleId, string paymentStatus)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UpdateProjectModuleStatusAndPaymentStatus",
                new string[] { "@OptId", "@ModuleId", "@PaymentStatus" },
            new string[] { "1", moduleId, paymentStatus });

            if (obj?.Rows?.Count > 0)
            {
                var updateProjectModulePaymentStatus = await _exec.DataTableToModelAsync<ProjectModule>(obj);
                return updateProjectModulePaymentStatus.Id;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<string> UpdateProjectModuleStatusAsync(string moduleId, string moduleStatus)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UpdateProjectModuleStatusAndPaymentStatus",
                new string[] { "@OptId", "@ModuleId", "@ModuleStatus" },
            new string[] {"2", moduleId, moduleStatus});

            if (obj?.Rows?.Count > 0)
            {
                var updateProjectModuleStatus = await _exec.DataTableToModelAsync<ProjectModule>(obj);
                return updateProjectModuleStatus.Id ?? string.Empty;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

   
}
