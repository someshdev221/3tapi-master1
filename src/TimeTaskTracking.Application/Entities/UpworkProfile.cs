
namespace TimeTaskTracking.Models.Entities;

public class UpworkProfile
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int ProfileType { get; set; }
}
