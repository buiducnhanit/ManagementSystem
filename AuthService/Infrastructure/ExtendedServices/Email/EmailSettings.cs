namespace Infrastructure.ExtendedServices.Email
{
    public class EmailSettings
    {
        //public string SmtpServer { get; set; } = string.Empty;
        //public int SmtpPort { get; set; } = 587;
        //public string SmtpUser { get; set; } = string.Empty;
        //public string SmtpPassword { get; set; } = string.Empty;
        public string SendGridApiKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Management System";
    }
}
