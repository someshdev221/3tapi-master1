using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class MonthlyTraineeFeedbackViewModel
    {
        public string? FeedBackId { get; set; }
        public string? AssessmentMonth { get; set; }
        public string? MentorName { get; set; }
        public int Performance { get; set; }
        public string? SkillSet { get; set; }
        public string? Comments { get; set; }
    }
}
