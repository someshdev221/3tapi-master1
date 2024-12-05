using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class TeamPerformanceViewModel
    {
        public string? EmployeeId { get; set; }
        public string? Name { get; set; }
        public decimal? FixedHours { get; set; }
        public decimal? UpworkHours { get; set; }
        public decimal? NonBillableHours { get; set; }
        public string? TotalBilling { get; set; }
        public string? Email { get; set; }
        public decimal? ProductivityPercentage { get; set; }

    }
}