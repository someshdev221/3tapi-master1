using FluentValidation;
using TimeTaskTracking.Core.Dtos;

namespace TimeTaskTracking.Validators
{
    public class UserProfileValidator : AbstractValidator<UpdateUserProfileDto>
    {
        public UserProfileValidator()
        {
            RuleFor(x => x.SkypeMail)
                .Matches(@"^live:[a-zA-Z0-9._-]{6,32}$")
                .WithMessage("Invalid Skype ID. A valid Skype ID must start with 'live:'.")
                .When(x => !string.IsNullOrEmpty(x.SkypeMail));

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\d{10}$")
                .WithMessage("Phone number must be a 10-digit number.");

            RuleFor(x => x.Designation).NotEmpty();

            RuleFor(x => x.DepartmentId)
                .NotEmpty()
                .WithMessage("Department ID must not be empty.");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
