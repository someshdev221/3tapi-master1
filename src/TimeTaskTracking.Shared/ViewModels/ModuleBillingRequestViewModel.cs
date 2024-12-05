using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class ModuleBillingRequestViewModel :DateFilterViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        public string? PaymentStatus { get; set; }
        public string? ModuleStatus { get; set; }

    }
}
