using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class MonthlyFeedbackFormDto
    {
        public int FeedBackId { get; set; }
        public string? ApplicationUserId { get; set; }
        public DateTime? AssessmentMonth { get; set; }
        public string? MentorName { get; set; }
        public string? SkillSet { get; set; }
        public string? Comments { get; set; }
         public int? Performance { get; set; }
    }
}
