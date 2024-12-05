using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class RegisterDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? Email { get; set; }
        public string? SkypeMail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public int DepartmentId { get; set; }
        public string? Designation { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? Address { get; set; }
        public int? ExperienceOnJoining { get; set; }
        public DateTime? JoiningDate { get; set; }

        [SwaggerIgnore]
        [HiddenInput(DisplayValue = false)] // Hide this field from human users
        public string? Country { get; set; }
    }
}
