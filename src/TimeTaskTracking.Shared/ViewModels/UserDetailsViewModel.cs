using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class UserDetailsViewModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public decimal? Experience { get; set; }
        public string? Designation { get; set; }
        public int Projects { get; set; }
        public string? ProfileImage { get; set; }
        public string? JoiningDate { get; set; }
        public decimal? ExperienceOnJoining { get; set; }  
        public List<UserBadgeViewModel> Awards { get; set; }
    }
}
