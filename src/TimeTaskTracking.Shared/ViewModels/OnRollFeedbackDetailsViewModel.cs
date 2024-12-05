using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class OnRollFeedbackDetailsViewModel
    {
        public string ApplicationUserId {  get; set; }
        public string AssessmentMonth { get; set; }
        public string Performance { get; set; }
        public string MentorName { get; set; }
        public bool RecommendedSalary {  get; set; }
        public string SkillSet {  get; set; }
    }
}
