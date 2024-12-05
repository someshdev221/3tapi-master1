using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class TeamLeadPerformanceViewModel
    {
        public int Day { get; set; }
        public double? TotalWorkingHours { get; set; }
        public double? BillingHours { get; set;}
        public double? MonthlyPotentialHours { get; set; }
        public string? AttendanceStatus { get; set; }
    }
}
