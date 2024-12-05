using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class GroupFullReportViewModel
    {
        public DateTime? Date { get; set; }
        public List<FullReportViewModel> ReportViewModel { get; set; }
    }
}
