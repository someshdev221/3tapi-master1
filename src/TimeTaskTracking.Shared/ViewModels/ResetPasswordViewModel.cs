using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Shared.ViewModels.Utility;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class ResetPasswordViewModel: TokenViewModel
    {
        public string Password { get; set; }
    }
    public class TokenViewModel
    {
        [Required]
        public string Token { get; set; }
    }
}
