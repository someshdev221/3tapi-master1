
namespace TimeTaskTracking.Models.Entities
{
    public class EstimatedHour
    {
        public decimal EstimatedHours { get; set; }
        public decimal BilledHours { get; set; }
        public decimal OfflineHours { get; set; }
        public string PaymentStatus { get; set; }
    }
}
