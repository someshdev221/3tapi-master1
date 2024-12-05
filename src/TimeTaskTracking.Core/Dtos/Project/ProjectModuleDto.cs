
using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Core.Dtos.Project;

public class ProjectModuleDto
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public int ProjectId { get; set; }
    public decimal EstimatedHours { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime ApprovalDate { get; set; }
    public string PaymentStatus { get; set; }
    public string? ApprovedBy { get; set; }
    public string ModuleStatus { get; set; }
    public string? ModuleNotes { get; set; }
}
public class ProjectModuleResponseDto : ProjectModuleDto
{
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public string? CreatedBy { get; set; }
    public double? ExistingHours { get; set; }
}

