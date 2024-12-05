using System.ComponentModel.DataAnnotations;


namespace TimeTaskTracking.Shared.Enums;

public enum BillingType
{
    Fixed = 1,
    Hourly = 2,
    [Display(Name = "Non-Billable")]
    NonBillable = 3
}

public enum BillingTypeStatus
{
    All = 0,
    Fixed = 1,
    Hourly = 2,
    [Display(Name = "Non-Billable")]
    NonBillable = 3
}