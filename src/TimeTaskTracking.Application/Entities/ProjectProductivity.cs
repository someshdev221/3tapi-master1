
namespace TimeTaskTracking.Models.Entities
{
    public class ProjectProductivity
    {
        public int ProjectID { get; set; }
        public double TotalUpworkHours { get; set; }
        public double TotalFixedHours { get; set; }
        public double TotalOfflineHours { get; set; }
        public double Productivity { get; set; }
    }
}
