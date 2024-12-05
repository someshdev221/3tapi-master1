
namespace TimeTaskTracking.Models.Entities
{
    public class TeamAdminModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string PhoneNumber { get; set; }
        public string EmployeeNumber { get; set; }
        public int DepartmentId { get; set; }
        public int IsDeleted { get; set; }
    }
}
