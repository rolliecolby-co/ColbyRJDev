using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace ColbyRJ.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSenderOptions _options;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

        private async Task Execute(string subject, string message, string email)
        {
            _options = _configuration.GetSection("EmailSmtp").Get<EmailSenderOptions>();

            var msg = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            //msg.From.Add(new MailboxAddress("Info", "info@jrcws.com"));
            msg.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));

            msg.To.Add(new MailboxAddress("", email));

            msg.Subject = subject;

            bodyBuilder.HtmlBody = message;
            msg.Body = bodyBuilder.ToMessageBody();

            // send email
            using var smtp = new SmtpClient();

            //smtp.Connect("mail.jrcws.com", 8889, SecureSocketOptions.Auto);
            smtp.Connect(_options.Smtp, 8889, SecureSocketOptions.Auto);

            //smtp.Authenticate("info@jrcws.com", "!nf023");
            smtp.Authenticate(_options.AuthID, _options.AuthPwd);

            await smtp.SendAsync(msg);
            smtp.Disconnect(true);
        }
    }
}
