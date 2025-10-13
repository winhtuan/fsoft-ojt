using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plantpedia.Helper;
using Plantpedia.Settings;

namespace Plantpedia.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _cfg;

        public EmailService(IOptions<EmailSettings> options) => _cfg = options.Value;

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_cfg.Host, _cfg.Port)
            {
                EnableSsl = _cfg.EnableSsl,
                Credentials = new NetworkCredential(_cfg.SenderEmail, _cfg.Password),
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_cfg.SenderEmail, _cfg.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };
            mail.To.Add(toEmail);

            LoggerHelper.Info($"Gửi email đến {toEmail} với tiêu đề: {subject}");
            await client.SendMailAsync(mail);
        }
    }
}
