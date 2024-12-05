using FluentValidation;
using TimeTaskTracking.Core.Dtos;

namespace TimeTaskTracking.Validators
{
    public class UpdateUserToolValidator:AbstractValidator<UsersToolDto>
    {
        public UpdateUserToolValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
           
        }
    }
}
