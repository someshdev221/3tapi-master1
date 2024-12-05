using FluentValidation;
using FluentValidation.Validators;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validators
{
    public class UpworkProfileValidator : AbstractValidator<UpworkDetailsProfileDto>
    {
        private readonly IUpworkProfileService _upworkProfileService;
        private readonly IEmployeeService _employeeService;
        public UpworkProfileValidator(IUpworkProfileService upworkProfileService, IEmployeeService employeeService)
        {
            _upworkProfileService = upworkProfileService;
            _employeeService = employeeService;

            RuleSet(OperationType.Create.ToString(), () =>
            {
                RuleFor(x => x.Name)
               .NotEmpty()
               .MustAsync(async (model, name, cancellationToken) =>
               {
                       var existingModule = await _upworkProfileService.GetUpworkProfileByName(name);
                       if (existingModule.Model != null)
                       {
                           return false;
                       }
                       else
                           return true;
               }).WithMessage(SharedResources.NameAlreadyExist);

                RuleFor(x => x.DepartmentId).GreaterThan(0).MustAsync(async (DepartmentId, cancellationToken) =>
                {
                    if (DepartmentId > 0)
                    {
                        var deparmentExists = await _employeeService.GetDepartmentById(DepartmentId);
                        if (!deparmentExists.Model)
                        {
                            return false;
                        }
                        else
                            return true;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.DepartmentNotFound);
            });

            RuleSet(OperationType.Update.ToString(), () =>
            {
                RuleFor(x => x.Id).GreaterThan(0).MustAsync(async (id, cancellationToken) =>
                {
                    if (id > 0)
                    {
                        var upworkProfile = await _upworkProfileService.GetUpworkProfileById(id);
                        return upworkProfile.Model != null;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.UpworkProfileNotExist);

                RuleFor(x => x.Name)
               .MustAsync(async (dto, name, cancellationToken) =>
               {
                   if (dto.Id > 0)
                   {
                       var existingModule = await _upworkProfileService.GetUpworkProfileByName(name);
                       if (existingModule.Model != null)
                       {
                           if (existingModule.Model.Id == dto.Id)
                           {
                               return true;
                           }
                           return false;
                       }
                       return true;
                   }
                   return true;

               }).WithMessage(SharedResources.NameAlreadyExist);

                RuleFor(x => x.DepartmentId).GreaterThan(0).MustAsync(async (DepartmentId, cancellationToken) =>
                {
                    if (DepartmentId > 0)
                    {
                        var deparmentExists = await _employeeService.GetDepartmentById(DepartmentId);
                        if (!deparmentExists.Model)
                        {
                            return false;
                        }
                        else
                            return true;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.DepartmentNotFound);
            });

            RuleSet(OperationType.Delete.ToString(), () =>
            {
                RuleFor(x => x.Id).GreaterThan(0).MustAsync(async (id, cancellationToken) =>
                {
                    if (id > 0)
                    {
                        var upworkProfile = await _upworkProfileService.GetUpworkProfileById(id);
                        return upworkProfile.Model != null;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.UpworkProfileNotExist);
            });
        }
    }
}
