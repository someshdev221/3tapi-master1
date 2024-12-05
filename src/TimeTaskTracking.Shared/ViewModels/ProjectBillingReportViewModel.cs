namespace TimeTaskTracking.Shared.ViewModels;
public class ProjectBillingReportViewModel
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; }
    public decimal? TotalUpworkHours { get; set; }
    public decimal? TotalFixedHours { get; set; }
    public decimal? TotalOfflineHours { get; set; }
    public decimal? TotalBillingHours { get; set; }
}
