using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTaskTracking.Models.Entities;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class TeamLeadToDoViewModel
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public ToDoListModel? ToDoList { get; set; }
    }
}
