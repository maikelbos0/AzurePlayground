using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Mail;
using System;
using System.Net;
using System.Security.Cryptography;

namespace AzurePlayground.Commands.Security {
    public class BaseUserCommand {
        private readonly IMailClient _mailClient;
        private readonly IAppSettings _appSettings;

        public BaseUserCommand(IMailClient mailClient, IAppSettings appSettings) {
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

        protected int GetNewActivationCode() {
            return new Random().Next(10000, int.MaxValue);
        }

        protected byte[] GetNewPasswordSalt() {
            byte[] salt = new byte[20];

            new RNGCryptoServiceProvider().GetBytes(salt);

            return salt;
        }

        protected byte[] GetPasswordHash(string password, byte[] salt, int iterations) {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);

            // Return 20 bytes because after that it repeats
            return pbkdf2.GetBytes(20);
        }
    }
}
