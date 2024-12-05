
namespace TimeTaskTracking.Models.Entities.Project;

public class ProjectModule
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int ProjectId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public string? CreatedBy { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ExistingHours { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime ApprovalDate { get; set; }
    public string PaymentStatus { get; set; }
    public string? ApprovedBy { get; set; }
    public string ModuleStatus { get; set; }
    public string? ModuleNotes { get; set; }
}
