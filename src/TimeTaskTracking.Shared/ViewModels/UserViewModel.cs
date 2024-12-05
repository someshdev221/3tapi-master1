using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class UserViewModel : PaginationRequestViewModel
    {
        public int DepartmentId { get; set; }
    }
}
