namespace TimeTaskTracking.Shared.ViewModels.TeamAdminDashboard;

public class TeamsSummaryViewModel
{
    public string TeamLeadId { get; set; }
    public string TeamLeadName { get; set; }
    public string TeamLeadProfilePicture { get; set; }
    public string TeamMemberId { get; set; }
    public string TeamMemberName { get; set; }
    public string TeamMemberExperienceOnJoining { get; set; }
    public DateTime TeamMemberJoiningDate {  get; set; }
    public string TeamMemberProfilePicture { get; set; }
    public string TeamMemberDesignation { get; set; }
}
