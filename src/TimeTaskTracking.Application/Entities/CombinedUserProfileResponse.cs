
namespace TimeTaskTracking.Models.Entities
{
    public class CombinedUserProfileResponse
    {
        public UserProfile UserProfile { get; set; }
        public List<UserProject> UserProjects { get; set; }
        public List<UserTools> UserTools { get; set; }
    }

}
