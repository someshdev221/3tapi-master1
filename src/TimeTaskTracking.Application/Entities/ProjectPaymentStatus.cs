
namespace TimeTaskTracking.Core.Dtos
{
    public class ProjectPaymentStatus
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<ProjectModuleReport>? ModulesList { get; set; }
    }

    public class ProjectModuleReport
    {
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int? BillingType { get; set; }   
        public int? HiringStatus { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public int? ApprovedHours { get; set; }
        public decimal? BillingHours { get; set; }
        public decimal? LeftHours { get; set; }
        public string? ModuleStatus { get; set; }
        public string? PaymentStatus { get; set; }
    }

}
