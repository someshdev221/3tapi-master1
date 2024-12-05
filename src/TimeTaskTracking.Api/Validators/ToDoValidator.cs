using FluentValidation;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Validators
{
    public class ToDoValidator : AbstractValidator<ToDoListViewModel>
    {
        public ToDoValidator()
        {
            RuleFor(x => x.ToDo)
                .NotNull().WithMessage(SharedResources.ToDoCannotBeEmptyOrNull);

            RuleFor(x => x.AssignedToId)
                .NotEmpty().WithMessage(SharedResources.AssignedToIdCannotBeEmptyOrNull);
        }
    }
}
