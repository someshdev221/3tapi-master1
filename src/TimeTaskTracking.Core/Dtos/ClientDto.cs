using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Skypeid { get; set; }
        public string? PhoneNumber { get; set; }
        public int DepartmentId { get; set; }
        public string? BillingAddress { get; set; }
        [Required]
        public string Country { get; set; }
        public string? ClientCompanyName { get; set; }
    }
}
