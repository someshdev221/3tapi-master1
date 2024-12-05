using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class UpdateUserProfileDto
    {
        public string? Id { get; set; }       
        public string? FirstName { get; set; }     
        public string? LastName { get; set; }     
        public string? Email { get; set; }
        public string? SkypeMail { get; set; }
        public string? DepartmentId { get; set; }
        public IFormFile? ProfileImage { get; set; }  
        public string? Designation { get; set; }
        public string? Skills { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? PhoneNumber { get; set; } 
        public string? EmployeeNumber { get; set; }
        public string? Address { get; set; }
    }
}
