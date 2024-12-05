using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.Enums;

public enum PaymentStatusInvoiceStatusFilter
{
    All = 0,
    [Display(Name = "In Process")]
    InProcess = 1,
    [Display(Name = "Send To Client")]
    SendToClient = 2,
    Paid = 3,
    Hold = 4
}

public enum PaymentStatusInvoiceStatus
{
    [Display(Name = "In Process")]
    InProcess = 1,
    [Display(Name = "Send To Client")]
    SendToClient = 2,
    Paid = 3,
    Hold = 4
}
