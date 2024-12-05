
namespace TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

public class ManagerProjectSummaryViewModel
{
    public int TotalEmployees { get; set;}
    //public int TotalProjects { get; set;}
    //public decimal UpworkHours { get; set;}
    //public decimal FixedBillingHours { get; set; }
    public decimal WorkInHand { get; set; }
    public decimal PendingPayment { get; set; }
    public decimal ProductivityPercentage { get; set; }
}
