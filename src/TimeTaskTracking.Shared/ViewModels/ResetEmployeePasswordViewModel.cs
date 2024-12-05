using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class ResetEmployeePasswordViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]  
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
