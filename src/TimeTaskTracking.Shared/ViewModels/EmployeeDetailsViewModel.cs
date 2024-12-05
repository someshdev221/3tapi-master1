using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public string? Id { get; set; }
        public int DepartmentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Designation { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? Address { get; set; }
        public int? ExperienceOnJoining { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int ProjectCount { get; set; }
        public List<BadgeViewModel> Badges { get; set; }
    }
    public class BadgeViewModel
    {
        public int Id { get; set; }
        public int BadgeId { get; set; }
        public string BadgeName { get; set; }
        public string BadgeImage { get; set; }
        public string UserId { get; set; }
        public string BadgeDescription { get; set; }
        public DateTime DateReceived { get; set; }
        public string SubmittedBy { get; set; }
        public string SubmittedByName { get; set; }
    }
    public class EmployeeListForBioMatric
    {
        public string? Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeNumber { get; set; }
    }
}
