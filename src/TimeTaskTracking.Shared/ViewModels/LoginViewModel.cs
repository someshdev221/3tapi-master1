using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.ViewModels;

public record LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(254, ErrorMessage = "Email length can't exceed 254 characters")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; set; }
}
