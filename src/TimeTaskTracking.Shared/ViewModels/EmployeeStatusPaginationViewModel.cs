using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class EmployeeStatusPaginationViewModel :EmployeeStatusFilterViewModel
    {
      
        [Required]
        public DateTime FromDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
