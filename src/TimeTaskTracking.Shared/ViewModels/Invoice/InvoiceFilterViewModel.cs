using System.ComponentModel.DataAnnotations;
namespace TimeTaskTracking.Shared.ViewModels.Invoice;

public class InvoiceFilterViewModel : GenrateInvoiceFilterViewModel
{
    public int PaymentStatus { get; set; }
    public bool? Status {  get; set; }
}

public class GenrateInvoiceFilterViewModel
{
    public int ClientId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int DepartmentId { get; set; }
}
