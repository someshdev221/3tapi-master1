
using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Core.Dtos.Project;

public class ProjectDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? ProductionUrl { get; set; }
    public string? StageUrl { get; set; }
    public int IsBilling { get; set; }
    public int HiringStatus { get; set; }
    public int ProjectStatus { get; set; }
    public string? InvoiceProjectID { get; set; }
    public string? SalesPerson { get; set; }

    [Display(Name = "Technology Set")]
    public string? Skills { get; set; }
    public string? ApplicationDomains { get; set; }

    public bool InterDepartment { get; set; }
    public List<int> DepartmentIds { get; set; }
}
public class ProjectResponseDto : ProjectDto
{
    public string? ClientName { get; set; }
    public int IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? CreatedTime { get; set; }
    public List<string> EmployeeList { get; set; }

}
public class AddAssignProjectDto : ProjectDto
{
    public List<string>? EmployeeList { get; set; } = new List<string>();
}
