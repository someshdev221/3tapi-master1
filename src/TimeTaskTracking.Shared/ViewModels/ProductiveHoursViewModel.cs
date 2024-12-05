using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class ProductiveHoursViewModel
    {
        public string Id { get; set; }
        public string EmployeeName { get; set; }
        public decimal TotalBillingHours { get; set; }
        public decimal ExpectedProductivity { get; set; }
    }
}
