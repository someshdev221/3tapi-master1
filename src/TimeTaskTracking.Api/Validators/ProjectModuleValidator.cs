using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validators;

public class ProjectModuleValidator : AbstractValidator<ProjectModuleDto>
{
    private readonly IProjectService _projectService;
    private readonly IProjectModuleService _projectModuleService;
    private readonly IEmployeeService _employeeService;
    public ProjectModuleValidator(IProjectService projectService, IProjectModuleService projectModuleService, IEmployeeService employeeService)
    {
        _projectService = projectService;
        _projectModuleService = projectModuleService;
        _employeeService = employeeService;



        RuleSet(OperationType.Create.ToString(), () =>
        {
            RuleFor(x => x.Name)
           .NotEmpty()
           .MustAsync(async (model, Name, cancellationToken) =>
           {
               var existingModule = await _projectModuleService.GetProjectModuleByName(model.ProjectId, Name);
               if (existingModule.Model != null)
               {
                   return false;
               }
               else
                   return true;

           }).WithMessage(SharedResources.NameAlreadyExist);

        });

        RuleFor(x => x.ProjectId).GreaterThan(0).MustAsync(async (projectId, cancellationToken) =>
        {
            if (projectId > 0)
            {
                var client = await _projectService.GetProject(projectId, 0);
                if (client.Model == null)
                    return false;
                return true;
            }
            else
                return true;
        }).WithMessage(SharedResources.ProjectNotFound);

        RuleFor(x => x.EstimatedHours)
          .GreaterThan(0)
          .When(x => x.PaymentStatus == "Pending"
                  || x.PaymentStatus == "Hold"
                  || x.PaymentStatus == "Complete"
                  || x.PaymentStatus == "UpworkHourly");

        RuleFor(x => x.EstimatedHours)
            .Equal(0)
            .When(x => x.PaymentStatus == "NonBillable");

        RuleFor(x => x.ApprovalDate)
            .LessThanOrEqualTo(x => x.Deadline)
            .WithMessage(SharedResources.DateError);

        RuleFor(x => x.PaymentStatus).NotEmpty().MustAsync(async (status, cancellationToken) =>
        {
            if (status != null)
                return SharedResources.IsValidEnumValue(status, new string[] { "Pending", "Hold", "Complete", "UpworkHourly", "NonBillable" });
            return true;
        }).WithMessage(SharedResources.PaymentStatus);

        RuleFor(x => x.ModuleStatus).NotEmpty().MustAsync(async (status, cancellationToken) =>
        {
            if (status != null)
                return SharedResources.IsValidEnumValue(status, new string[] { "Open", "Hold", "Done" });
            return true;
        }).WithMessage(SharedResources.ModuleStatus);

        RuleSet(OperationType.Update.ToString(), () =>
        {
            RuleFor(x => x.Id).NotEmpty().MustAsync(async (model,id, cancellationToken) =>
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var checkModuleExistsInProject = await _projectModuleService.CheckModuleExistsInProject(model.ProjectId, id);
                    if (checkModuleExistsInProject)
                    {
                        var projectModule = await _projectModuleService.GetProjectModule(id, 0);
                        return projectModule.Model != null;
                    }
                    return false;
                }
                return true;
            }).WithMessage(SharedResources.ProjectModuleNotExist);

            RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (model, name, cancellationToken) =>
                {
                  if (!string.IsNullOrEmpty(model.Id))
                  {
                      var existingModule = await _projectModuleService.GetProjectModuleByName(model.ProjectId, name);
                      if (existingModule.Model != null)
                      {
                          var moduleId = existingModule.Model.Id.ToLower();
                          var matchCase = model.Id.ToLower();
                          if (moduleId == matchCase)
                          {
                              return true;
                          }
                          return false;
                      }
                      return true;
                  }
                  return true;

                }).WithMessage(SharedResources.NameAlreadyExist);
            });

        RuleSet(OperationType.Delete.ToString(), () =>
        {
            RuleFor(x => x.Id).NotEmpty().MustAsync(async (id, cancellationToken) =>
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var client = await _projectModuleService.GetProjectModule(id, 0);
                    return client.Model != null;
                }
                return true;
            }).WithMessage(SharedResources.ProjectModuleNotExist);
        });
    }
}
