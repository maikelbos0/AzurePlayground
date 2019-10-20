using AzurePlayground.Utilities.Mail;
using System.Collections.Generic;

namespace AzurePlayground.Test.Utilities {
    public sealed class FakeMailClient : IMailClient {
        public List<MailMessage> SentMessages { get; set; } = new List<MailMessage>();

        public void Send(MailMessage message) {
            SentMessages.Add(message);
        }
    }
}