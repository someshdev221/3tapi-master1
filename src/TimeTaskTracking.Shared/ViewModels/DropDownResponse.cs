using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class DropDownResponse<T>
    {
        public T? Id { get; set; }
        public string? Name { get; set; }
    }
    public class DropDownResponseDepartment<T> : DropDownResponse<T>
    {
        public string Department { get; set; }
    }
}
