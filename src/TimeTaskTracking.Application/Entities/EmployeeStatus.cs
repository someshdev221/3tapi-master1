using System.ComponentModel;


namespace TimeTaskTracking.Models.Entities
{
    public class EmployeeStatus
    {
        public decimal UpworkHours { get; set; }
        public decimal FixedHours { get; set; }
        public decimal OfflineHours { get; set; }
        public int Id { get; set; }
        public string? ApplicationUsersId { get; set; }
        public int ProjectID { get; set; }
        public DateTime Date { get; set; }
        public string ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public string? ProfileName { get; set; }
        public string? Memo { get; set; }
        public bool IsSVNUpdated { get; set; }
        public bool UpdatedClient { get; set; }
        public bool IsDeleted { get; set; }
        [DefaultValue(false)]
        public bool MarkAsLeave { get; set; }
        public string? AttendanceStatus { get; set; }

        public string? ClientName { get; set; }
        public string? projectName { get; set; }
        public int ProfileId { get;set; }

    }
}
