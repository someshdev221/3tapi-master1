using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class EmployeeRequestViewModel: DateFilterViewModel
    {
        [Required]
        public string EmployeeId { get; set; }
    }
}
