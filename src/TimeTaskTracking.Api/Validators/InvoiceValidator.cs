using FluentValidation;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validators;

public class InvoiceValidator : AbstractValidator<InvoiceDto>
{
    private readonly IClientService _clientService;
    public InvoiceValidator(IClientService clientService)
    {
        _clientService = clientService;

        RuleFor(x => x.ClientId).GreaterThan(0).MustAsync(async (id, cancellationToken) =>
        {
            if (id > 0)
            {
                var upworkProfile = await _clientService.GetClientById(id, 0);
                return upworkProfile.Model != null;
            }
            else
                return true;
        }).WithMessage(SharedResources.ClientNotExist);

        RuleFor(invoice => invoice.DueDate)
            .GreaterThan(DateTime.Now).WithMessage("DueDate must be in the future.")
            .When(invoice => invoice.DueDate.HasValue);

        RuleFor(invoice => invoice.InvoiceHtml).NotEmpty();

    }
}
