using AzurePlayground.Utilities.Container;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;

namespace AzurePlayground.Utilities.Mail {
    [Injectable]
    public class MailClient : IMailClient {
        public void Send(string to, string subject, string plainTextBody, string htmlBody) {
            var client = new SendGridClient(ConfigurationManager.AppSettings["SendGrid.ApiKey"]);
            var message = new SendGridMessage() {
                From = new EmailAddress("maikel.bos0@gmail.com"),
                Subject = subject,
                PlainTextContent = plainTextBody,
                HtmlContent = htmlBody
            };

            message.AddTo(new EmailAddress(to));
            client.SendEmailAsync(message);
        }
    }
}