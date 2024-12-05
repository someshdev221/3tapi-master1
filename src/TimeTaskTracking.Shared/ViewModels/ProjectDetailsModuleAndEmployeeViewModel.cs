using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class ProjectDetailsModuleAndEmployeeViewModel 
    {
        public  string?  EmployeeId { get; set; }
        public  string? EmployeeName { get; set;}
        public List<ModuleDetailsViewModel> ModuleDetails { get; set; } = new List<ModuleDetailsViewModel>();
    }
}
