using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class SuperAdminOverAllPerformanceViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public decimal RequiredProductivity { get; set; }
        public decimal CurrentProductivity { get; set; }
    }
}
