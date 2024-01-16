using System;
using System.Net;
using System.Net.Mail;

namespace BHYT_BE.Internal.Adapter
{
    public class EmailAdapter
    {
        private readonly string smtpServer;
        private readonly int smtpPort;
        private readonly string smtpUsername;
        private readonly string smtpPassword;
        private readonly bool smtpEnableSSL;

        public EmailAdapter(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, bool smtpEnableSSL)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.smtpUsername = smtpUsername;
            this.smtpPassword = smtpPassword;
            this.smtpEnableSSL = smtpEnableSSL;
        }


        public void SendEmail(string to, string subject, string body, bool IsBodyHTML)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = smtpEnableSSL;

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpUsername),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = IsBodyHTML
                    };

                    mailMessage.To.Add(to);

                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here (log, display error to user, etc.)
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }

}
