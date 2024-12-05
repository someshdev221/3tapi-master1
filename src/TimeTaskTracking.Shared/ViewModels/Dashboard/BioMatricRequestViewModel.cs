using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class BioMatricRequestViewModel : PaginationRequestViewModel
    {
        public string? EmployeeId { get; set; }
        public string? TeamLeadId { get; set; }
        public int DepartmentId { get; set; }
        public string? TeamAdminId { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
    }
}
