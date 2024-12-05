using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class AssignBadgeToEmployeesViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int DepartmentId { get; set; }
    }
}
