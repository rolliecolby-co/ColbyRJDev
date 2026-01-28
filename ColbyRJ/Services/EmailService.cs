using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ColbyRJ.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailSenderOptions _options;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Send(MimeMessage mimeMessage)
        {
            _options = _configuration.GetSection("EmailSmtp").Get<EmailSenderOptions>();

            mimeMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));

            using var smtp = new SmtpClient();

            smtp.Connect(_options.Smtp, 8889, SecureSocketOptions.Auto);

            smtp.Authenticate(_options.AuthID, _options.AuthPwd);

            await smtp.SendAsync(mimeMessage);
            smtp.Disconnect(true);
        }
    }

    public interface IEmailService
    {
        Task Send(MimeMessage mimeMessage);
    }
}
