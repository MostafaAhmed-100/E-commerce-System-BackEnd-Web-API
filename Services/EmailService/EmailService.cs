using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApplication1.Services.EmailService;
using WebApplication1.Settings;

namespace WebApplication1.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                await ProcessEmailSendingAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                HandleEmailError(ex, toEmail);
            }
        }

        private async Task ProcessEmailSendingAsync(string toEmail, string subject, string body)
        {
            var emailMessage = BuildEmailMessage(toEmail, subject, body);
            await DeliverEmailAsync(emailMessage);
            _logger.LogInformation("Email successfully sent to {ToEmail}", toEmail);
        }

        private MimeMessage BuildEmailMessage(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.SenderEmail);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            return email;
        }

        private async Task DeliverEmailAsync(MimeMessage emailMessage)
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
            await smtp.SendAsync(emailMessage);
            await smtp.DisconnectAsync(true);
        }

        private void HandleEmailError(Exception ex, string toEmail)
        {
            _logger.LogError(ex, "Error occurred while sending email to {ToEmail}", toEmail);
            throw new InvalidOperationException("حدث خطأ أثناء محاولة إرسال البريد الإلكتروني.", ex);
        }
    }
}