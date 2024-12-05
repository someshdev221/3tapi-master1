
namespace TimeTaskTracking.Core.Services.IServices;

public interface IEmailService
{
    void SendMail(string employeeEmail, string employeeName, string teamLeadEmail, string teamLeadName, string mailContent, string subject);
    Task<bool> SendWarningEmailsAsync(List<string> emailAddresses, string description, string subject);
}
