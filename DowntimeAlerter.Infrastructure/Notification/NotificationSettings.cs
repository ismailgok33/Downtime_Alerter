namespace DowntimeAlerter.Infrastructure.Notification
{
    public class NotificationSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public string NotificationEmail { get; set; }
    }
}