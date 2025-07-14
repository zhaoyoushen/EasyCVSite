using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PersonalHomepage.Services
{
    public class EmailService : IEmailSender
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly bool _enableSsl;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            
            // 从配置中读取邮件设置
            var emailConfig = configuration.GetSection("EmailSettings");
            _smtpServer = emailConfig["SmtpServer"] ?? "smtp.example.com";
            _smtpPort = int.Parse(emailConfig["SmtpPort"] ?? "587");
            _smtpUsername = emailConfig["SmtpUsername"] ?? "";
            _smtpPassword = emailConfig["SmtpPassword"] ?? "";
            _senderEmail = emailConfig["SenderEmail"] ?? "noreply@example.com";
            _senderName = emailConfig["SenderName"] ?? "Personal Homepage";
            _enableSsl = bool.Parse(emailConfig["EnableSsl"] ?? "true");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_senderEmail, _senderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                
                message.To.Add(email);

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = _enableSsl;
                    
                    await client.SendMailAsync(message);
                }
                
                _logger.LogInformation($"Email sent to {email} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {email}: {ex.Message}");
                // 在开发环境中，我们仍然允许密码重置流程继续，即使邮件发送失败
                // 在生产环境中，可能需要抛出异常或采取其他措施
            }
        }
    }
}