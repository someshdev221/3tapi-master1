using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class TeamLeadProfileViewModel
    {
           public string? FirstName { get; set; }
          public string? LastName { get; set; }
        public int IsActive { get; set; }
            public string? TeamAdminId { get; set; }
            public string? Address { get; set; }
            public DateTime JoiningDate { get; set; }
            public int Experience { get; set; }
            public string? EmployeeID { get; set; }
            public string? Designation { get; set; }
            public string? ProfileImage { get; set; }
            public string? Email { get; set; }
            public bool CanEditStatus { get; set; }


            public List<EmployeeProjectsViewModel> Projects { get; set; }
            public List<EmployeeToolsViewModel> UserTools { get; set; }
            public List<UserBadgeViewModel> UserBadges { get; set; }
            public List<AwardListViewModel> AwardList { get; set; }

        
    }
}
