using FluentValidation;
using System.Collections.Generic;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Validators
{
    public class WarningMailValidator : AbstractValidator<WarningEmailViewModel>
    {
        public WarningMailValidator()
        {
            RuleFor(x => x.EmployeeId)
           .NotNull().WithMessage(SharedResources.EmployeeListIsRequired)
           .Must(list => list != null && list.Any(id => !string.IsNullOrWhiteSpace(id)))
           .WithMessage(SharedResources.PleaseProvideEmployee);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(SharedResources.DescriptionIsRequired);

        }
    }
}
