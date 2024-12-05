using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class FullReportViewModel
    {
        public string? EmployeeId { get; set; }
        public string? Employee { get; set; }
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public Guid? ModuleId { get; set; }
        public string? Module{ get; set; }
        public int? ClientId { get; set; }
        public string? Client { get; set; }
        public int? ProfileId { get; set; }
        public string? Profile{ get; set;}
        public string? Memo { get; set; }
        [DisplayName("Upwork Hours")]
        public decimal? UpworkHours { get; set; }
        [DisplayName("Fixed Hours")]
        public decimal? FixedHours { get; set; }
        [DisplayName("Non-BillableHours")]
        public decimal? NonBillableHours { get; set; }
    }
}
