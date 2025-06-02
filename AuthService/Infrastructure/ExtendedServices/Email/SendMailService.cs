using ApplicationCore.Interfaces;
using Mailjet.Client;
using Mailjet.Client.Resources;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

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
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required.", nameof(toEmail));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject is required.", nameof(subject));

            try
            {
                _logger.Info($"Api Key: {_emailSettings.ApiKey}, Secret Key: {_emailSettings.SecretKey}");
                _logger.Info($"Sender Email: {_emailSettings.SenderEmail}, Sender Name: {_emailSettings.SenderName}");
                var client = new MailjetClient(_emailSettings.ApiKey, _emailSettings.SecretKey);

                var request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                .Property(Send.FromEmail, _emailSettings.SenderEmail)
                .Property(Send.FromName, _emailSettings.SenderName)
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, isHtml ? body : null)
                .Property(Send.Recipients, new JArray {
                    new JObject {
                        {
                            "Email", toEmail
                        }
                    }
                });

                var response = await client.PostAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Info("Email sent to {Email} with subject: {Subject}", toEmail, subject);
                }
                else
                {
                    var responseData = response.GetData()?.ToString() ?? "(no response data)";
                    _logger.Error($"Failed to send email to {toEmail}. Status: {response.StatusCode}, Response: {responseData}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occurred while sending email to {toEmail}", ex);
                throw;
            }
        }
    }
}
