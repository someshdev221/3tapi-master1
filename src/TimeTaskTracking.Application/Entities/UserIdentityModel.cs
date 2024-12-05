
namespace TimeTaskTracking.Models.Entities
{
    public class UserIdentityModel
    {
        public string UserId { get; set; }
        public int DepartmentId { get; set; }
        public string Role { get; set; }
        public string LoggedInUserId { get; set; }
    }
}
