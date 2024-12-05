using FluentValidation;
using TimeTaskTracking.Core.Dtos;

namespace TimeTaskTracking.Validators
{
    public class UserToolValidator : AbstractValidator<UsersToolDto>
    {
        public UserToolValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description length can't exceed 1000 characters");

            RuleFor(x => x.ApplicationUsersId)
                .NotEmpty()
                .WithMessage("Application user ID is required");

            RuleFor(x => x.Technology)
                .MaximumLength(100)
                .WithMessage("Technology length can't exceed 100 characters");
            RuleFor(x => x.NetworkUrl)
              .Must(BeAValidUrl)
              .WithMessage("Invalid Network URL format");

            RuleFor(x => x.LocalUrl)
                .Must(BeAValidUrl)
                .WithMessage("Invalid Local URL format");
        }
        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
