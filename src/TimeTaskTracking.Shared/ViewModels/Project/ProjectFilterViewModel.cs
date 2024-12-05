
using TimeTaskTracking.Shared.Enums;

namespace TimeTaskTracking.Shared.ViewModels.Project;

public class ProjectFilterViewModel : FilterViewModel
{
    public int ProjectStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int BilingType { get; set; } 
    public int HiringStatus { get; set; }
}
