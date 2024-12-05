using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class ProductivityResponseViewModel<T>
    {
        public string? TeamLeadName { get; set; }
        public decimal? DeliveredProductivityHours { get; set; } = 0;
        public decimal? EstimatedProductivityHours { get; set; } = 0;
        public decimal? ProductivityPercentage { get; set; } = 0;
        public T results { get; set; }
    }
}