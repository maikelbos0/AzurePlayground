namespace AzurePlayground.Utilities.Mail {
    public interface IMailClient {
        void Send(string to, string subject, string plainTextBody, string htmlBody);
    }
}