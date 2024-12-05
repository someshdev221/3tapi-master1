using FluentValidation;
using TimeTaskTracking.Core.Dtos;

namespace TimeTaskTracking.Validators
{
    public class UpdateUserProjectValidator:AbstractValidator<UsersProjectDto>
    {
        public UpdateUserProjectValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            
        }

    }
}
