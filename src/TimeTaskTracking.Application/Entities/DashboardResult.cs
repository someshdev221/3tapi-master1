using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Models.Entities
{
    public class DashboardResult
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public decimal WorkInHand { get; set; }
        public decimal PendingPayment { get; set; }
        public int TotalEmployeeCount { get; set; }
        public List<EmployeeDetail> EmployeeDetails { get; set; }
    }

    public class EmployeeDetail
    {
        public Guid Id { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int? ExperienceOnJoining { get; set; }
        public string Designation { get; set; }
    }
}
