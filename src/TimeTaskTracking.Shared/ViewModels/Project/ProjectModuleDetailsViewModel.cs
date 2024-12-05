using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.ViewModels.Project
{
    public class ProjectModuleDetailsViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        public List<string>? ModuleStatus { get; set; }
        public List<string>? PaymentStatus { get; set; }
        public int DepartmentId { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public string? SearchValue { get; set; }
    }
}
