using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.AccessControl;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;

namespace TimeTaskTracking.Core.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAccountService,AccountService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITeamStatusService, TeamStatusService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IUpworkProfileService, UpworkProfileService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IEmployeeStatusServices, EmployeeStatusServices>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IProjectModuleService, ProjectModuleService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IReportsService, ReportsService>();
        services.AddScoped<IEmployeeDashboardService, EmployeeDashboardService>();
        services.AddScoped<IHRDashboardService, HRDashboardService>();
        services.AddScoped<IPerformanceService, PerformanceService>();
        services.AddScoped<ITeamAdminDashboardService, TeamAdminDashboardService>();
        services.AddScoped<ICommonService, CommonService>();
        services.AddScoped<ISuperAdminDashboardService, SuperAdminDashboardService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        return services;
    }
}
