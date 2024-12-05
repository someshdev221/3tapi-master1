using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class EmployeeListResponse
    {
        public int TotalCount { get; set; }
        public List<EmployeeViewModel> Results { get; set; }
    }

    public class EmployeeViewModel
        {
            public string? EmployeeID { get; set; }
           public string? EmployeeUserName { get; set; }
            public string? EmployeeNumber { get; set; }
            public string? ProfileImage { get; set; }
            public string? Designation { get; set; }
            public string? Department { get; set; }
            public string? Email { get; set; }
            public int EmpStatus { get; set; }
            public string? TeamAdminId { get; set; }
            public string? ResponseMessage { get; set; }
        }
}
