
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TimeTaskTracking.Shared.ViewModels;

public class EmployeeProfileViewModel : TeamLeadEmployeeProfileViewModel
{
}
public class TeamLeadEmployeeProfileViewModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmployeeID { get; set; }
    public string? Designation { get; set; }
    public string? ProfileImage { get; set; }
    public string? Email { get; set; }
    public int IsActive { get; set; }
    public string? Manager { get; set; }
    public string? Department { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime JoiningDate { get; set; }
    public int Experience { get; set; }
    public string? TeamAdminId { get; set; }
    public bool CanEditStatus { get; set; }
    public string? Skills { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SkypeMail { get; set; }
    public string? RoleName { get; set; }

    public List<EmployeeProjectsViewModel> Projects { get; set; }
    public List<EmployeeToolsViewModel> UserTools { get; set; }
    public List<UserBadgeViewModel> UserBadges { get; set; }
    public List<AwardListViewModel> AwardList { get; set; }
    public List<OnRollFeedbackDetailsViewModel> FeedbackDetails { get; set; }
}

