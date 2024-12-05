using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class TeamProductivityPerformanceReportViewModel
    {
        public string? EmployeeId { get; set; }
        public string? DeveloperName { get; set; }
        public decimal? UpWorkHours { get; set; }
        public decimal? FixedBillingHours { get; set; }
        public decimal? NonBillableHours { get; set; }
        public string? BillingHours { get; set; }
    }
}
