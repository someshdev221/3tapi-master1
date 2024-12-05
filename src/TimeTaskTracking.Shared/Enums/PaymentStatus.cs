
using System.ComponentModel;

namespace TimeTaskTracking.Shared.Enums;

public enum PaymentStatus
{
    Pending,
    [Description("Hold/Dispute")]
    Hold,
    Complete,
    [Description("Upwork Hourly")]
    UpworkHourly,
    [Description("Non-Billable")]
    NonBillable,

}
