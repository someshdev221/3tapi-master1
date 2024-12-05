
namespace TimeTaskTracking.Shared.ViewModels;

public class EmployeePerformanceSummaryViewModel
{
    public int TotalProjects { get; set; }
    public decimal UpworkHours { get; set; }
    public decimal FixedBillingHours { get; set; }
    public decimal NonBillableHours { get; set; }
    public decimal ProductivityPercentage { get; set; }
}

public class ProjectPerformanceDetailsViewModel
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; }
    public decimal TotalUpworkHours { get; set; }
    public decimal TotalFixedHours { get; set; }
    public decimal TotalBillingHours { get; set; }
    public decimal NonBillableHours { get; set; }
}
public class MonthlyBillingSummaryViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalBillingHours { get; set; }
}

public class EmployeeAttendanceSummaryViewModel
{
    public decimal LeavePercentage { get; set; }
    public decimal PresentPercentage { get; set; }
    public decimal AbsentPercentage { get; set; }
}