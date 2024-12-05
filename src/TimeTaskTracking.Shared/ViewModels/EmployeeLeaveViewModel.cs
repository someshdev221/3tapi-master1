using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class EmployeeLeaveViewModel
    {
        public bool IsPresent { get; set; }
        public DateTime Date { get; set; }
        public string ApplicationUsersId { get; set; }
        public string AttendanceStatus { get; set; }
    }
}
