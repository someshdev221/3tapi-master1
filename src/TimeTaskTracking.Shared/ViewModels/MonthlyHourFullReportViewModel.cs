using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class MonthlyHourFullReportViewModel
    {
        public string? Month { get; set; }
        public string? UpworkHours { get; set; }
        public string? FixedHours { get; set; }
        public string? OfflineHours { get; set; }
        public string? ProductiveHours { get; set; }
        public string? MonthlyPotentialHours { get; set; }
    }
}
