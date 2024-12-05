using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Dtos
{
    public class EmployeeDetailDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageName { get; set; }
        public string? Email { get; set; }
        public string? Designation { get; set; }
        public int? ExperienceOnJoining { get; set; }
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
}
