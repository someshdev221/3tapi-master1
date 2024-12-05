
namespace TimeTaskTracking.Shared.ViewModels;
public class EmployeeReportRequestModel:PaginationRequestViewModel
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int DepartmentId { get; set; }
    public string? TeamAdminId { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
}
