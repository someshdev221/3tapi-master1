using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class UpdateModuleStatusViewModel
    {
        public string? ModuleId { get; set; }
        public string? PaymentStatus { get; set; }
        public string? ModuleStatus { get; set; }
    }
}
