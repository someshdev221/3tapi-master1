
using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.ViewModels;

public class FilterViewModel
{
    public int DepartmentId { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? SearchValue { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
    public string? TeamAdminId { get; set; }
}
