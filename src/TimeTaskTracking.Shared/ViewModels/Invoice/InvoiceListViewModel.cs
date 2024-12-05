namespace TimeTaskTracking.Shared.ViewModels.Invoice;

public class InvoiceListViewModel
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; }
    public string Month { get; set; }
    public string DepartmentName { get; set; }
    public DateTime GeneratedDate { get; set; }
    public DateTime DueDate { get; set; }
    public string PaymentStatus { get;set; }
    public bool? Status { get; set; }
}
