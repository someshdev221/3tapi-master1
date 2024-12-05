using FluentValidation;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Validators
{
    public class AssignAwardValidator:AbstractValidator<AssignAwardViewModel>
    {
        public AssignAwardValidator()
        {
            RuleFor(x => x.BadgeId)
           .NotEmpty().WithMessage("BadgeId is required.")
           .GreaterThan(0).WithMessage("BadgeId must be greater than 0.");

            RuleFor(x => x.UserId)
           .NotEmpty().WithMessage("UserId is required.")
           .NotNull().WithMessage("UserId cannot be null.")
           .MaximumLength(50).WithMessage("UserId must not exceed 50 characters.");
        }
    }
}
