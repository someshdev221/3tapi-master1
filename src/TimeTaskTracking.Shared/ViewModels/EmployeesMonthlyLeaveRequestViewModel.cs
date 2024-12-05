using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.ViewModels;
public class EmployeesMonthlyLeaveRequestViewModel:PaginationRequestViewModel
{
    [Required]
    public int Month { get; set; }
    [Required]
    public int Year { get; set; }
    [Required]
    public int? departmentId { get; set; }

    public string? TeamAdminId { get; set; }

}
