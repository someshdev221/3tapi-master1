using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.ViewModels;
public class EmployeeProjectRequestModel
{
    [Required]
    public Guid EmployeeId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
public class DepartmentEmployeeProjectRequestModel : EmployeeProjectRequestModel
{
    public int DepartmentId { get; set; }
}