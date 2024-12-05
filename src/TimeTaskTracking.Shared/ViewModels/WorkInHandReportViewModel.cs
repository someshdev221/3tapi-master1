namespace TimeTaskTracking.Shared.ViewModels;

public class WorkInHandReportViewModel
{
	public int ProjectId {  get; set; }
	public string ProjectName { get; set; }
    public string ModuleId { get; set; }
    public string ModuleName { get; set; }
    public DateTime DeadlineDate { get; set; }
    public Decimal ApprovedHours { get; set; }
    public Decimal BilledHours { get; set; }
    public Decimal LeftHours { get; set; }
    public string ModuleStatus { get; set; }
}
