
namespace TimeTaskTracking.Shared.ViewModels;

public class SuccessFailureResultViewModel<T>
{
    public List<T> Success { get; set; } = new List<T>();
    public List<T> Failure { get; set; } = new List<T>();
}
