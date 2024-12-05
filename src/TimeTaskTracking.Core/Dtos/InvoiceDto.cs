namespace TimeTaskTracking.Core.Dtos;

public class InvoiceDto
{
    public int Id { get; set; }
    public string? Month { get; set; }
    public DateTime? GeneratedDate { get; set; }
    public int ClientId { get; set; }
    public string? ClientName { get; set; }
    public string? InvoiceHtml { get; set; }
    public int DepartmentId { get; set; }
    public string? CreatedBy { get; set; }
    public bool Status { get; set; }
    public int PaymentStatus { get; set; }
    public DateTime? DueDate { get; set; }
    public string? ApprovedBy { get; set; }
}
