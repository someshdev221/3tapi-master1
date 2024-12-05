using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class ProjectAssignmentViewModel<T,TKey>
    {
        public int Id { get; set; }
        [Required]
        public TKey ProjectId { get; set; }

        [Required]
        public T EmployeeId { get; set; }
    }
    public class ProjectAssignmentResponseViewModel<T, TKey> : ProjectAssignmentViewModel<T, TKey>
    {
        public string? Reason { get; set; }
    }
}
