
namespace TimeTaskTracking.Models.Entities
{
    public class DocumentUpload
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public int ProjectId { get; set; }
    }

    public class DocumentUploads
    {
        public string? FileName { get; set; }
    }
}
