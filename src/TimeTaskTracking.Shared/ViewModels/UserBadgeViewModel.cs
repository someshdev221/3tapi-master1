using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class UserBadgeViewModel
    {
        public int Id { get; set; }
        public int BadgeId { get; set; }
        public string BadgeName { get; set; }
        public string BadgeImage { get; set; }
        public string BadgeDescription { get; set; }
        public DateTime DateReceived { get; set; }
        public string? SubmittedBy { get; set; }
        public string? SubmittedByName { get; set; }
    }
}
