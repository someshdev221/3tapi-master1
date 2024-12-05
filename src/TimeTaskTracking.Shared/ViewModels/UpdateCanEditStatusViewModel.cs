using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class UpdateCanEditStatusViewModel
    {
        public int DepartmentId { get; set; }
        public string? TeamAdminId { get; set; }
        public bool? CanEditStatus { get; set; }
    }
}
