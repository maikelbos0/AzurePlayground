using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Mail;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace AzurePlayground.Commands.Security {
    public abstract class BaseUserCommandHandler {
        private readonly IMailClient _mailClient;
        private readonly IAppSettings _appSettings;
        private readonly char[] _tokenCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        public BaseUserCommandHandler(IMailClient mailClient, IAppSettings appSettings) {
            _mailClient = mailClient;
            _appSettings = appSettings;
        }

        protected void SendActivationEmail(User user) {
            var activationUrl = $"{_appSettings["Application.BaseUrl"]}Home/Activate/?activationCode={user.ActivationCode}&email={WebUtility.UrlEncode(user.Email)}";
            var subject = Resources.Security.ActivationEmailSubject.Replace("{ActivationUrl}", activationUrl).Replace("{ActivationCode}", user.ActivationCode.ToString());
            var body = Resources.Security.ActivationEmailBody.Replace("{ActivationUrl}", activationUrl).Replace("{ActivationCode}", user.ActivationCode.ToString());

            _mailClient.Send(new MailMessage() {
                To = user.Email,
                Subject = subject,
                Body = body
            });
        }

        protected void SendPasswordResetEmail(User user, string token) {
            var passwordResetUrl = $"{_appSettings["Application.BaseUrl"]}Home/ResetPassword/?email={WebUtility.UrlEncode(user.Email)}&token={WebUtility.UrlEncode(token)}";
            var subject = Resources.Security.PasswordResetEmailSubject.Replace("{PasswordResetUrl}", passwordResetUrl);
            var body = Resources.Security.PasswordResetEmailBody.Replace("{PasswordResetUrl}", passwordResetUrl);

            _mailClient.Send(new MailMessage() {
                To = user.Email,
                Subject = subject,
                Body = body
            });
        }

        protected int GetNewActivationCode() {
            return new Random().Next(10000, int.MaxValue);
        }

        protected string GetNewPasswordResetToken() {
            using (var rng = new RNGCryptoServiceProvider()) {
                // Establish a maximum based on the amount of characters to prevent bias
                var maximumNumber = (byte.MaxValue / _tokenCharacters.Length) * _tokenCharacters.Length;
                var tokenBuilder = new StringBuilder();
                byte[] buffer = new byte[1];

                for (var i = 0; i < 20; i++) {
                    // Get a new number as long as we're at or over the maximum number
                    do {
                        rng.GetBytes(buffer);
                    }
                    while (buffer[0] >= maximumNumber);

                    tokenBuilder.Append(_tokenCharacters[buffer[0] % _tokenCharacters.Length]);
                }

                return tokenBuilder.ToString();
            }
        }
    }
}