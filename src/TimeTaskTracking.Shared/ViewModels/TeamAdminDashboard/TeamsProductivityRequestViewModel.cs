
namespace TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

public class TeamsProductivityRequestViewModel
{
    public string? TeamAdminId { get; set; }
    public int DepartmentId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
