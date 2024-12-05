using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class TeamLeadProjectListViewModel:PaginationRequestViewModel
    {
        [Required]
        public string TeamLeadId { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } = DateTime.Today;
        public int BilingType { get; set; }
        public int HiringStatus { get; set; }
        public int ProjectStatus { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
    }
}
