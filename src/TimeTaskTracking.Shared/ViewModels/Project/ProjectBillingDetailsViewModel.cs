using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Project
{
    public class ProjectBillingDetailsViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        public int DepartmentId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set;}
    }
}
