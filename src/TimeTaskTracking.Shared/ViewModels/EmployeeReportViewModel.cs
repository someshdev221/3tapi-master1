
namespace TimeTaskTracking.Shared.ViewModels;
public class EmployeeReportViewModel
{
    public string EmployeeId { get; set; }
    public string FullName { get; set; }
    public decimal TotalUpworkHours { get; set; }
    public decimal TotalFixedHours { get; set; }
    public decimal TotalOfflineHours { get; set; }
    public decimal TotalBillingHours { get; set; }
    public string ProjectNames { get; set; }
    public string ProjectIds { get; set; }
}
