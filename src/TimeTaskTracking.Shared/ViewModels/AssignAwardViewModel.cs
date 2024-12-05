using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class AssignAwardViewModel
    {
        public int Id { get; set; }
        [Required]
        public int BadgeId { get; set; }
        [Required]
        public string UserId { get; set; }
        public string BadgeDescription { get; set; }
        public int DepartmentId { get; set; }
    }
}
