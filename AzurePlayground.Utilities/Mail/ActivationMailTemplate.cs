using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using System.Net;

namespace AzurePlayground.Utilities.Mail {
    [Injectable]
    public class ActivationMailTemplate : IMailTemplate<ActivationMailTemplateParameters> {
        private readonly IAppSettings _appSettings;

        public ActivationMailTemplate(IAppSettings appSettings) {
            _appSettings = appSettings;
        }

        public MailMessage GetMessage(ActivationMailTemplateParameters parameters, string to) {
            return new MailMessage() {
                To = to,
                Subject = Resources.Security.ActivationEmailSubject,
                Body = Resources.Security.ActivationEmailBody
                    .Replace("{BaseUrl}", _appSettings["Application.BaseUrl"])
                    .Replace("{ActivationCode}", parameters.ActivationCode.ToString())
                    .Replace("{EmailEncoded}", WebUtility.UrlEncode(parameters.Email))
            };
        }
    }
}