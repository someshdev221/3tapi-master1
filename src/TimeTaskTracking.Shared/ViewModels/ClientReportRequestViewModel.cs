
namespace TimeTaskTracking.Shared.ViewModels;
public class ClientReportRequestViewModel
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int DepartmentId { get; set; }
    public string? TeamAdminId { get; set; }
}
