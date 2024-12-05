
namespace TimeTaskTracking.Models.Entities
{
    public class EmployeeLeavesStatus
    {
        public int Id { get; set; }
        public bool IsPresent { get; set; }
        public DateTime Date { get; set; }
        public string ApplicationUsersId { get; set; }
        public string AttendanceStatus { get; set; }
    }
}
