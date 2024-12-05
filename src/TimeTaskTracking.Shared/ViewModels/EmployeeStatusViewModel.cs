using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class EmployeeStatusViewModel
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string? ModuleName { get; set; }
        public string ProfileName { get; set; }
        public string? Memo { get; set; }
        public decimal UpworkHours { get; set; }
        public decimal FixedHours { get; set; }
        public decimal OfflineHours { get; set; }
        public DateTime StatusSubmitDate { get; set; }




    }
}
