using FluentValidation;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validators
{
    public class OnRollFeedbackValidator : AbstractValidator<AddOnRollFeedbackFormDto>
    {
        private readonly IEmployeeService _employeeService;
        public OnRollFeedbackValidator( IEmployeeService employeeService) 
        {
            _employeeService = employeeService;


            RuleFor(x => x.ApplicationUserId)
         .NotEmpty().WithMessage("ApplicationUserId is required.")
         .NotNull().WithMessage("ApplicationUserId cannot be null.");

            RuleFor(x => x.MentorName)
           .NotEmpty().WithMessage("MentorName is required.")
           .NotNull().WithMessage("MentorName cannot be null.");

            RuleFor(x => x.AssessmentMonth)
           .NotEmpty().WithMessage("AssessmentMonth is required.")
           .NotNull().WithMessage("AssessmentMonth cannot be null.");
           

            RuleSet(OperationType.Create.ToString(), () =>
            {
                RuleFor(x => x.AssessmentMonth)
                    .NotNull()
                    .MustAsync(async (model, assessmentMonth, cancellationToken) =>
                    {
                        if (assessmentMonth != null)
                        {
                            int currentMonth = assessmentMonth.Value.Month;
                            int currentYear = assessmentMonth.Value.Year;

                            var traineeFeedback = await _employeeService.GetTraineeFeedbackByDate(currentMonth, currentYear, model.ApplicationUserId);
                            return traineeFeedback.Model == null;
                        }
                        return false;
                    }).WithMessage(SharedResources.TraineeFeedbackAlreadySubmitted);
            });

            RuleFor(x => x.SkillSet)
           .NotEmpty().WithMessage("SkillSet is required.")
           .NotNull().WithMessage("SkillSet cannot be null.");

            RuleFor(x => x.StartSalary)
          .NotNull().WithMessage("StartSalary cannot be null.");

            RuleFor(x => x.Comments)
           .NotEmpty().WithMessage("Comments is required.")
           .NotNull().WithMessage("Comments cannot be null.").
            MaximumLength(500).WithMessage("Comments must not exceed 500 characters."); ;

            RuleSet(OperationType.Update.ToString(), () =>
            {
                RuleFor(x => x.FeedBackId).GreaterThan(0).MustAsync(async (model, feedBackId, cancellationToken) =>
                {
                    if (feedBackId > 0)
                    {
                        var traineeFeedback = await _employeeService.GetTraineeFeedbackById(feedBackId, model.ApplicationUserId);
                        return traineeFeedback.Model != null;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.TraineeFeedbackFormNotFound);
            });

        }
    }
}
