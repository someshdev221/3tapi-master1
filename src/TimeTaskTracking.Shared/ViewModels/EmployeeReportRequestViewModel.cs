
namespace TimeTaskTracking.Shared.ViewModels;
public class EmployeeReportRequestViewModel
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public int DepartmentId { get; set; }
    public string? SearchValue { get; set; }
    public string? TeamAdminId { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
}
