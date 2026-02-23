using System.Net;
using System.Net.Mail;

namespace PriceFlowApp.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _cofiguration;
        
        public EmailService(IConfiguration cofiguration)
        {
            _cofiguration = cofiguration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_cofiguration["Smtp:Host"], int.Parse(_cofiguration["Smtp:Port"]))
            {
                Credentials = new NetworkCredential(_cofiguration["Smtp:Username"], _cofiguration["Smtp:Password"]),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress(_cofiguration["Smtp:FromEmail"], "PriceFlow"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
    }
}
