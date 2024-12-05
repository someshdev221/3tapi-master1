using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class ProjectRequestViewModel : DateFilterViewModel
    {
        [Required]
        public int ProjectId { get; set; }

        public string? TeamLeadId { get; set; }
    }
}
