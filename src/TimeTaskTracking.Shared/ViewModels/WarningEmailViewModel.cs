using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class WarningEmailViewModel
    {
        public List<string>? EmployeeId { get; set; }
        public string? Description { get; set; }
        public int DepartmentId { get; set; }
    }
}
