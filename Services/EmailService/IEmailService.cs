namespace WebApplication1.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string ToEmail, string Subject , string Body);
    }
}
