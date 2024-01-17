using BHYT_BE.Common.AppSetting;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Net;
using System.Net.Mail;

namespace BHYT_BE.Internal.Adapter
{
    // Cấu hình dịch vụ gửi mail, giá trị Inject từ appsettings.json
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

    }
    public interface IEmailAdapter
    {
        Task SendEmailAsync(string email, string subject, string body, bool isHTML);
    }
    public class EmailAdapter : IEmailAdapter
    {
        private readonly MailSettings mailSettings;

        private readonly ILogger<EmailAdapter> logger;

        // mailSetting được Inject qua dịch vụ hệ thống
        // Có inject Logger để xuất log
        public EmailAdapter(IOptions<MailSettings> _mailSettings, ILogger<EmailAdapter> _logger)
        {
            mailSettings = _mailSettings.Value;
            logger = _logger;
            logger.LogInformation("Create SendMailService");
        }

        public async Task SendEmailAsync(string email, string subject, string body, bool isHTML)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var builder = new BodyBuilder();
            if (isHTML)
            {
                builder.HtmlBody = body;
            }
            else
            {
                builder.TextBody = body;
            }
            message.Body = builder.ToMessageBody();

            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await message.WriteToAsync(emailsavefile);

                logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                logger.LogError(ex.Message);
            }

            smtp.Disconnect(true);

            logger.LogInformation("send mail to: " + email);

        }
    }

}
