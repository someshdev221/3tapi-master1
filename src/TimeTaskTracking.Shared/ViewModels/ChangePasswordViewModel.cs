using System.Text.Json.Serialization;
using TimeTaskTracking.Shared.ViewModels.Utility;

namespace TimeTaskTracking.Shared.ViewModels;

public class ChangePasswordViewModel
{
    public string? Id { get; set; }
    public string Email { get; set; }
    public string OldPassward { get; set; }
    public string NewPassword { get; set; }
}
