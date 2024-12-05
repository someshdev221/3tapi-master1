using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class FullReportRequestViewModel
    {
        public string? TeamAdminId { get; set; }
        public int DepartmentId { get; set; }
        public string? EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public int ClientId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
