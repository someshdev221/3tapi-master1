namespace TimeTaskTracking.Models.Entities;

public class InvoicePost
{
    public int Id { get; set; }
    public string? Month { get; set; }
    public int ClientId { get; set; }
    public string? InvoiceHtml { get; set; }
    public int DepartmentId { get; set; }
    public DateTime? DueDate { get; set; }
}
public class InvoiceUpdate
{
    public int Id { get; set; }
    public bool Status { get; set; }
    public int PaymentStatus { get; set; }
    public int DepartmentId { get; set; }
}
