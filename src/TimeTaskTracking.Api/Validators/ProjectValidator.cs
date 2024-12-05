using FluentValidation;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validators
{
    public class ProjectValidator : AbstractValidator<ProjectDto>
    {
        private readonly IAccountService _accountService;
        private readonly IClientService _clientService;
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;
        public ProjectValidator(IAccountService accountService, IClientService clientService, IProjectService projectService
            , IEmployeeService employeeService)
        {
            _accountService = accountService;
            _clientService = clientService;
            _projectService = projectService;
            _employeeService = employeeService;

            RuleFor(x => x.ClientId).GreaterThan(0);
            RuleFor(x => x.ClientId).MustAsync(async (clientId, cancellationToken) =>
            {
                if (clientId > 0)
                {
                    var client = await _clientService.GetClient(clientId);
                    if (client.Model == null)
                        return false;
                    return true;
                }
                else
                   return true;
            }).WithMessage("Client does not exist!");
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            // Skills field required validation
            RuleFor(x => x.Skills)         
                .Must(skills => skills == null || skills.Split(',').All(skill => !string.IsNullOrWhiteSpace(skill)))
                .WithMessage(SharedResources.SkillsFieldIsRequired); 


            RuleFor(x => x).CustomAsync(async (project, context, cancellationToken) =>
            {
                if (project.Name != null)
                {
                    var projectDetail = await _projectService.GetProjectByName(project.Name);
                    if (projectDetail.Model != null)
                    {
                        if(project.Id > 0)
                        {
                            if (projectDetail.Model.Id != project.Id && project.Id > 0)
                                context.AddFailure("name", "Project name already exist!");
                        }
                        else
                            context.AddFailure("name", "Project name already exist!");
                    }
                }
            });

            RuleFor(project => project.HiringStatus)
            .Must(status => SharedResources.IsValidEnumValue(status, new int[] { 1, 2, 3, 4 }))
            .WithMessage(SharedResources.HiringStatus);

            RuleFor(project => project.IsBilling)
                .Must(status => SharedResources.IsValidEnumValue(status, new int[] { 1, 2, 3 }))
                .WithMessage(SharedResources.BillingStatus);

            RuleFor(project => project.ProjectStatus)
                .Must(status => SharedResources.IsValidEnumValue(status, new int[] { 1, 2, 3 }))
                .WithMessage(SharedResources.ProjectStatus);
            RuleFor(x => x.SalesPerson).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().MustAsync(async (salesPerson, cancellationToken) =>
            {
                if (!string.IsNullOrEmpty(salesPerson))
                {
                    var employee = await _employeeService.GetEmployeeDetail(salesPerson);
                    if (employee.Model == null)
                        return false;
                    return true;
                }
                else
                    return true;
            }).WithMessage("Sales person does not exists in DB!");

            RuleFor(x => x.DepartmentIds)
           .NotEmpty().WithMessage("Please provide at least one DepartmentId.")
           .ForEach(departmentId =>
               departmentId.GreaterThan(0).WithMessage("Please provide a valid DepartmentId greater than 0."));

            RuleSet(OperationType.Update.ToString(), () =>
            {
                RuleFor(x => x.Id).GreaterThan(0).MustAsync(async (id, cancellationToken) =>
                {
                    if (id > 0)
                    {
                        var project = await _projectService.GetProject(id,0);
                        return project.Model != null;
                    }
                    else
                        return true;
                }).WithMessage("Project does not exist!");
                
                RuleFor(x => x.DepartmentIds)
               .NotEmpty().WithMessage("Please provide at least one DepartmentId.")
               .ForEach(departmentId =>
                   departmentId.GreaterThan(0).WithMessage("Please provide a valid DepartmentId greater than 0."));
            });
            RuleSet(OperationType.Delete.ToString(), () =>
            {
                RuleFor(x => x.Id).GreaterThan(0).MustAsync(async (id, cancellationToken) =>
                {
                    if (id > 0)
                    {
                        var project = await _projectService.GetProject(id,0);
                        return project.Model != null;
                    }
                    else
                        return true;
                }).WithMessage("Project does not exist!");
            });
        }
    }
}
