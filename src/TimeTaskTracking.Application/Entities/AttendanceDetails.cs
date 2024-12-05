using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Models.Entities
{
    public class AttendanceDetails
    {
        [Required]
        public List <string>? EmpId { get; set; }
        [Required]
        public DateTime FilterByDate { get; set; }
        [Required]
        public int? AttendanceStatus { get; set; }
    }
}
