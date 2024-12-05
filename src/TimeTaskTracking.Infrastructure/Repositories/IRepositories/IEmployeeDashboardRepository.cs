using TimeTaskTracking.Shared.ViewModels.Dashboard;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface IEmployeeDashboardRepository
{
    Task<EmployeeDashboardViewModel> GetEmployeeDashboardDetails(string employeeId, int month, int year);
}
