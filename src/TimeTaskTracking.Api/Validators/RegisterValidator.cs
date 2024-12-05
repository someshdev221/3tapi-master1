using FluentValidation;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Linq;
using System.Text.RegularExpressions;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validations;

public class RegisterValidator : AbstractValidator<RegisterDto>
{

    private readonly IClientService _clientService;
    public RegisterValidator(IClientService clientService)
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        //RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(email => email.Contains("@cssoftsolutions.com"))
            .WithMessage("Email is invalid");
        RuleFor(x => x.SkypeMail)
            .Matches(@"^live:[a-zA-Z0-9._-]{6,32}$")
            .WithMessage("Invalid Skype ID. A valid Skype ID must start with 'live:'.")
            .When(x => !string.IsNullOrEmpty(x.SkypeMail));
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Must(ContainUppercase).WithMessage("Password must contain at least one uppercase letter.")
            .Must(ContainLowercase).WithMessage("Password must contain at least one lowercase letter.")
            .Must(ContainDigit).WithMessage("Password must contain at least one digit.")
            .Must(ContainSpecialCharacter).WithMessage("Password must contain at least one special character.");
        RuleFor(x => x.PhoneNumber)
           .NotEmpty();
        //.Matches(@"^\+91\d{10}$").WithMessage("Phone number must be a valid number");
        RuleFor(x => x.Designation).NotEmpty();


        RuleFor(x => x.EmployeeNumber).Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty().WithMessage(SharedResources.EmployeeNumberIsRequired)
            .MustAsync(async (model, employeeNumber, cancellationToken) =>
            {
                if (new List<string> { "Stipend", "Trainee" }.Contains(model.Designation))
                {
                    Regex regex = new Regex(@"^\d{3,7}$");
                    return (regex.IsMatch(employeeNumber));
                }
                return true;
            }).WithMessage("For employees with the 'Stipend' or 'Trainee' designation, the employee number must be between 3 to 5 digits.")
            .MustAsync(async (model, employeeNumber, cancellationToken) =>
            {
                if (!new List<string> { "Stipend", "Trainee" }.Contains(model.Designation))
                {
                    Regex regex = new Regex(@"^EMP\d{6,9}$");
                    return (regex.IsMatch(employeeNumber));
                }
                return true;
            }).WithMessage("Employee number must start with 'EMP' and making it 9 to 12 characters long.");


        // Replaced DepartmentId rule with the new validation logic
        RuleFor(x => x.DepartmentId).GreaterThan(0).MustAsync(async (DepartmentId, cancellationToken) =>
        {
            if (DepartmentId > 0)
            {
                var departmentExists = await clientService.GetDepartmentById(DepartmentId);
                if (!departmentExists.Model)
                {
                    return false;
                }
            }
            return true;
        }).WithMessage(SharedResources.DepartmentNotFound);
    }
    private bool ContainUppercase(string password)
    {
        return password.Any(char.IsUpper);
    }

    private bool ContainLowercase(string password)
    {
        return password.Any(char.IsLower);
    }

    private bool ContainDigit(string password)
    {
        return password.Any(char.IsDigit);
    }

    private bool ContainSpecialCharacter(string password)
    {
        return Regex.IsMatch(password, @"[\W_]");
    }
}

