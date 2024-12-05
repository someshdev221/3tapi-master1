using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Dtos
{
    public  class EmployeeProfileDto
    {
        public string? EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Designation { get; set; }
        public string? ProfileImage { get; set; }
        public string? DepartmentId { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
        public string? SkypeMail { get; set; }
        public string? PhoneNumber { get; set; }
        public int IsActive { get; set; }
        public DateTime JoiningDate { get; set; }
        public int Experience { get; set; }
        public string? TeamAdminId { get; set; }
        public string? Manager { get; set; }
        public string? Address { get; set; }
        public string? Skills { get; set; }
        public bool CanEditStatus { get; set; }
        public List<EmployeeProjectsViewModel> Projects { get; set; }
        public List<EmployeeToolsViewModel> UserTools { get; set; }
        public List<UserBadgeViewModel> userBadges { get; set; }
        public List<AwardListViewModel> AwardList { get; set; }
        public List<OnRollFeedbackDetailsViewModel> FeedbackDetails { get; set; }

    }
}
