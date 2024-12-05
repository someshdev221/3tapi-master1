
namespace TimeTaskTracking.Models.Entities
{
    public class CombinedUserProfile
    {
        public UserProfile UserProfile { get; set; }
        public List<UserBadges> UserBadges { get; set; }
    }

}
