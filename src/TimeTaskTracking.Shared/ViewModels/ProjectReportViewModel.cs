
namespace TimeTaskTracking.Shared.ViewModels;
public class ProjectReportViewModel
{
    public string ProjectName { get; set; }
    public string ClientName { get; set; }
    public List<ProjectModuleViewModel> ModuleList { get; set; }
}

public class ProjectModuleViewModel
{
    public string ModuleName { get; set; }
    public string ModuleStatus { get; set; }
    public decimal? UpworkHours { get; set; }
    public decimal? FixedHours { get; set; }
    public decimal? OfflineHours { get; set; }
    public decimal? BillingHours { get; set; }
}