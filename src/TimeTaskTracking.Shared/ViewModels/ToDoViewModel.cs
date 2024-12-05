using TimeTaskTracking.Models.Entities;

namespace TimeTaskTracking.Shared.ViewModels
{
    public class ToDoViewModel 
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set;}
        public string? TeamLeadId { get; set; }
        public string? TeamLeadName { get; set; }
        public ToDoListModel? ToDoList { get; set; }
    }
}
