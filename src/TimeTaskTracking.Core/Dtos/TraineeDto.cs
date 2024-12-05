using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class TraineeDto
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public List<string> Months { get; set; }
    }
}
