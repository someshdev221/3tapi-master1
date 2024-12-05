using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class HoursSummaryViewModel
    {
        public decimal EstimatedHours { get; set; }
        public decimal BilledHours { get; set; }
        public decimal OfflineHours { get; set; }
    }
}
