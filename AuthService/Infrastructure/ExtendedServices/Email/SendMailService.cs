using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.ExtendedServices.Email
{
    public class SendMailService : ISendMailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ICustomLogger<SendMailService> _logger;

        public SendMailService(IOptions<EmailSettings> emailSettings, ICustomLogger<SendMailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var client = new SendGridClient(_emailSettings.SendGridApiKey);
                var from = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, isHtml ? null : body, isHtml ? body : null);
                await client.SendEmailAsync(msg);

                _logger.Info("Email sent to {Email} with subject: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to send email to {toEmail}", ex);
                throw;
            }
        }
    }
}
