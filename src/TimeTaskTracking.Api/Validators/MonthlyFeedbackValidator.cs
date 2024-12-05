using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Validators
{
    public class MonthlyFeedbackValidator : AbstractValidator<MonthlyFeedbackFormDto>
    {
        private readonly IEmployeeService _employeeService;
        public MonthlyFeedbackValidator( IEmployeeService employeeService)
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
           .NotNull().WithMessage("AssessmentMonth cannot be null.")
           .LessThanOrEqualTo(DateTime.Now).WithMessage("AssessmentMonth cannot be in the future.");

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

            RuleFor(x => x.Comments)
           .NotEmpty().WithMessage("Comments is required.")
           .NotNull().WithMessage("Comments cannot be null.").
            MaximumLength(500).WithMessage("Comments must not exceed 500 characters."); ;

            RuleFor(x => x.Performance)
           .NotEmpty().WithMessage("Performance is required.")
           .GreaterThan(0).WithMessage("Performance must be greater than 0.");

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
