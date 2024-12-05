using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Repositories;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;

namespace TimeTaskTracking.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly IConfiguration _configuration;
    public UnitOfWork(IConfiguration configuration)
    {
        _configuration = configuration;
        Account = new AccountRepository(_configuration);
        TeamStatus = new TeamStatusRepository(_configuration);
        Setting = new SettingRepository(_configuration);
        Dashboard = new DashboardRepository(_configuration);
        UpworkProfile = new UpworkProfileRepository(_configuration);
        Project = new ProjectRepository(_configuration);
        EmployeeStatus = new EmployeeStatusRepository(_configuration);
        UserProfile = new UserProfileRepository(_configuration);
        ProjectModule = new ProjectModuleRepository(_configuration);
        Client = new ClientRepository(_configuration);
        Employee = new EmployeeRepository(_configuration);
        Reports = new ReportsRepository(_configuration);
        EmployeeDashboard = new EmployeeDashboardRepository(_configuration);
        HRDashboard = new HRDashboardRepository(_configuration);
        Performance = new PerformanceRepository(_configuration);
        TeamAdminDashboard = new TeamAdminDashboardRepository(_configuration);
        CommonRepository = new CommonRepository(_configuration);
        Invoice = new InvoiceRepository(_configuration);
        SuperAdminDashboard = new SuperAdminDashboardRepository(_configuration);
    }
    public IAccountRepository Account { get; private set; }
    public ITeamStatusRepository TeamStatus { get; private set; }
    public ISettingRepository Setting { get; private set; }
    public IDashboardRepository Dashboard { get; private set; }
    public IUpworkProfileRepository UpworkProfile { get; private set; }
    public IProjectRepository Project { get; private set; }
    public IEmployeeStatusRepository EmployeeStatus { get; private set; }
    public IUserProfileRepository UserProfile { get; private set; }
    public IProjectModuleRepository ProjectModule { get; private set; }
    public IClientRepository Client { get; private set; }
    public IEmployeeRepository Employee { get; private set; }
    public IReportsRepository Reports { get; private set; }
    public IEmployeeDashboardRepository EmployeeDashboard { get; private set; }
    public IHRDashboardRepository HRDashboard { get; private set; }
    public IPerformanceRepository Performance { get; private set; }
    public ITeamAdminDashboardRepository TeamAdminDashboard { get; private set; }
    public ICommonRepository CommonRepository { get; private set; }

    public ISuperAdminDashboardRepository SuperAdminDashboard { get; private set; }

    public IInvoiceRepository Invoice { get; private set; }
}
