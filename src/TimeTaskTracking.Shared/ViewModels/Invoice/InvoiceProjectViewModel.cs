namespace TimeTaskTracking.Shared.ViewModels.Invoice;

public class InvoiceProjectViewModel
{
    public string MonthYear { get; set; }
    public int ProjectId {  get; set; }
    public string ProjectName { get; set; }
    public string ModuleId { get; set; }
    public string Module {  get; set; }
    public decimal HourBilled { get; set; }
}
