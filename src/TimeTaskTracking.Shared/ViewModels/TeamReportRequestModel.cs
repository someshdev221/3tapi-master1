
namespace TimeTaskTracking.Shared.ViewModels;
public class TeamReportRequestModel
{
    public int DepartmentId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? SearchValue { get; set; }
    public string? TeamAdminId { get; set; }
}
