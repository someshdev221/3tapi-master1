
namespace TimeTaskTracking.Shared.ViewModels;

public class PaginationResponseViewModel<T>
{
    public int TotalCount { get; set; }
    public T results { get; set; }
}
