using TimeTaskTracking.Models.Entities.Project;

namespace TimeTaskTracking.Shared.ViewModels.Project;
public class ModuleDetailsViewModel : ProjectModule
{
    public decimal NonBillableHours { get; set; }
    public decimal UpworkHours { get; set; }
    public decimal FixedHours { get; set; }
    public decimal BilledHours { get; set; }
}

