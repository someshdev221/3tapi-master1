using FluentValidation;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Validators
{
    public class AssignBadgeValidator : AbstractValidator<AssignBadgeToEmployeesViewModel>
    {
        public AssignBadgeValidator()
        {
            RuleFor(x => x.Month)
                .NotNull()
                .WithMessage("Month cannot be null.")
                .InclusiveBetween(1, 12)
                .WithMessage("Month must be between 1 and 12.");

            RuleFor(x => x.Year)
                .NotNull()
                .WithMessage("Year cannot be null.")
                .GreaterThan(0)
                .WithMessage("Year should be greater than zero.");

            RuleFor(x => x.DepartmentId)
                .NotNull()
                .WithMessage("DepartmentId cannot be null.")
                .GreaterThan(0)
                .WithMessage("DepartmentId should be greater than zero.");
        }
    }
}
