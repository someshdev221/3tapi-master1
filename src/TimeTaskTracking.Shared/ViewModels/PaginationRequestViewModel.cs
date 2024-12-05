using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Shared.ViewModels;

public class PaginationRequestViewModel
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? SearchValue { get; set; }
}
