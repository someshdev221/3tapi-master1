using Microsoft.Extensions.DependencyInjection;
using TimeTaskTracking.Infrastructure.Repositories;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;

namespace TimeTaskTracking.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITeamStatusRepository, TeamStatusRepository>();
        services.AddScoped<ISettingRepository, SettingRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IEmployeeStatusRepository, EmployeeStatusRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ICommonRepository, CommonRepository>();

        return services;
    }
}
