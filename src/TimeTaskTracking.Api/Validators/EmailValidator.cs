using FluentValidation;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Validators
{
    public class EmailValidator : AbstractValidator<UpdateUserEmailViewModel>
    {
        public EmailValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .Must(Email => Email.Contains("@cssoftsolutions.com"))
            .WithMessage("Email is invalid");
        }
    }
}
