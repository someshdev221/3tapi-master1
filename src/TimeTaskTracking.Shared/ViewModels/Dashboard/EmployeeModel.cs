using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class EmployeeModel :FilterViewModel
    {
        public string? Designation { get; set; }
        public int? IsActive { get; set; }
        public string? TeamAdminId { get; set; }
    }
}
