using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.ViewModels.Project;

public class ProjectDetailFilterViewModel
{
    [Required]
    public int ProjectId { get; set; }
    public List<string>? ModuleStatus { get; set; }
    public List<string>? PaymentStatus { get; set; }
    [Required]
    public DateTime? StartDate { get; set; }
    [Required]
    public DateTime? EndDate { get; set; }
    public int DepartmentId { get; set; }
}
