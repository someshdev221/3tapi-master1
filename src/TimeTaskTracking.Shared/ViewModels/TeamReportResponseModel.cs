

namespace TimeTaskTracking.Shared.ViewModels;
public class TeamReportResponseModel
{
    public string TeamLeaderName { get; set; }
    public List<EmployeeBillingDetails> EmployeeDetails { get; set; }
}

public class EmployeeBillingDetails
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal? UpworkHours { get; set; }
    public decimal? FixedHours { get; set; }
    public decimal? BillingHours { get; set; }
    public decimal? OfflineHours { get; set; }
}
