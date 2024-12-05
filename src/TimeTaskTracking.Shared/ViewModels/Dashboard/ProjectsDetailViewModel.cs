using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels.Dashboard
{
    public class ProjectsDetailViewModel
    {
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public decimal? FixedHours { get; set; }
        public decimal? UpworkHours { get; set; }
        public decimal? NonBillableHours { get; set; }
        public decimal? TotalBilling { get; set; }
    }
}
