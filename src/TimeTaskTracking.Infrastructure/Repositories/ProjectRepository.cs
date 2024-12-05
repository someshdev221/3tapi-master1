using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Project;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ExecuteProcedure _exec;
    public ProjectRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }
    public async Task<PaginationResponseViewModel<List<Project>>> GetAllProjects(string employeeId, string role, ProjectFilterViewModel projectFilterViewModel)
    {
        try
        {
            var dataset = await _exec.Get_DataSetAsync("UserProjects",
                new string[] { "@OptID", "@PageNo", "@PageSize", "@SearchValue", "@SortColumn", "@SortOrder", "@IsActive", "@DepID", "@StartDate", "@EndDate", "@EmployeeId", "@Role", "@HiringStatus", "@IsBilling" },
                new string[] { "5", Convert.ToString(projectFilterViewModel.PageNumber), Convert.ToString(projectFilterViewModel.PageSize), projectFilterViewModel.SearchValue?? string.Empty,
                 projectFilterViewModel.SortColumn?? string.Empty, projectFilterViewModel.SortOrder?? string.Empty,  Convert.ToString(projectFilterViewModel.ProjectStatus) ?? "0",Convert.ToString(projectFilterViewModel.DepartmentId)?? string.Empty,
                projectFilterViewModel.StartDate.ToString() ?? string.Empty, projectFilterViewModel.EndDate.ToString(), employeeId, role, projectFilterViewModel.HiringStatus.ToString(), projectFilterViewModel.BilingType.ToString()});

            PaginationResponseViewModel<List<Project>> projectViewModel = new();

            if (dataset == null || dataset.Tables.Count <= 0)
            {
                return null;
            }
            projectViewModel.results = await _exec.DataTableToListAsync<Project>(dataset.Tables[0]);
            projectViewModel.TotalCount = Convert.ToInt32(dataset.Tables[1].Rows[0][0]);
            return projectViewModel;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<DropDownResponse<string>>> GetListOfOpenAssignedProjectsToTeamLeadAsync(string teamLeadId, int departmentId)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetProjectByTeamLeadId",
                new string[] { "@TeamLeaderId", "@DepartmentId", "@OptId" },
                new string[] { Convert.ToString(teamLeadId), departmentId.ToString(), "2" });

            if (reportsList?.Tables?.Count > 0 && reportsList.Tables[0] != null)
            {
                return await _exec.DataTableToListAsync<DropDownResponse<string>>(reportsList.Tables[0]);
            }
            return null;
        }
        catch (Exception ex)
        {

            return null;
        }
    }


    public async Task<PaginationResponseViewModel<List<Project>>> GetListOfProjectsAssignedToTeamLeadAsync(TeamLeadProjectListViewModel model)
    {
        try
        {
            var dataset = await _exec.Get_DataSetAsync("UserProjects",
                new string[] { "@OptID", "@PageNo", "@PageSize", "@SearchValue", "@DepID", "@StartDate", "@EndDate", "@EmployeeId", "@HiringStatus", "@IsBilling","@IsActive", "@SortColumn", "@SortOrder" },
                new string[] { "32", Convert.ToString(model.PageNumber), Convert.ToString(model.PageSize), model.SearchValue?? string.Empty
                  ,Convert.ToString(model.DepartmentId)?? string.Empty, model.StartDate.ToString() ?? string.Empty, model.EndDate.ToString(), model.TeamLeadId, model.HiringStatus.ToString(), model.BilingType.ToString(), Convert.ToString(model.ProjectStatus), model.SortColumn, model.SortOrder});

            PaginationResponseViewModel<List<Project>> projectViewModel = new();

            if (dataset == null || dataset.Tables.Count <= 0)
            {
                return null;
            }
            projectViewModel.results = await _exec.DataTableToListAsync<Project>(dataset.Tables[0]);
            projectViewModel.TotalCount = Convert.ToInt32(dataset.Tables[1].Rows[0][0]);
            return projectViewModel;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<string>> GetProjectTeamMembers(int projectId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetProjectBillingHistory",
                new string[] { "@OptId", "@ProjectId" },
                new string[] { "1", Convert.ToString(projectId) });

            if (obj?.Rows.Count > 0)
            {
                var employeeList = new List<string>();
                foreach (DataRow row in obj.Rows)
                {

                    employeeList.Add(row["EmployeeId"].ToString());
                }
                return employeeList;
            }

            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<ProjectBillingHistoryViewModel>> GetProjectBillingHistoryAsync(int projectId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetProjectBillingHistory",
                new string[] { "@OptId", "@ProjectId" },
                new string[] { "2", Convert.ToString(projectId) });

            if (obj?.Rows.Count > 0)
            {
                var billingDetails = await _exec.DataTableToListAsync<ProjectBillingHistoryViewModel>(obj);
                return billingDetails;
            }

            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<Project> GetProject(int id, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataSetAsync("UserProjects",
                new string[] { "Id", "@OptId", "@DepID" },
                new string[] { Convert.ToString(id), "4", departmentId.ToString() });

            if (obj == null || obj.Tables.Count == 0 || obj.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            var projectDetails = await _exec.DataTableToModelAsync<Project>(obj.Tables[0]);
            projectDetails.EmployeeList = obj.Tables[1].AsEnumerable()
           .Select(row => row.Field<string>("Id"))
           .ToList();
            return projectDetails;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    // New Method to Get Application Domains
    public async Task<List<ApplicationDomain>> GetApplicationDomainsAsync()
    {
        try
        {

            var dataTable = await _exec.Get_DataTableAsync("GetApplicationDomains",
                new string[] { "@OptId"},
                new string[] { "1" });

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }

            return await _exec.DataTableToListAsync<ApplicationDomain>(dataTable);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<int> AddNewProject(Project project)
    {

        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
                new string[] { "@Name", "@Description", "@IsActive", "@ClientName", "@ApplicationDomains",
                        "@Notes","@ClientId","@ProductionUrl", "@StageUrl", "@IsBilling", "@HiringStatus","@CreatedBy","OptID","@DepID","@InvoiceProjectID","@SalesPerson", "@Skills", "@InterDepartment"},
                new string[] { project.Name ?? string.Empty, project.Description ?? string.Empty, Convert.ToString((int)project.ProjectStatus) ?? string.Empty,
                        project.ClientName ?? string.Empty, project.ApplicationDomains, project.Notes ?? string.Empty,Convert.ToString(project.ClientId),project.ProductionUrl ?? string.Empty,
                        project.StageUrl ?? string.Empty, Convert.ToString(project.IsBilling),Convert.ToString(project.HiringStatus),project.CreatedBy, "2","0", project.InvoiceProjectID ?? string.Empty,
                         project.SalesPerson, project.Skills, Convert.ToString(project.InterDepartment )});

            if (obj?.Rows?.Count > 0)
            {
                var projectData = await _exec.DataTableToModelAsync<Project>(obj);
                return projectData.Id;
            }
            return 0;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<int> UpdateProjectDetails(Project project)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
                new string[] { "@Name", "@Description","@IsActive", "@ClientName","@ApplicationDomains", "@Notes", "@ClientId","@ProductionUrl",
                            "@StageUrl", "@IsBilling", "@HiringStatus", "@OptId","@CreatedBy","@UpdatedBy","@Id","@DepID","@SalesPerson", "@Skills", "@InterDepartment" },
                new string[] { project.Name ?? string.Empty, project.Description ?? string.Empty, Convert.ToString((int)project.ProjectStatus) ?? string.Empty,
                        project.ClientName ?? string.Empty, project.ApplicationDomains , project.Notes ?? string.Empty,Convert.ToString(project.ClientId), project.ProductionUrl ?? string.Empty,
                        project.StageUrl ?? string.Empty, Convert.ToString(project.IsBilling),Convert.ToString(project.HiringStatus), "6" ,project.CreatedBy,project.UpdatedBy,Convert.ToString(project.Id),"0",
                project.SalesPerson, project.Skills, Convert.ToString(project.InterDepartment)});

            if (obj?.Rows?.Count > 0)
            {
                var projectData = await _exec.DataTableToModelAsync<Project>(obj);
                return projectData?.Id ?? 0;
            }
            return 0;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<bool> UpdateProjectStatusAsync(int projectId, int projectStatus)
    {
        try
        {
            var obj = _exec.Get_DataTable("UserProjects",
            new string[] { "@OptId","@ProjectId", "@IsActive" },
            new string[] { "33", Convert.ToString(projectId), Convert.ToString(projectStatus) });

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

    public async Task<bool> DeleteProject(int id)
    {
        try
        {
            var obj = _exec.Get_DataTable("UserProjects",
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
    public async Task<List<Project>> GetAllProjectsByTL(string teamLeadId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
                new string[] { "@TLID", "@OptId" },
                new string[] { teamLeadId, "8" });

            if (obj.Rows.Count > 0 && obj != null)
                return _exec.DataTableToList<Project>(obj);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<DropDownResponse<string>>> GetProjectNameByTL(string teamLeaderId, int departmentId)
    {
        try
        {
            var reportsList = await _exec.Get_DataSetAsync("GetProjectByTeamLeadId",
                new string[] { "@TeamLeaderId", "@DepartmentId", "@OptId" },
                new string[] { Convert.ToString(teamLeaderId), departmentId.ToString(),"1" });

            if (reportsList?.Tables?.Count > 0 && reportsList.Tables[0] != null)
            {
                return await _exec.DataTableToListAsync<DropDownResponse<string>>(reportsList.Tables[0]);
            }

            return null;
        }
        catch (Exception ex)
        {

            return null;
        }
    }
    public async Task<Project> GetProjectByName(string name)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
                new string[] { "@Name", "@OptId" },
                new string[] { Convert.ToString(name), "22" });

            if (obj != null && obj.Rows?.Count > 0)
                return await _exec.DataTableToModelAsync<Project>(obj);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<List<ProjectTypeViewModel>> GetProjectsByDepartment(int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataSetAsync("UserProjects",
                new string[] { "@DepID", "@OptID" },
                new string[] { Convert.ToString(departmentId), "24" });

            if (obj?.Tables?.Count > 0)
            {
                return await _exec.DataTableToListAsync<ProjectTypeViewModel>(obj.Tables[0]);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<ProjectTeamsViewModel>> GetListOfEmployeesInProjectById(int projectId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetEmployeeInProject",
                new string[] { "@ProjectId" },
                new string[] { Convert.ToString(projectId) });

            if (obj.Rows.Count > 0 && obj != null)
            {
                return await _exec.DataTableToListAsync<ProjectTeamsViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> RemoveEmployeeFromProject(int projectId, string employeeId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("DeleteEmployeeFromProject",
            new string[] { "@ProjectId", "@EmployeeId" },
            new string[] { projectId.ToString(), employeeId });

            if (obj?.Rows?.Count > 0)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Internal Server Error. An error occurred while deleting client .");
        }
    }

    public async Task<int> AddDocumentAsync(string fileName, int projectId)
    {
        try
        {
            var optId = 1;
            var obj = await _exec.Get_DataTableAsync("UploadDocumentSP",
                new string[] { "@FileName", "@ProjectId", "@OptId" },
                new string[] { fileName, projectId.ToString(), optId.ToString() });

            if (obj?.Rows?.Count > 0)
            {
                var projectData = await _exec.DataTableToModelAsync<DocumentUpload>(obj);
                return projectData.Id;
            }
            return 0;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<List<DocumentUploads>> GetUploadedDocumentAsync(int projectId)
    {
        try
        {
            var optId = 2;
            var obj = await _exec.Get_DataTableAsync("UploadDocumentSP",
                new string[] { "@ProjectId", "@OptId" },
                new string[] { projectId.ToString(), optId.ToString() });

            var documentUploads = new List<DocumentUploads>();
            if (obj?.Rows != null)
            {
                foreach (DataRow row in obj.Rows)
                {
                    var documentUpload = new DocumentUploads
                    {
                        FileName = row["FileName"].ToString(),
                    };
                    documentUploads.Add(documentUpload);
                }
            }
            return documentUploads;
        }
        catch (Exception ex)
        {
            return new List<DocumentUploads>();
        }
    }

    public async Task<ProjectProductivity> GetProjectProductivityByProjectIdAsync(int projectId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetProjectProductivityByProjectId",
                new string[] { "@ProjectID", "@DepartmentID" },
                new string[] { projectId.ToString(), departmentId.ToString() });

            if (obj?.Rows?.Count > 0)
            {
                var projectData = await _exec.DataTableToModelAsync<ProjectProductivity>(obj);
                return projectData;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<(int, string)> ProjectCountInCurrentMonthAndDepartmentName(int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
               new string[] { "@DepID", "@OptID" },
               new string[] { Convert.ToString(departmentId), "26" });

            if (obj?.Rows?.Count > 0)
            {
                return (Convert.ToInt32(obj.Rows[0][0]), Convert.ToString(obj.Rows[0][1]));
                //var employeeCountInTeam = Convert.ToInt32(obj.Rows[0][0]);
                //return employeeCountInTeam;
            }
            return (0, null);
        }
        catch (Exception ex)
        {
            return (0, null);
        }
    }

    public async Task<bool?> AddNewApplicationDomain(string ApplicationDomain)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetApplicationDomains",
                new string[] { "@OptId", "@DomainType" },
                new string[] { "2", ApplicationDomain });

            if (obj?.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<int> GetApplicationDomainIdByName(string ApplicationDomain)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetApplicationDomains",
                new string[] { "@OptId","@DomainType" },
                new string[] { "3" ,ApplicationDomain });

            if (obj?.Rows.Count > 0)
            {
                int skillId = Convert.ToInt32(obj.Rows[0][0]);
                return skillId;
            }
            return 0;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<List<DropDownResponse<int>>> GetManagerProjectList(string teamAdminId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetManagerProjectList",
               new string[] { "@EmployeeId", "@DepartmentId" },
               new string[] { teamAdminId, Convert.ToString(departmentId) });

            if (obj != null)
            {
                var projectList = await _exec.DataTableToListAsync<DropDownResponse<int>>(obj);
                return projectList;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task<List<string>> GetAssignedEmployeesToTheProject(int projectId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("GetEmployeeListByProjectId",
               new string[] { "@ProjectId", "@DepartmentId" },
               new string[] { Convert.ToString(projectId), Convert.ToString(departmentId) });

            if (obj != null && obj.Rows.Count > 0)
            {
                var projectList = new List<string>();

                foreach (DataRow row in obj.Rows)
                {
                    if (row["EmployeeId"] != null)
                    {
                        projectList.Add(row["EmployeeId"].ToString());
                    }
                }

                return projectList;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<Project> GetProjectByClient(int clientId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
               new string[] { "@ClientId", "@OptID" },
               new string[] { Convert.ToString(clientId), "27" });

            if (obj != null && obj?.Rows?.Count > 0)
            {
                var project = await _exec.DataTableToModelAsync<Project>(obj);
                return project;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task AddProjectDepartment(int projectId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
               new string[] { "@ProjectId", "@DepID", "@OptID" },
               new string[] { Convert.ToString(projectId), Convert.ToString(departmentId), "28" });
        }
        catch (Exception ex)
        {
        }
    }

    public async Task<List<int>> GetProjectDepartments(int projectId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
               new string[] { "@ProjectId", "@OptID" },
               new string[] { Convert.ToString(projectId), "29" });
            if (obj != null && obj.Rows?.Count > 0)
            {
                // Manually extract the integers from the DataTable
                List<int> projectDepartments = new List<int>();

                foreach (DataRow row in obj.Rows)
                {
                    if (int.TryParse(row[0].ToString(), out int departmentId)) // Assuming first column is the department ID
                    {
                        projectDepartments.Add(departmentId);
                    }
                }

                return projectDepartments;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task RemoveDepartmentFromProject(int projectId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
               new string[] { "@ProjectId", "@DepID", "@OptID" },
               new string[] { Convert.ToString(projectId), Convert.ToString(departmentId), "30" });
        }
        catch (Exception ex)
        {
        }
    }

    public async Task RemoveEmployeesFromTheProject(int projectId, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserProjects",
               new string[] { "@ProjectId", "@DepID", "@OptID" },
               new string[] { Convert.ToString(projectId), Convert.ToString(departmentId), "31" });
        }
        catch (Exception ex)
        {
        }
    }

    //public async Task AddSkillsToProject(int projectId, string skills)
    //{
    //    await _exec.ExecuteProcedureAsync("AddSkillsToProject",
    //        new SqlParameter("@ProjectId", projectId),
    //        new SqlParameter("@Skills", skills));
    //}

    //public async Task<string> AddSkillsToProject(string skills)
    //{
    //    try
    //    {
    //        var obj = await _exec.Get_DataTableAsync("AddSkillsToProject",
    //           new string[] { "@Skills" },
    //           new string[] { Convert.ToString(skills) });

    //        if (obj != null && obj?.Rows?.Count > 0)
    //        {
    //            var project = await _exec.DataTableToModelAsync<>(obj);
    //            return project;
    //        }
    //        return null;
    //    }
    //    catch (Exception ex)
    //    {
    //        return null;
    //    }
    //}
}
