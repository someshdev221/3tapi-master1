
namespace TimeTaskTracking.Shared.ViewModels.Project;

public class ProjectDetailsViewModel
{
    public ProjectViewModel ProjectModels { get; set; }

    public List<ModuleDetailsViewModel> ModuleDetails { get; set; }
    public List<BillingDetailsViewModel> BillingDetails { get; set; }
    public List<ReportsResponseViewModel> EmployeeDetails { get; set; }
    public List<DropDownResponse<string>> ProjectAssignedDetails { get; set; }
}
