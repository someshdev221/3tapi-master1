using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.ViewModels.Invoice;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ExecuteProcedure _exec;
    public InvoiceRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }

    public async Task<bool> AddInvoice(InvoicePost invoice, string createdBy)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteInvoice",
            new string[] { "@OptId", "@Month", "@ClientId", "@InvoiceHtml", "@DepartmentId", "@CreatedBy", "@DueDate", },
            new string[] { "1", Convert.ToString(invoice.Month), Convert.ToString(invoice.ClientId),Convert.ToString(invoice.InvoiceHtml),
                  Convert.ToString(invoice.DepartmentId), Convert.ToString(createdBy), Convert.ToString(invoice.DueDate) });

            if (obj?.Rows?.Count > 0)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<List<InvoiceListViewModel>> GetInvoices(InvoiceFilterViewModel invoiceFilter)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteInvoice",
            new string[] { "@OptId", "@ClientId", "@FromDate", "@EndDate", "@PaymentStatus", "@Status", "@DepartmentId" },
            new string[] { "2", Convert.ToString(invoiceFilter.ClientId), Convert.ToString(invoiceFilter.FromDate),Convert.ToString(invoiceFilter.ToDate), Convert.ToString(invoiceFilter.PaymentStatus),
                           Convert.ToString(invoiceFilter.Status), Convert.ToString(invoiceFilter.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToListAsync<InvoiceListViewModel>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<InvoiceViewModel> GetInvoiceProjectsDetails(GenrateInvoiceFilterViewModel genrateInvoiceFilterView)
    {
        try
        {
            var obj = await _exec.Get_DataSetAsync("ExecuteInvoice",
            new string[] { "@OptId", "@ClientId", "@DepartmentId", "@FromDate", "@EndDate" },
            new string[] { "3", Convert.ToString(genrateInvoiceFilterView.ClientId), Convert.ToString(genrateInvoiceFilterView.DepartmentId),
               Convert.ToString(genrateInvoiceFilterView.FromDate), Convert.ToString(genrateInvoiceFilterView.ToDate)});

            if (obj?.Tables?.Count > 0)
            {
                var invoiceDetails = new InvoiceViewModel();
                invoiceDetails.ProjectsDetails = await _exec.DataTableToListAsync<InvoiceProjectViewModel>(obj.Tables[0]);
                invoiceDetails.InvoiceIds = Convert.ToString(obj.Tables[1].Rows[0][0]);
                return invoiceDetails;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<Invoice> GetInvoice(int id, int departmentId)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteInvoice",
            new string[] { "@OptId", "@Id", "@DepartmentId" },
            new string[] { "4", Convert.ToString(id), Convert.ToString(departmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToModelAsync<Invoice>(obj);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> UpdateInvoice(InvoicePost invoice)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteInvoice",
            new string[] { "@OptId", "@Id", "@Month", "@InvoiceHtml", "@DueDate", "@DepartmentId" },
            new string[] { "5", Convert.ToString(invoice.Id), Convert.ToString(invoice.Month), Convert.ToString(invoice.InvoiceHtml),
                 Convert.ToString(invoice.DueDate),Convert.ToString(invoice.DepartmentId) });

            if (obj?.Rows?.Count > 0)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateInvoicePaymentStatusOrIsLockStatus(InvoiceUpdate invoice, string approvedBy)
    {
        try
        {
            //if (string.IsNullOrEmpty(approvedBy))
            //{
                var obj = await _exec.Get_DataTableAsync("ExecuteInvoice",
                    new string[] { "@OptId", "@Id", "@PaymentStatus", "@DepartmentId", "@Status", "@ApprovedBy" },
                    new string[] { "6", Convert.ToString(invoice.Id), Convert.ToString(invoice.PaymentStatus), Convert.ToString(invoice.DepartmentId),
                    Convert.ToString(invoice.Status), Convert.ToString(approvedBy)});

                if (obj?.Rows?.Count > 0)
                {
                    return true;
                }
                return false;
                //}
                //else
                //{
                //    var obj = await _exec.Get_DataTableAsync("ExecuteInvoice",
                //       new string[] { "@OptId", "@Id", "@DepartmentId", "@Status", "@ApprovedBy" },
                //       new string[] { "7", Convert.ToString(invoice.Id), Convert.ToString(invoice.DepartmentId),
                //     Convert.ToString(invoice.Status), Convert.ToString(approvedBy) });

                //    if (obj?.Rows?.Count > 0)
                //    {
                //        return true;
                //    }
                //    return false;
            //}
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<bool> DeleteInvoice(int id)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("DeleteInvoice",
                new string[] { "@Id" },
                new string[] { Convert.ToString(id) });
            if (obj?.Rows?.Count > 0)
            {
                var status = Convert.ToInt32(obj.Rows[0]["Status"]);
                return status == 1;
            }
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

}
