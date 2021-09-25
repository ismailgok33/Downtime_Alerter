using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using DowntimeAlerter.Application.Interfaces;

namespace DowntimeAlerter.Infrastructure.Notification
{
    public class NotificationSender : INotificationSender
    {
        private readonly NotificationSettings _config;

        public NotificationSender(IOptions<NotificationSettings> config)
        {
            _config = config.Value;
        }

        public async Task SendNotificationAsync(string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.SmtpUser));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_config.SmtpHost, _config.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.SmtpUser, _config.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }      
    }
}