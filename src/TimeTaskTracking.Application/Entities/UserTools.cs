namespace TimeTaskTracking.Models.Entities
{
    public class UserTools
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? NetworkUrl { get; set; }
        public string? LocalUrl { get; set; }
        public DateTime DateTime { get; set; }
        public string ApplicationUsersId { get; set; }
        public string? Technology { get; set; }
    }
}
