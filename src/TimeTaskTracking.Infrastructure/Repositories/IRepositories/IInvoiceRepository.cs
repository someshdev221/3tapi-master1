using Microsoft.AspNetCore.Http.HttpResults;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels.Invoice;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface IInvoiceRepository
{
    Task<bool> AddInvoice(InvoicePost invoice, string createdBy);
    Task<List<InvoiceListViewModel>> GetInvoices(InvoiceFilterViewModel invoiceFilter);
    Task<InvoiceViewModel> GetInvoiceProjectsDetails(GenrateInvoiceFilterViewModel genrateInvoiceFilterView);
    Task<Invoice> GetInvoice(int id, int departmentId);
    Task<bool> UpdateInvoice(InvoicePost invoice);
    Task<bool> UpdateInvoicePaymentStatusOrIsLockStatus(InvoiceUpdate invoice, string approvedBy);
    Task<bool> DeleteInvoice(int id);
}
