
namespace TimeTaskTracking.Models.Entities
{
    public class UserBadges
    {
        public int Id { get; set; }
        public int BadgeId { get; set; }
        public string BadgeName { get; set; }
        public string BadgeImage { get; set; }
        public string BadgeDescription { get; set; }
        public DateTime DateReceived { get; set; }
        public string? SubmittedBy { get; set; }
        public string? SubmittedByName { get; set; }
    }

    public class UserBadge
    {
        public int Id { get; set; }
        public string? BadgeName { get; set; }
        public string? BadgeImage { get; set; }
    }
}
