
namespace TimeTaskTracking.Shared.ViewModels;

public class EmployeeAttendanceReportViewModel
{
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
    public List<AttendanceReportViewModel> attendanceReports { get; set; }
}
public class AttendanceReportViewModel
{
    public int Day { get; set; }
    public decimal DayHours {  get; set; }
    public string AttendanceStatus { get; set; }
}

