using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using System.Net;

namespace AzurePlayground.Utilities.Mail {
    [InterfaceInjectable]
    public sealed class ConfirmEmailMailTemplate : IMailTemplate<ConfirmEmailMailTemplateParameters> {
        private readonly IAppSettings _appSettings;

        public ConfirmEmailMailTemplate(IAppSettings appSettings) {
            _appSettings = appSettings;
        }

        public MailMessage GetMessage(ConfirmEmailMailTemplateParameters parameters, string to) {
            return new MailMessage() {
                To = to,
                Subject = "Please confirm your email address",
                Body = $"<p>Please <a href=\"{_appSettings["Application.BaseUrl"]}Home/ConfirmEmail/?activationCode={parameters.ActivationCode}&email={WebUtility.UrlEncode(parameters.Email)}\">click here to confirm your new email address</a>.</p>"
            };
        }
    }
}