using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class EmployeeStatusDto
    {
        public int Id { get; set; }
        public string? ApplicationUsersId { get; set; }
        public int ProjectID { get; set; }
        public DateTime Date { get; set; }
        public string? ModuleId { get; set; }
        public string? Memo { get; set; }
        public decimal UpworkHours { get; set; }
        public decimal FixedHours { get; set; }
        public decimal OfflineHours { get; set; }
        public bool IsSVNUpdated { get; set; }
        public bool UpdatedClient { get; set; }
        [DefaultValue(false)]
        public bool? MarkAsLeave { get; set; }
        public int ProfileId {  get; set; }
    }
    public class EmployeeStatusResponseDto : EmployeeStatusDto
    {
        public string? ModuleName { get; set; }
        public string? AttendanceStatus { get; set; }
        public string? ClientName { get; set; }
        public string? ProjectName { get; set; }
    }
}
