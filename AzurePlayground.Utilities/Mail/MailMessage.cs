namespace AzurePlayground.Utilities.Mail {
    public class MailMessage {
        public string To { get; set; }
        public string Subject { get; set; }
        public string PlainTextBody { get; set; }
        public string HtmlBody { get; set; }
    }
}