using TimeTaskTracking.Infrastructure.Repositories.IRepositories;

namespace TimeTaskTracking.Infrastructure;

public interface IUnitOfWork
{
    IAccountRepository Account { get; }
    ITeamStatusRepository TeamStatus { get; }
    ISettingRepository Setting { get; }
    IDashboardRepository Dashboard { get; }
    IUpworkProfileRepository UpworkProfile { get; }
    IProjectRepository Project { get; }
    IEmployeeStatusRepository EmployeeStatus { get; }
    IProjectModuleRepository ProjectModule { get; }
    IClientRepository Client { get; }
    IEmployeeRepository Employee { get; }
    IReportsRepository Reports { get; }
    IUserProfileRepository UserProfile { get; }
    IEmployeeDashboardRepository EmployeeDashboard { get; }
    IHRDashboardRepository HRDashboard { get; }
    IPerformanceRepository Performance { get; }
    ITeamAdminDashboardRepository TeamAdminDashboard { get; }
    ICommonRepository CommonRepository { get; }
    ISuperAdminDashboardRepository SuperAdminDashboard { get; }
    IInvoiceRepository Invoice { get; }
}
