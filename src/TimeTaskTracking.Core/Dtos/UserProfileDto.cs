using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class UserProfileDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int? IsActive { get; set; }
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
        public decimal? UpworkHours { get; set; }
        public decimal? FixedHours { get; set; }
        public decimal? OfflineHours { get; set; }
        public int? Projects { get; set; }
        public decimal? ExperienceYears { get; set; }
        //public string? TeamAdminFirstName { get; set; }
        //public string? TeamAdminLastName { get; set; }
    }
   
}
