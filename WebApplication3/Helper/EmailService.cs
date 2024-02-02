using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace WebApplication3.Helper
{
    public class EmailService
    {
        // Method for sending password reset links
        public async Task SendResetLinkAsync(string toEmail, string callbackUrl)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Gmail SMTP port
            string smtpUsername = "ouchueyangschool@gmail.com";
            string appPassword = "zydr oxkh aenr sgez\r\n"; 

            var fromAddress = new MailAddress("ouchueyangschool@gmail.com", "Bookworms Online");
            var toAddress = new MailAddress(toEmail);

            using (var smtp = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUsername, appPassword)
            })
            {
                // Format the email body as HTML
                var body = $@"<p>Click the following link to reset your password:</p>
                     <a href=""{callbackUrl}"">{callbackUrl}</a>";

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "Password Reset",
                    Body = body,
                    IsBodyHtml = true // Set this to true to indicate HTML content
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
        }

        // New method for sending 2FA code
        public async Task SendTwoFactorCodeAsync(string toEmail, string twoFactorCode)
        {

            var fromAddress = new MailAddress("ouchueyangschool@gmail.com", "Bookworms Online");
            var toAddress = new MailAddress(toEmail);

            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ouchueyangschool@gmail.com", "zydr oxkh aenr sgez\r\n")
            })
            {
                // Format the email body as HTML
                var body = $@"<p>Your 2FA code is: {twoFactorCode}</p>";

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "Two-Factor Authentication Code",
                    Body = body,
                    IsBodyHtml = true // Set this to true to indicate HTML content
                })
                {
                    // Add error handling here if needed
                    await smtp.SendMailAsync(message);
                }
            }
        }
    }
}
