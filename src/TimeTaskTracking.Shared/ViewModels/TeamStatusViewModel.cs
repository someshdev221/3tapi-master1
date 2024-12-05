
namespace TimeTaskTracking.Shared.ViewModels;

public class TeamStatusViewModel
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? ProjectName { get; set; }
    public string? ClientName { get; set; }
    public string? ModuleName { get; set; }
    public string? ProfileName { get; set; }
    public string? Memo { get; set; }
    public decimal? UpworkHours { get; set; } = 0;
    public decimal? FixedHours { get; set; } = 0;
    public decimal? BillingHours { get; set; } = 0;
    public decimal? NonBillableHours { get; set; } = 0;
    public string? Attendance { get; set; }
    public int ClientId { get; set; }
    public int ProjectId { get; set; }
    public string ModuleId { get; set; }
    public int ProfileId { get; set; }
}
