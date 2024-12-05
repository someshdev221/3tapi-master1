using FluentValidation;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Validators
{
    public class EmployeeProfileValidator:AbstractValidator <UpdateEmployeeProfileByManagerViewModel>
    {
        public EmployeeProfileValidator()
        {
            RuleFor(x => x.EmployeeId).NotNull()
           .NotEmpty().WithMessage("EmployeeId is required.");

            RuleFor(x => x.FirstName).NotNull()
                .NotEmpty().WithMessage("FirstName is required.");

            RuleFor(x => x.LastName).NotNull()
                .NotEmpty().WithMessage("LastName is required.");

            RuleFor(x => x.SkypeMail)
                .Matches(@"^live:[a-zA-Z0-9._-]{6,32}$")
                .WithMessage("Invalid Skype ID. A valid Skype ID must start with 'live:'.")
                .When(x => !string.IsNullOrEmpty(x.SkypeMail));

            RuleFor(x => x.Designation).NotNull().NotEmpty()
                .NotEmpty().WithMessage("Designation is required.");

            RuleFor(x => x.DepartmentId).NotNull()
                .GreaterThan(0).WithMessage("DepartmentId must be greater than 0.");

            RuleFor(x => x.Email).NotNull()
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid Email format.");

            RuleFor(x => x.IsActive)
             .NotEmpty().WithMessage("IsActive is required.")
             .InclusiveBetween(0, 1).WithMessage("IsActive must be either 0 or 1.");

            //RuleFor(x => x.CanEditStatus)
            //    .NotEmpty().WithMessage("CanEditStatus is required.")
            //    .InclusiveBetween(0, 1).WithMessage("CanEditStatus must be either 0 or 1.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\d{10}$").WithMessage("PhoneNumber must be a 10-digit number.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.JoiningDate)
                .NotEmpty().WithMessage("JoiningDate is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("JoiningDate cannot be in the future.");

        }
    }
}
