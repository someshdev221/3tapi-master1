
namespace TimeTaskTracking.Shared.ViewModels;
public class ProjectReportRequestModel
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? HoursFrom { get; set; }
    public DateTime? HoursTo { get; set; }
    public int DepartmentId { get; set; }
    public string? SearchValue { get; set; }
    public string? TeamAdminId { get; set; }
    public string? SortColumn { get; set; } 
    public string? SortOrder { get; set; }  
}
