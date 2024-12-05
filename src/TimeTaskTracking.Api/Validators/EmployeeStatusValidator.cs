using FluentValidation;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validators
{
    public class EmployeeStatusValidator : AbstractValidator<EmployeeStatusDto>
    {
        private readonly IUpworkProfileService _upworkProfileService;
        private readonly IProjectModuleService _projectModuleService;
        private readonly IEmployeeStatusServices _employeeStatusServices;
        public readonly IProjectService _projectService;
        public EmployeeStatusValidator(IUpworkProfileService upworkProfileService, IProjectModuleService projectModuleService, IEmployeeStatusServices employeeStatusServices, IProjectService projectService)
        {
            _upworkProfileService = upworkProfileService;
            _projectModuleService = projectModuleService;
            _employeeStatusServices = employeeStatusServices;
            _projectService = projectService;

            RuleFor(x => x.ProjectID).GreaterThan(0);
            RuleFor(x => x.Date).NotEmpty();
            //RuleFor(x => x.ProfileName).NotEmpty();
            RuleFor(x => x.Memo).NotEmpty();
            RuleFor(x => x.ApplicationUsersId).NotEmpty();
            RuleFor(x => x.ModuleId).NotEmpty();
            RuleFor(x => x.UpworkHours).GreaterThan(-1);
            RuleFor(x => x.FixedHours).GreaterThan(-1);
            RuleFor(x => x.OfflineHours).GreaterThan(-1);

            RuleFor(x => x.ProjectID).GreaterThan(0).MustAsync(async (projectID, cancellationToken) =>
            {
                if (projectID > 0)
                {
                    var existingModule = await _projectService.GetProject(projectID, 0);
                    return (existingModule.Model != null);
                }
                return true;
            }).WithMessage(SharedResources.ProjectsNotExist);

            //RuleFor(x => x.ProfileName).MustAsync(async (profileName, cancellationToken) =>
            //{
            //    if (!string.IsNullOrEmpty(profileName))
            //    {
            //        var existingModule = await _upworkProfileService.GetUpworkProfileByName(profileName);
            //        return (existingModule.Model != null);
            //    }
            //    return true;
            //}).WithMessage(SharedResources.ProfileNotFound);

            RuleFor(x => x.ProfileId).Cascade(CascadeMode.StopOnFirstFailure).GreaterThan(0).MustAsync(async (profileId, cancellationToken) =>
            {
                var existingModule = await _upworkProfileService.GetUpworkProfileById(profileId);
                   return (existingModule.Model != null);
                return true;
            }).WithMessage(SharedResources.ProfileNotFound);

            //RuleFor(x => x.ModuleId).MustAsync(async (id, cancellationToken) =>
            //{
            //    if (!string.IsNullOrEmpty(id))
            //    {
            //        var client = await _projectModuleService.GetProjectModule(id, 0);
            //        return client.Model != null;
            //    }
            //    return true;
            //}).WithMessage(SharedResources.ProjectModuleNotExist);

            RuleFor(x => new { x.ModuleId, x.ProjectID }).MustAsync(async (ids, cancellationToken) =>
            {
                if (!string.IsNullOrEmpty(ids.ModuleId) && ids.ProjectID != 0)
                {
                    var checkModuleExistsInProject = await projectModuleService.CheckModuleExistsInProject(ids.ProjectID, ids.ModuleId);

                    return checkModuleExistsInProject;
                }
                return true;
            }).WithMessage(SharedResources.ProjectModuleIdNotExist);

            RuleSet(OperationType.Update.ToString(), () =>
            {
                RuleFor(x => x.Id).GreaterThan(0).MustAsync(async (id, cancellationToken) =>
                {
                    if (id > 0)
                    {
                        var employeeStatus = await _employeeStatusServices.GetEmployeeStatusById(id);
                        return employeeStatus.Model != null;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.EmployeeStatusNotFound);
            });
            RuleSet(OperationType.Delete.ToString(), () =>
            {
                RuleFor(x => x.Id).GreaterThan(0).MustAsync(async (id, cancellationToken) =>
                {
                    if (id > 0)
                    {
                        var employeeStatus = await _employeeStatusServices.GetEmployeeStatusById(id);
                        return employeeStatus.Model != null;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.EmployeeStatusNotFound);
            });
           
        }
    }
}
