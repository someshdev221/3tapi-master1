using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class UpdateEmployeeDetailsDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        [JsonIgnore]
        public string? PasswordHash { get; set; }
        public string? Password { get; set; }
        public string? DepartmentId { get; set; }
        public string? Designation { get; set; }
        public string? DepName { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? Address { get; set; }
        public int? ExperienceOnJoining { get; set; }
        public DateTime? JoiningDate { get; set; }
        [JsonIgnore]
        public string? ResponseMessage { get; set; }
    }
}
