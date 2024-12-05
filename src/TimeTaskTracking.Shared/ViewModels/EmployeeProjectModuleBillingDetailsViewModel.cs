using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class EmployeeProjectModuleBillingDetailsViewModel
    {
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public List<ModuleDetailsViewModel> ModuleDetails { get; set; } = new List<ModuleDetailsViewModel>();

    }
}
