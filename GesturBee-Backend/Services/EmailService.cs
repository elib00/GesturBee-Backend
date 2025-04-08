using GesturBee_Backend.Services.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;

namespace GesturBee_Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            // Retrieve SMTP server settings from configuration (appsettings.json)
            _smtpHost = configuration["EmailSettings:SmtpHost"];
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
            _smtpUsername = configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = configuration["EmailSettings:SmtpPassword"];
            _fromEmail = configuration["EmailSettings:FromEmail"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("GesturBee", _fromEmail));
            message.To.Add(new MailboxAddress("GesturBee", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = body
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpHost, _smtpPort, false);
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
