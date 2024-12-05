using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Please enter your email.")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Please enter DomainName.")]
    [Url]
    public string DomainName { get; set; }
}
