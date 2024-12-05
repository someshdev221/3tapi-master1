using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels.Invoice;

namespace TimeTaskTracking.Core.Services.IServices;

public interface IInvoiceService
{
    Task<ResponseModel<bool>> AddInvoice(InvoicePostDto invoice);
    Task<ResponseModel<List<InvoiceListViewModel>>> GetInvoices(InvoiceFilterViewModel invoiceFilter);
    Task<ResponseModel<InvoiceViewModel>> GetInvoiceProjectsDetails(GenrateInvoiceFilterViewModel genrateInvoiceFilterView);
    Task<ResponseModel<InvoiceDto>> GetInvoice(int id, int departmentId);
    Task<ResponseModel<bool>> UpdateInvoice(InvoicePostDto invoice);
    Task<ResponseModel<bool>> UpdateInvoicePaymentStatusOrIsLockStatus(InvoiceUpdateDto invoice);
    Task<ResponseModel<bool>> DeleteInvoice(int id, int departmentId);
}
