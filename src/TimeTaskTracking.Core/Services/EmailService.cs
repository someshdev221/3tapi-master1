using Microsoft.Extensions.Configuration;
using TimeTaskTracking.Core.Services.IServices;
using MailKit.Net.Smtp;
using MimeKit;

namespace TimeTaskTracking.Core.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendMail(string employeeEmail, string employeeName, string teamLeadEmail, string teamLeadName, string mailContent, string subject)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpServer = emailSettings["SmtpServer"];
        var smtpPort = int.Parse(emailSettings["SmtpPort"]);
        var smtpUsername = emailSettings["SmtpUsername"];
        var smtpPassword = emailSettings["SmtpPassword"];
        var defaultFromEmail = emailSettings["DefaultFromEmail"];
        var defaultFromName = emailSettings["DefaultFromName"];

        // Ensure teamLeadEmail is valid; if not, use a default value
        if (string.IsNullOrWhiteSpace(teamLeadEmail) || !IsValidEmail(teamLeadEmail))
        {
            teamLeadEmail = defaultFromEmail;
            teamLeadName = string.IsNullOrWhiteSpace(teamLeadName) ? defaultFromName : teamLeadName;
        }

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(teamLeadName, teamLeadEmail));
        email.To.Add(new MailboxAddress(employeeName, employeeEmail));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = mailContent
        };

        using var smtp = new SmtpClient();
        smtp.Connect(smtpServer, smtpPort, false);
        smtp.Authenticate(smtpUsername, smtpPassword);
        smtp.Send(email);
        smtp.Disconnect(true);
    }

    public async Task<bool> SendWarningEmailsAsync(List<string> emailAddresses, string description, string subject)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpServer = emailSettings["SmtpServer"];
        var smtpPort = int.Parse(emailSettings["SmtpPort"]);
        var smtpUsername = emailSettings["SmtpUsername"];
        var smtpPassword = emailSettings["SmtpPassword"];
        var defaultFromEmail = emailSettings["DefaultFromEmail"];
        var defaultFromName = emailSettings["DefaultFromName"];

        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, false);
            await smtp.AuthenticateAsync(smtpUsername, smtpPassword);

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(defaultFromName, defaultFromEmail));

            foreach (var email in emailAddresses)
            {
                mailMessage.To.Add(MailboxAddress.Parse(email));
            }

            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = description
            };

            await smtp.SendAsync(mailMessage);

            await smtp.DisconnectAsync(true);
            return true; // Email sent successfully
        }
        catch (Exception ex)
        {
            // Handle exceptions (log or throw)
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false; // Email sending failed
        }
    }



    // Helper method to validate email format
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
