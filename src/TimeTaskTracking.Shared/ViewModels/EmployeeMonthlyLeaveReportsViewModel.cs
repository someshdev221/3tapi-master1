
namespace TimeTaskTracking.Shared.ViewModels;

public class EmployeeMonthlyLeaveReportsViewModel
{
    public string Id { get; set; }
    public string EmployeeName { get; set; }
    public string ProfilePicture { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Leaves { get; set; }
    public int HalfDay { get; set; }
}
