
namespace TimeTaskTracking.Models.Entities
{
    public class UpdateUserProject
    {
        public string Id { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }   
        public string Email { get; set; }
        public string? SkypeMail { get; set; }
        public string? DepartmentId { get; set; }
        public string? ProfileImageName { get; set; }
        public string? Designation { get; set; }
        public string? Skills { get; set; }     
        public DateTime JoiningDate { get; set; }      
        public string? PhoneNumber { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? Address { get; set; }
    }
}
