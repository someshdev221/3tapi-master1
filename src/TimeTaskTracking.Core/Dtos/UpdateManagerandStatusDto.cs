using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class UpdateManagerandStatusDto
    {
        [Required]
        public string EmployeeId { get; set; }
        public string? TeamAdminId { get; set; }
        public int? IsActive { get; set; }
    }
}
