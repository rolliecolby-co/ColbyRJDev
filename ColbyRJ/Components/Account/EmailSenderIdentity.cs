using ColbyRJ.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ColbyRJ.Components.Account
{
    public class EmailSenderIdentity(IOptions<AuthMessageSenderOptions> optionsAccessor,
    ILogger<EmailSenderIdentity> logger, IConfiguration configuration) : IEmailSender<ApplicationUser>
    {
        private readonly ILogger logger = logger;
        private readonly IConfiguration _configuration = configuration;
        public EmailSenderOptions _options;

        public AuthMessageSenderOptions Options { get; } = optionsAccessor.Value;

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email,
            string confirmationLink) => SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by " +
            $"<a href='{confirmationLink}'>clicking here</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email,
            string resetLink) => SendEmailAsync(email, "Reset your password",
            $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email,
            string resetCode) => SendEmailAsync(email, "Reset your password",
            $"Please reset your password using the following code: {resetCode}");

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            await Execute(subject, message, toEmail);
        }

        public async Task Execute(string subject, string message, string toEmail)
        {
            _options = _configuration.GetSection("EmailSmtp").Get<EmailSenderOptions>();

            var msg = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            //msg.From.Add(new MailboxAddress("Info", "info@jrcws.com"));
            msg.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));

            msg.To.Add(new MailboxAddress("", toEmail));

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
    public class AuthMessageSenderOptions
    {
        public string? EmailAuthKey { get; set; }
    }
}
