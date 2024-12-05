using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    //public class AttendanceLogsViewModel
    //{
    //    public string? EmployeeCode { get; set; }
    //    public List<AttendaneBioMatricViewModel> Biomatics { get; set;}
    //}

    public class AttendaneBioMatricViewModel
    {
        public string? EmployeeCode { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string? status { get; set; }
        public string StatusCode { get; set; } = "";
        public int Duration { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public string? PunchRecords { get; set; }
    }
}
