
namespace TimeTaskTracking.Shared.ViewModels;

public class ReportsResponseViewModel
{
    public string ApplicationUsersId { get; set; }
    public string EmployeeName { get; set; }
    public string? ProjectName { get; set; }
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; }
    public string ModuleName { get; set; }
    public string? ProfileName { get; set; }
    public string? Memo { get; set; }
    public decimal? FixedHours { get; set; }
    public decimal? UpworkHours { get; set; }
    public decimal? NonBillableHours { get; set; }
    public DateTime Date {  get; set; } 
    public string ModuleId { get;set; }
    public int ProfileId { get; set; }
}
