using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class CompleteModuleDetailsViewModel : ModuleDetailsViewModel
    {
        public DateTime ApprovalDate { get; set; }
        public string? ModuleStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal? ApprovedHours { get; set; }
    }
}
