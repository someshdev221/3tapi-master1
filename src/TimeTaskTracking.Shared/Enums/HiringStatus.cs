using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Shared.Enums;

public enum HiringStatus
{
    Agency = 1,
    Freelancer = 2,
    Direct = 3,
    [Display(Name = "Other Platform")]
    OtherPlatform = 4
}

public enum HiringTypeFilter
{
    All = 0,
    Agency = 1,
    Freelancer = 2,
    Direct = 3,
    [Display(Name = "Other Platform")]
    OtherPlatform = 4
}
