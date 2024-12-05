

namespace TimeTaskTracking.Models.Entities
{
    public class FeedbackForm
    {
        public int FeedBackId { get; set; }
        public string? ApplicationUserId { get; set; }
        public string? Name { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public DateTime? DOJ { get; set; }
        public DateTime? AssessmentMonth { get; set; }
        public string? MentorName { get; set; }
        public string? SkillSet { get; set; }
        public bool? StartSalary { get; set; }
        public string? Comments { get; set; }
        public int? Performance { get; set; }
        public string? Strength { get; set; }
        public string? Weakness { get; set; }
        public string? TimePeriod { get; set; }
    }
}
