using JPP.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class AccountEmailService : IAccountEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountEmailService> _logger;

        public AccountEmailService(IConfiguration configuration, ILogger<AccountEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendForgotPasswordEmailAsync(string emailAddress, string newPassword)
        {
            try
            {
                using var message = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress(GetRequiredSetting("Smtp:FromEmail")),
                    Subject = _configuration["LegacyLogin:PasswordResetSubject"] ?? "JPP Skin Laser Clinic - Password Reset",
                    Body = BuildForgotPasswordBody(newPassword)
                };

                message.To.Add(new MailAddress(emailAddress));

                var bcc = _configuration["LegacyLogin:PasswordResetBcc"];
                if (!string.IsNullOrWhiteSpace(bcc))
                {
                    message.Bcc.Add(new MailAddress(bcc));
                }

                using var client = new SmtpClient(GetRequiredSetting("Smtp:Host"))
                {
                    Port = int.TryParse(_configuration["Smtp:Port"], out var port) ? port : 25,
                    EnableSsl = bool.TryParse(_configuration["Smtp:EnableSsl"], out var enableSsl) && enableSsl,
                    Credentials = new NetworkCredential(
                        GetRequiredSetting("Smtp:Username"),
                        GetRequiredSetting("Smtp:Password"))
                };

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send forgot password email to {EmailAddress}", emailAddress);
                throw;
            }
        }

        private string BuildForgotPasswordBody(string newPassword)
        {
            var appName = _configuration["LegacyLogin:ApplicationName"] ?? "JPP Skin Laser Clinic";

            return $@"You have requested to reset your {appName} access password on {DateTime.Now}.<br />
                Please relogin using your current userid and the new password.<br /><br />
                Your new password is {WebUtility.HtmlEncode(newPassword)}<br /><br />
                Thank you.<br />
                Cell Box team<br /><br />
                (Please do not reply to this E-mail as it is automated and is unable to receive replies)";
        }

        private string GetRequiredSetting(string key)
        {
            var value = _configuration[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Missing configuration value: {key}");
            }

            return value;
        }
    }
}
