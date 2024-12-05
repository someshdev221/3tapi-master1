
namespace TimeTaskTracking.Shared.ViewModels.Project;

public class BillingDetailsViewModel
{
    public string DeveloperName { get; set; }
    public string ProjectModuleName { get; set; }
    public decimal UpworkHours { get; set; }
    public decimal FixedHours { get; set; }
    public decimal OfflineHours { get; set; }
    public decimal TotalBilledHours { get; set; }
    public decimal NonBillableHours { get; set; }
}
