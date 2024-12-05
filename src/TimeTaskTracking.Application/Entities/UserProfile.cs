
namespace TimeTaskTracking.Models.Entities
{
    public class UserProfile
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 
        public string? Email { get; set; }
        public int? IsActive { get; set; }
        public string? Manager { get; set; }
        public bool CanEditStatus { get; set; }
        public string? SkypeMail { get; set; }
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? Designation { get; set; }
        public string? ProfileImage { get; set; }
        public string? Skills { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? Address { get; set; }
        public int? ExperienceOnJoining { get; set; }
        public decimal? UpworkHours { get; set; } = 0;
        public decimal? FixedHours { get; set; } = 0;
        public decimal? OfflineHours { get; set; } = 0;
        public int? Projects { get; set; }
        public decimal? ExperienceYears { get; set; }
        public string? Role { get; set; }
    }
    
}
