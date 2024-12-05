
namespace TimeTaskTracking.Shared.ViewModels.Project;

public class ProjectViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? CreatedTime { get; set; }
    public string CreatedBy { get; set; }
    public string ClientName { get; set; }
    public string Notes { get; set; }
    public string AssignedTo { get; set; }
    public decimal? ProjectUpworkHours { get; set; }
    public decimal? ProjectFixedHours { get; set; }
    public decimal? ProjectOfflineHours { get; set; }
    public String ProjectGetID { get; set; }
    public String ProjectListStartDate { get; set; }
    public String ProjectListEndDate { get; set; }
    public string ProductionUrl { get; set; }
    public string StageUrl { get; set; }
    public int ProjectStatus { get; set; }
    public int IsBilling { get; set; }
    public int HiringStatus { get; set; }
    public int ClientId { get; set; }
    public string InvoiceProjectID { get; set; }
    public string CreatedByUser { get; set; }
    public bool InterDepartment { get; set; }
    public List <int> ProjectDepartmentIds { get; set; }
    public List<DropDownResponse<int>> ProjectProfileDetails { get; set; }
}
