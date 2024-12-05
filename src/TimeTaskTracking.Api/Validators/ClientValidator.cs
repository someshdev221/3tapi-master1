using FluentValidation;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validations
{
    public class ClientValidator : AbstractValidator<ClientDto>
    {

        private readonly IClientService _clientService;

        public ClientValidator(IClientService clientService)
        {
            _clientService = clientService;
            RuleSet(OperationType.Create.ToString(), () =>
            {
                RuleFor(x => x.Name)
               .NotEmpty()
                .MustAsync(async (model, name, cancellationToken) =>
                {
                    var existingClient = await _clientService.GetClientByName(0, name);
                    if (existingClient.Model != null)
                    {
                        return false;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.NameAlreadyExist);

                RuleFor(x => x.Email)
               .NotEmpty()
                .MustAsync(async (model, email, cancellationToken) =>
                {
                    var existingClient = await _clientService.GetClientByEmail(0, email);
                    if (existingClient.Model != null)
                    {
                        return false;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.EmailAlreadyExist);

                RuleFor(x => x.PhoneNumber)
               .NotEmpty()
                .MustAsync(async (model, phoneNumber, cancellationToken) =>
                {
                    var existingClient = await _clientService.GetClientByPhoneNumber(0, phoneNumber);
                    if (existingClient.Model != null)
                    {
                        return false;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.PhoneNumberAlreadyExist);

            });

            RuleSet(OperationType.Update.ToString(), () =>
            {
                RuleFor(x => x.Id).GreaterThan(0).MustAsync(async (id, cancellationToken) =>
                {
                    if (id > 0)
                    {
                        var upworkProfile = await _clientService.GetClientById(id, 0);
                        return upworkProfile.Model != null;
                    }
                    else
                        return true;
                }).WithMessage(SharedResources.ClientNotExist);

                RuleFor(x => x.Name).NotEmpty()
               .MustAsync(async (dto, name, cancellationToken) =>
               {
                   if (dto.Id > 0)
                   {
                       var existingClient = await _clientService.GetClientByName(0, name);
                       if (existingClient.Model != null)
                       {
                           return existingClient.Model.Id == dto.Id;
                       }
                       return true;
                   }
                   return true;

               }).WithMessage(SharedResources.NameAlreadyExist);

                RuleFor(x => x.Email)
               .NotEmpty()
                .MustAsync(async (dto, email, cancellationToken) =>
                {
                if (dto.Id > 0)
                {
                    var existingClient = await _clientService.GetClientByEmail(0, email);
                        if (existingClient.Model != null)
                        {
                            return existingClient.Model.Id == dto.Id;
                        }
                        return true;
                    }
                    return true;
                }).WithMessage(SharedResources.EmailAlreadyExist);

                RuleFor(x => x.PhoneNumber)
               .NotEmpty()
                .MustAsync(async (dto, phoneNumber, cancellationToken) =>
                {
                if (dto.Id > 0)
                {
                    var existingClient = await _clientService.GetClientByPhoneNumber(0, phoneNumber);
                        if (existingClient.Model != null)
                        {
                            return existingClient.Model.Id == dto.Id;
                        }
                        return true;
                    }
                    return true;
                }).WithMessage(SharedResources.PhoneNumberAlreadyExist);
            });


            RuleFor(x => x.Email)
            .NotEmpty().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Please provide a valid email address.")
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email address format.");

            RuleFor(x => x.Skypeid)
                .Matches(@"^live:[a-zA-Z0-9._-]{6,32}$")
                .WithMessage("Invalid Skype ID. A valid Skype ID must start with 'live:'.")
                .When(x => !string.IsNullOrEmpty(x.Skypeid));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?\d{10,15}$")
                .WithMessage("Invalid phone number.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
            RuleFor(x => x.Country).NotNull().NotEmpty();


            // Replaced DepartmentId rule with the new validation logic
            RuleFor(x => x.DepartmentId).GreaterThan(0).MustAsync(async (DepartmentId, cancellationToken) =>
            {
                if (DepartmentId > 0)
                {
                    var deparmentExists = await clientService.GetDepartmentById(DepartmentId);
                    if (!deparmentExists.Model)
                    {
                        return false;
                    }
                }
                return true;
            }).WithMessage(SharedResources.DepartmentNotFound);
        }
    }
}
