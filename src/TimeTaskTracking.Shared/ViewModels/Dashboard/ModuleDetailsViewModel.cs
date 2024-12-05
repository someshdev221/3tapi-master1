using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class ModuleDetailsViewModel
    {
        public string? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public decimal? UpworkHours { get; set; }
        public decimal? FixedHours { get; set; }
        public decimal? NonBillableHours { get; set; }
        public decimal? TotalHours { get; set; }
        public decimal? BillingHours { get; set; }
    }
}
