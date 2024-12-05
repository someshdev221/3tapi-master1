using System.Text.Json.Serialization;

namespace TimeTaskTracking.Models.Entities;

public class Register
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImageName { get; set; }
    public string? Email { get; set; }
    public string? SkypeMail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PasswordHash { get; set; }
    public int DepartmentId { get; set; }
    public string? Designation { get; set; }
    public string? DepName { get; set; }
    public string? DepId { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? Address { get; set; }
    public int? ExperienceOnJoining { get; set; }
    public DateTime? JoiningDate { get; set; }
    [JsonIgnore]
    public string? ResponseMessage { get; set; }
    public string? Token { get; set; }
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public int IsActive { get; set; }
    public string? TeamAdminId { get; set; }
}
