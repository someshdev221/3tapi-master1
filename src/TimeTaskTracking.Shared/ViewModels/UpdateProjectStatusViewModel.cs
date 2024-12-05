using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class UpdateProjectStatusViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        public int? DepartmentId { get; set; }
        [Required]
        public int ProjectStatus { get; set; } 
    }
}
