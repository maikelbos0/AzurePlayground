using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;

namespace AzurePlayground.Utilities.Mail {
    public class MailClient {
        public void Send() {
            var apiKey = ConfigurationManager.AppSettings["SendGrid.ApiKey"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage() {
                From = new EmailAddress("maikel.bos0@gmail.com"),
                Subject = "Hello World from the SendGrid CSharp SDK!",
                PlainTextContent = "Hello, Email!",
                HtmlContent = "<strong>Hello, Email!</strong>"
            };

            msg.AddTo(new EmailAddress("maikel.bos0@gmail.com"));
            client.SendEmailAsync(msg);
        }
    }
}