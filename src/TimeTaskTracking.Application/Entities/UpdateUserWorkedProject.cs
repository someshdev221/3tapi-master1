
namespace TimeTaskTracking.Models.Entities
{
    public class UpdateUserWorkedProject
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? SvnUrl { get; set; }
        public string? LiveUrl { get; set; }
        public string? LocalUrl { get; set; }
        public string ApplicationUsersId { get; set; }
        public int ProjectsId { get; set; }
        public string? Technology { get; set; }
        public bool? Feature { get; set; }
    }
}
