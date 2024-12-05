
using TimeTaskTracking.Infrastructure;
using FluentValidation;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Validators;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Validations;

namespace TimeTaskTracking.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IValidator<ProjectDto>, ProjectValidator>();
            services.AddScoped<IValidator<ProjectModuleDto>, ProjectModuleValidator>();
            services.AddScoped<IValidator<UpworkDetailsProfileDto>, UpworkProfileValidator>();
            services.AddScoped<IValidator<UpdateEmployeeProfileByManagerViewModel>, EmployeeProfileValidator>();
            services.AddScoped<IValidator<ClientDto>, ClientValidator>();
            services.AddScoped<IValidator<EmployeeStatusDto>, EmployeeStatusValidator>();
            services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
            services.AddScoped<IValidator<MonthlyFeedbackFormDto> , MonthlyFeedbackValidator>();
            services.AddScoped<IValidator<AddOnRollFeedbackFormDto>, OnRollFeedbackValidator>();
            services.AddScoped<IValidator<WarningEmailViewModel>, WarningMailValidator>();
            services.AddScoped<IValidator<ToDoListViewModel>, ToDoValidator>();
            services.AddScoped<IValidator<UpdateUserEmailViewModel>, EmailValidator>();
            services.AddScoped<IValidator<InvoiceDto>, InvoiceValidator>();
            services.AddScoped<IValidator<AssignBadgeToEmployeesViewModel>, AssignBadgeValidator>();

            return services;
        }
    }
}
