using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class EmployeeStatusFilterViewModel 
    {
        [Required]
        public string UserProfileId { get; set; }
        [Required]
        public DateTime ToDate { get; set; }
    }
}
