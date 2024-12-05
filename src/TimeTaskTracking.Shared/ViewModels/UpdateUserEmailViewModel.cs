using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class UpdateUserEmailViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}
