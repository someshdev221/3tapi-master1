using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Enums;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Shared.ViewModels.Invoice;

namespace TimeTaskTracking.Core.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<ResponseModel<bool>> AddInvoice(InvoicePostDto invoice)
        {
            var response = new ResponseModel<bool>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, invoice.DepartmentId, string.Empty);
            invoice.DepartmentId = claims.DepartmentId;
            var isAdded = await _unitOfWork.Invoice.AddInvoice(_mapper.Map<InvoicePost>(invoice), claims.LoggedInUserId);
            if (!isAdded)
                response.Message.Add(SharedResources.ErrorWhileSaveInvoice);
            else
            {
                response.Message.Add(SharedResources.SaveMessage);
                response.Model = isAdded;
            }
            return response;
        }

        public async Task<ResponseModel<List<InvoiceListViewModel>>> GetInvoices(InvoiceFilterViewModel invoiceFilter)
        {
            var response = new ResponseModel<List<InvoiceListViewModel>>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, invoiceFilter.DepartmentId, string.Empty);
            invoiceFilter.DepartmentId = claims.DepartmentId;
            var invoices = await _unitOfWork.Invoice.GetInvoices(invoiceFilter);
            if (invoices == null)
                response.Message.Add(SharedResources.InvoicesNotFound);
            response.Model = invoices;
            return response;
        }
        public async Task<ResponseModel<InvoiceViewModel>> GetInvoiceProjectsDetails(GenrateInvoiceFilterViewModel genrateInvoiceFilterView)
        {
            var response = new ResponseModel<InvoiceViewModel>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, genrateInvoiceFilterView.DepartmentId, string.Empty);
            genrateInvoiceFilterView.DepartmentId = claims.DepartmentId;
            var invoice = await _unitOfWork.Invoice.GetInvoiceProjectsDetails(genrateInvoiceFilterView);
            if (invoice == null)
                response.Message.Add(SharedResources.ProjectModulesNotFoundForInvoice);
            response.Model = invoice;
            return response;
        }

        public async Task<ResponseModel<InvoiceDto>> GetInvoice(int id, int departmentId)
        {
            var response = new ResponseModel<InvoiceDto>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, departmentId, string.Empty);
            var invoice = await _unitOfWork.Invoice.GetInvoice(id, claims.DepartmentId);
            if (invoice == null)
                response.Message.Add(SharedResources.InvoiceNotFound);
            response.Model = _mapper.Map<InvoiceDto>(invoice);
            return response;
        }

        public async Task<ResponseModel<bool>> UpdateInvoice(InvoicePostDto invoice)
        {
            var response = new ResponseModel<bool>();

            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, invoice.DepartmentId, string.Empty);
            invoice.DepartmentId = claims.DepartmentId;

            var invoiceDeatils = await GetInvoice(invoice.Id, invoice.DepartmentId);
            if (invoiceDeatils.Model == null)
                response.Message.Add(SharedResources.InvoiceNotFound);
            else
            {
                if (invoiceDeatils.Model.Status)
                    response.Message.Add(SharedResources.InvoiceLockedCantUpdate);
                else if (invoiceDeatils.Model.PaymentStatus == (int)PaymentStatusInvoiceStatus.Paid || invoiceDeatils.Model.PaymentStatus == (int)PaymentStatusInvoiceStatus.SendToClient)
                    response.Message.Add(SharedResources.CantEditInvoice);
                else
                {
                    var isUpdated = await _unitOfWork.Invoice.UpdateInvoice(_mapper.Map<InvoicePost>(invoice));
                    if (!isUpdated)
                        response.Message.Add(SharedResources.ErrorWhileUpdaingInvoice);
                    else
                    {
                        response.Message.Add(SharedResources.UpdatedMessage);
                        response.Model = isUpdated;
                    }
                }
            }
            return response;
        }
        public async Task<ResponseModel<bool>> UpdateInvoicePaymentStatusOrIsLockStatus(InvoiceUpdateDto invoice)
        {
            var response = new ResponseModel<bool>();

            // Ensure at least one field (PaymentStatus or Lock Status) is required
            if (invoice.PaymentStatus <= 0 && !invoice.Status.HasValue)
            {
                response.Message.Add(SharedResources.PaymentStatusOrLockStatusRequired);
                return response;
            }

            // Ensure only one of PaymentStatus or Lock Status is allowed at a time
            if (invoice.PaymentStatus > 0 && invoice.Status.HasValue)
            {
                response.Message.Add(SharedResources.OnlyOneFieldAllowedPaymentStatusOrLockStatus);
                return response;
            }

            // Retrieve department details from claims
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, invoice.DepartmentId, string.Empty);
            invoice.DepartmentId = claims.DepartmentId;

            // Get invoice details
            var invoiceDetails = await GetInvoice(invoice.Id, invoice.DepartmentId);
            if (invoiceDetails.Model == null)
            {
                response.Message.Add(SharedResources.InvoiceNotFound);
                return response;
            }

            var approvedBy = invoiceDetails.Model.ApprovedBy;

            // BDM cannot lock or unlock invoices
            if (claims.Role == "BDM")
            {
                if (invoice.Status.HasValue)
                {
                    response.Message.Add(SharedResources.CantUpdateLockStatus);
                    return response;
                }

                if (invoiceDetails.Model.Status == true)
                {
                    response.Message.Add(SharedResources.InvoiceLockedCantUpdate);
                    return response;
                }

                if (invoice.PaymentStatus != (int)PaymentStatusInvoiceStatus.InProcess &&
                    invoice.PaymentStatus != (int)PaymentStatusInvoiceStatus.Hold)
                {
                    response.Message.Add(SharedResources.BDMPaymentStatusToPaidException);
                    return response;
                }

                invoice.Status = false;
                approvedBy = claims.LoggedInUserId;
            }
            else
            {
                // Check if invoice is already locked and PaymentStatus is being changed
                if (invoiceDetails.Model.Status == true && invoice.PaymentStatus > 0)
                {
                    response.Message.Add(SharedResources.UnlockInvoiceToChangePaymentStatus);
                    return response;
                }

                // Handle based on current PaymentStatus and Lock Status
                if (invoice.PaymentStatus == (int)PaymentStatusInvoiceStatus.Paid)
                {
                    // Ensure the current status is "Send to Client" before marking as Paid
                    if (invoiceDetails.Model.PaymentStatus != (int)PaymentStatusInvoiceStatus.SendToClient)
                    {
                        response.Message.Add(SharedResources.PaymentStatusInProcessToPaid);
                        return response;
                    }

                    // Mark invoice as Paid and lock it
                    invoice.Status = true;
                    approvedBy = claims.LoggedInUserId;
                }
                else if (invoice.PaymentStatus == (int)PaymentStatusInvoiceStatus.SendToClient)
                {
                    // Lock the invoice when "Send to Client"
                    invoice.Status = true;
                    approvedBy = claims.LoggedInUserId;
                }
                else if (invoice.PaymentStatus == (int)PaymentStatusInvoiceStatus.InProcess)
                {
                    // In Process should remain unlocked
                    invoice.Status = false;
                    approvedBy = claims.LoggedInUserId;
                }
                else if (invoice.PaymentStatus == (int)PaymentStatusInvoiceStatus.Hold)
                {
                    // Hold status should not automatically lock the invoice
                    invoice.Status = false;
                }
                else if (invoice.Status == false || invoice.Status == true)
                {
                    invoice.PaymentStatus = invoiceDetails.Model.PaymentStatus;
                }
            }

            // Update the invoice
            var isUpdated = await _unitOfWork.Invoice.UpdateInvoicePaymentStatusOrIsLockStatus(_mapper.Map<InvoiceUpdate>(invoice), approvedBy);
            if (!isUpdated)
            {
                response.Message.Add(SharedResources.ErrorWhileUpdaingInvoice);
            }
            else
            {
                response.Message.Add(SharedResources.UpdatedMessage);
                response.Model = isUpdated;
            }

            return response;
        }

        public async Task<ResponseModel<bool>> DeleteInvoice(int id, int departmentId)
        {
            var response = new ResponseModel<bool>();
            var userIdentity = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);

            switch (userIdentity.Role)
            {
                case "Admin":
                    if (departmentId == 0)
                    {
                        response.Message.Add(SharedResources.DepartmentIdIsRequired);
                        return response;
                    }
                    else
                        userIdentity.DepartmentId = departmentId;
                    break;

                case "HOD":
                    var claims_HOD = await SharedResources.GetUserIdFromClaimsForHOD(_contextAccessor.HttpContext);
                    userIdentity.DepartmentId = claims_HOD.DepartmentId;
                    break;

                case "BDM":
                    response.Message.Add(SharedResources.UnauthorizedAccessToDeleteTheInvoice);
                    return response;
            }
            var invoice = await _unitOfWork.Invoice.GetInvoice(id, userIdentity.DepartmentId);
            if (invoice == null)
                response.Message.Add(SharedResources.InvoicesNotFound);
            else if (invoice.DepartmentId != userIdentity.DepartmentId)
                response.Message.Add(SharedResources.UnauthorizedAccessToDeleteTheInvoice);
            else if (invoice.Status)
                response.Message.Add(SharedResources.InvoiceLockNotDeleted);
            else if (invoice.PaymentStatus == (int)PaymentStatusInvoiceStatus.SendToClient &&
                                invoice.PaymentStatus == (int)PaymentStatusInvoiceStatus.Paid)
            {
                response.Message.Add(SharedResources.CantDeleteInvoice);
            }
            else
            {
                var isDeleted = await _unitOfWork.Invoice.DeleteInvoice(id);
                if (isDeleted)
                {
                    response.Message.Add(SharedResources.InvoiceDeletedSuccessfully);
                    response.Model = true;
                }
                else
                    response.Message.Add(SharedResources.ErrorWhileDeleteInvoice);
            }
            return response;
        }
    }
}
