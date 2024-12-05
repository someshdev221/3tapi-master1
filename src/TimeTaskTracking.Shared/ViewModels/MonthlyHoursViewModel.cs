
namespace TimeTaskTracking.Shared.ViewModels
{
    public class MonthlyHoursViewModel
    {
        public string MonthYear { get; set; }
        public Decimal TotalUpworkHours { get; set; }
        public Decimal TotalFixedHours { get; set; }
        public Decimal TotalNonBillableHours { get; set; }
        public Decimal TotalProductiveHours { get; set; }
        public Decimal MonthlyPotentialHours { get; set; }
    }
}
