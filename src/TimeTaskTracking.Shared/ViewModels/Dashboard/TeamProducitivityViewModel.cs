using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class TeamProducitivityViewModel
    {
        public string? EmployeeId { get; set; }
        public string? Email { get; set; }
        public string? Designation { get; set; }
        public int TotalWorkingHours { get; set; }
        public int TotalLeaves { get; set; }
        public int TotalHalfDays { get; set; }
        public int LeaveHours { get; set; }
        public int HalfDaysHours { get;}
        public int ExpectedWorkingHours { get; set;}
    }
}
