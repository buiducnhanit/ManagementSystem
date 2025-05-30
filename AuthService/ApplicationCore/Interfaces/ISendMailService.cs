﻿namespace ApplicationCore.Interfaces
{
    public interface ISendMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}
