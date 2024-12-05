using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class EmployeeProjectsViewModel
    {
        public string? ProjectName { get; set; }
        public string? Technology  { get; set; }
        public string? Description { get; set; }
        public string? ProductionURL { get; set; }
        public string? SvnURL { get; set; }
        public string? LiveURL { get; set; }
        public string? LocalURL { get; set; }
    }
}
