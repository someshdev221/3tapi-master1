using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels.Invoice;

namespace TimeTaskTracking.Controller;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IValidator<InvoiceDto> _validator;
    public InvoiceController(IInvoiceService invoiceService, IValidator<InvoiceDto> validator)
    {
        _invoiceService = invoiceService;
        _validator = validator;
    }
    [HttpPost]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> AddInvoice(InvoicePostDto invoice)
    {
        var invoiceDto = new InvoiceDto()
        {
            ClientId = invoice.ClientId,
            DueDate = invoice.DueDate,
            Id = invoice.Id,
            Month = invoice.Month,
            InvoiceHtml = invoice.InvoiceHtml
        };
        var results = await _validator.ValidateAsync(invoiceDto);
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

        var invoiceResult = await _invoiceService.AddInvoice(invoice);
        if (!invoiceResult.Model)
            return BadRequest(invoiceResult);
        return Ok(invoiceResult);
    }

    [HttpGet]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetInvoices([FromQuery] InvoiceFilterViewModel invoiceFilters)
    {
        var invoiceResult = await _invoiceService.GetInvoices(invoiceFilters);
        return Ok(invoiceResult);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetInvoice([Required] int id, int departmentId)
    {
        var invoiceResult = await _invoiceService.GetInvoice(id, departmentId);
        return Ok(invoiceResult);
    }

    [HttpPut]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> UpdateInvoice(InvoicePostDto invoice)
    {
        var invoiceDto = new InvoiceDto()
        {
            ClientId = invoice.ClientId,
            DueDate = invoice.DueDate,
            Id = invoice.Id,
            Month = invoice.Month,
            InvoiceHtml = invoice.InvoiceHtml
        };

        var results = await _validator.ValidateAsync(invoiceDto);
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

        var invoiceResult = await _invoiceService.UpdateInvoice(invoice);
        if (!invoiceResult.Model)
            return BadRequest(invoiceResult);
        return Ok(invoiceResult);
    }
    [HttpPut("UpdateInvoicePaymentStatusOrIsLockStatus")]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> UpdateInvoicePaymentStatusOrIsLockStatus(InvoiceUpdateDto invoice)
    {
        var invoiceResult = await _invoiceService.UpdateInvoicePaymentStatusOrIsLockStatus(invoice);
        if (!invoiceResult.Model)
            return BadRequest(invoiceResult);
        return Ok(invoiceResult);
    }

    [HttpGet("GetInvoiceProjectsDetails")]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> GetInvoiceProjectsDetails([FromQuery] GenrateInvoiceFilterViewModel genrateInvoiceFilterView)
    {
        var invoiceProjectDetails = await _invoiceService.GetInvoiceProjectsDetails(genrateInvoiceFilterView);
        return Ok(invoiceProjectDetails);
    }

    
    [HttpGet("GetPaymentStatusInvoiceStatusFilter")]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public IActionResult GetPaymentStatusInvoiceStatusFilter()
    {
        return Ok(SharedResources.GetEnumData<PaymentStatusInvoiceStatusFilter, int>());
    }

    [HttpGet("GetPaymentStatusInvoiceStatus")]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public IActionResult GetPaymentStatusInvoiceStatus()
    {
        return Ok(SharedResources.GetEnumData<PaymentStatusInvoiceStatus, int>());
    }


    [HttpDelete("DeleteInvoiceById")]
    [Authorize(Policy = "ManagerOrBDMOrAdminOrHOD")]
    public async Task<IActionResult> DeleteInvoice([Required] int id, int departmentId)
    {
        var response = await _invoiceService.DeleteInvoice(id, departmentId);
        if (!response.Model)
            return BadRequest(response);
        return Ok(response);
    }
}
