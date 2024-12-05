namespace TimeTaskTracking.Shared.ViewModels.Dashboard;

    public class TeamAttendanceViewModel
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? EmployeeName { get; set; }
        public List<AttendanceViewModel> EmployeeAttendance { get; set; }
    }
    public class AttendanceViewModel
    {
        public string? AttendanceStatus { get; set; }
        public int Day { get; set; }
        public decimal? FixedHours { get; set; }
        public decimal? UpworkHours { get; set; }
        public decimal? OfflineHours { get; set; }
        public decimal? TotalHours { get; set; }
    }

