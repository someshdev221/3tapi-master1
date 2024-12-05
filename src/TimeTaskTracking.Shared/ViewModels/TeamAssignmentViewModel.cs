using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class TeamAssignmentViewModel<T>
    {
        public int Id { get; set; }
        [Required]
        public string TeamLeaderId { get; set; }
        public string? TeamAdminId { get; set; }

        [Required]
        public T EmployeeId { get; set; }

        //public int? DepartmentId { get; set; }
    }
    public class TeamAssignmentResponseViewModel<T> : TeamAssignmentViewModel<T>
    {
        public string? Reason { get; set; }
    }
}
