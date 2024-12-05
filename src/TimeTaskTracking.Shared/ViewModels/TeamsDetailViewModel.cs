namespace TimeTaskTracking.Shared.ViewModels
{
    public class TeamsDetailViewModel
    {
        public string TeamLeadId { get; set; }
        public string TeamLeadName { get; set; }
        public string? TeamLeadProfileImage { get; set; } 
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string? Designation { get; set; } 
        public string? ProfileImage { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int? ExperienceOnJoining { get; set; }

    }

    public class TeamDetailGroupedViewModel
    {
        public string TeamLeadId { get; set; }
        public string TeamLeadName { get; set; }
        public string? TeamLeadProfileImage { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int? ExperienceOnJoining { get; set; }
        public List<TeamEmployeeViewModel> Employees { get; set; }
    }

    public class TeamEmployeeViewModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string? Designation { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int? ExperienceOnJoining { get; set; }
    }
}
