
namespace TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

public class TeamProductivityViewModel
{
    public string TeamLeadId { get; set; }
    public string TeamLeadName { get; set; }
    public decimal ProductivityHours { get; set; }
    public decimal ExpectedProductivityHours { get; set; }
    public decimal ProductivityPercentage { get; set; }
}
