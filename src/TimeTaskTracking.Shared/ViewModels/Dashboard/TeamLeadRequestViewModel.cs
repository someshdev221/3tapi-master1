using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class TeamLeadRequestViewModel
    {

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        public string? TeamLeadId { get; set; }
    }
}
