
namespace TimeTaskTracking.Shared.ViewModels.Dashboard;

public class EmployeeDashboardViewModel
{
    public string EmployeeName { get; set; }
    public string EmployeeNumber { get; set; }
    public List<EmployeeDashboardAttendanceViewModel> AttendanceList { get; set; }  
    public List<DropDownResponse<int>> UserProjects { get; set; }
    public List<EmployeeDashboardBadgesViewModel> EmployeeBadges { get;set; }
}
public class EmployeeDashboardAttendanceViewModel
{
    public int Day { get; set; }
    public double DayHours { get; set; }
    public double BillingHours { get; set; }
    public int MonthlyPotentialHours { get; set; }
    public string AttendanceStatus { get; set; }
   
}
public class EmployeeDashboardBadgesViewModel
{
    public int Id { get; set; }
    public int BadgeId { get; set; }
    public string BadgeName { get; set; }
    public string BadgeImage { get; set; }
}
