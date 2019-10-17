using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using System.Net;

namespace AzurePlayground.Utilities.Mail {
    [Injectable]
    public class PasswordResetMailTemplate : IMailTemplate<PasswordResetMailTemplateParameters> {
        private readonly IAppSettings _appSettings;

        public PasswordResetMailTemplate(IAppSettings appSettings) {
            _appSettings = appSettings;
        }

        public MailMessage GetMessage(PasswordResetMailTemplateParameters parameters, string to) {
            return new MailMessage() {
                To = to,
                Subject = Resources.Security.PasswordResetEmailSubject,
                Body = Resources.Security.PasswordResetEmailBody
                    .Replace("{BaseUrl}", _appSettings["Application.BaseUrl"])
                    .Replace("{TokenEncoded}", WebUtility.UrlEncode(parameters.Token))
                    .Replace("{EmailEncoded}", WebUtility.UrlEncode(parameters.Email))
            };
        }
    }
}