using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class ManagerTeamLeadBAAndBDListViewModel : TeamLeadBAAndBDListViewModel
    {
        public List<DropDownResponse<string>> Manager { get; set; }
    }
    public class ManagerTeamLeadBAAndBDListDepartmentsViewModel
    {
        public string Department { get; set; }
        public List<DropDownResponseDepartment<string>> Manager { get; set; }
        public List<DropDownResponseDepartment<string>> TeamLead { get; set; }
        public List<DropDownResponseDepartment<string>> BDM { get; set; }
        
    }
}
