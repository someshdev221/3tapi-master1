using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class MonthlyHourFullReportRequestViewModel :DateFilterViewModel
    {
        [Required]
        public string EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public int ClientId { get; set; }
    }
}
