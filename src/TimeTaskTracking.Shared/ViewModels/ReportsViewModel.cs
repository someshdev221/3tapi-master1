using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.ViewModels;

public class ReportsViewModel
{

    public string? EmployeeId { get; set; }
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public DateTime? FromDate { get; set; } 
    public DateTime? ToDate { get; set; }
    //public int PageNumber { get; set; }
    //public int PageSize { get; set; }
}
