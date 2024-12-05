using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TimeTaskTracking.Shared.ViewModels

{
    public class ProfileReportViewModel
    {
        public string ProfileName { get; set; }
        public string ProfileType { get; set; }
        public string ProjectName { get; set; }
        public string TotalBillableHours { get; set; }
    }
    public class ProfileReportRequestViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; } 
    }
}
