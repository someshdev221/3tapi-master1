using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class ProjectProductivityDto
    {
        public int ProjectID { get; set; }
        public double TotalUpworkHours { get; set; }
        public double TotalFixedHours { get; set; }
        public double TotalOfflineHours { get; set; }
        public double Productivity { get; set; }
    }
}
