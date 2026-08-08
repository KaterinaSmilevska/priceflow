using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;
using System.Net;
using System.Net.Mail;

namespace PriceFlowApp.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _cofiguration;
        
        public EmailService(IConfiguration configuration)
        {
            _cofiguration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            ValidationHelper.ValidateRequiredField(toEmail, "Recipient Email", "EMAIL_VALIDATION_REQUIRED");
            ValidationHelper.ValidateRequiredField(subject, "Email subject", "EMAIL_SUBJECT_REQUIRED");
            ValidationHelper.ValidateRequiredField(body, "Email body", "EMAIL_BODY_REQUIRED");

            var host = _cofiguration["Smtp:Host"];
            var port = _cofiguration["Smtp:Port"];
            var username = _cofiguration["Smtp:Username"];
            var password = _cofiguration["Smtp:Password"];
            var fromEmail = _cofiguration["Smtp:FromEmail"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(port) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException("SMTP configuration is incomplete.");
            }

            using var client = new SmtpClient(host, int.Parse(port))
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "PriceFlow"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            client.Send(message);
        }
    }
}
