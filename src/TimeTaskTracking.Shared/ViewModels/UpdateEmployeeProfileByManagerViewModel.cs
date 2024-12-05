using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class UpdateEmployeeProfileByManagerViewModel
    {
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SkypeMail { get; set; }
        public string? Designation { get; set; }
        public int? DepartmentId { get; set; }
        public string? Email { get; set; }
        public int? IsActive { get; set; }
        public bool CanEditStatus { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Skills { get; set; }
        public DateTime JoiningDate { get; set; }
    }
}
