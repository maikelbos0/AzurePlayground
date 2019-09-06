using AzurePlayground.Models.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Database;
using Resources = AzurePlayground.Resources;
using System;
using System.Linq;
using System.Security.Cryptography;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class RegisterUserCommand : IRegisterUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        private readonly IMailClient _mailClient;
        private readonly int passwordHashIterations = 1000;

        public RegisterUserCommand(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient) {
            _playgroundContextFactory = playgroundContextFactory;
            _mailClient = mailClient;
        }

        public CommandResult<UserRegistration> Execute(UserRegistration parameter) {
            var result = new CommandResult<UserRegistration>();
            var user = new User() {
                Email = parameter.Email,
                PasswordSalt = GetNewPasswordSalt(),
                PasswordHashIterations = passwordHashIterations,
                ActivationCode = GetNewActivationCode()
            };

            user.PasswordHash = GetPasswordHash(parameter.Password, user.PasswordSalt, user.PasswordHashIterations);

            user.UserEvents.Add(new UserEvent() {
                Date = DateTime.UtcNow,
                UserEventType = UserEventType.Registered
            });

            using (var context = _playgroundContextFactory.GetContext()) {
                if (context.Users.Any(u => u.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))) {
                    result.AddError(p => p.Email, "Email address already exists");
                }
                else {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }

            if (!result.Errors.Any()) {
                var subject = Resources.Security.ActivationEmailSubject.Replace("{ActivationCode}", user.ActivationCode.ToString());
                var plainTextBody = Resources.Security.ActivationEmailPlainTextBody.Replace("{ActivationCode}", user.ActivationCode.ToString());
                var htmlBody = Resources.Security.ActivationEmailPlainTextBody.Replace("{ActivationCode}", user.ActivationCode.ToString());

                _mailClient.Send(user.Email, subject, plainTextBody, htmlBody);
            }

            return result;
        }

        private int GetNewActivationCode() {
            byte[] code = new byte[4];

            new RNGCryptoServiceProvider().GetBytes(code);

            return BitConverter.ToInt32(code, 0);
        }

        private byte[] GetNewPasswordSalt() {
            byte[] salt = new byte[20];

            new RNGCryptoServiceProvider().GetBytes(salt);

            return salt;
        }

        private byte[] GetPasswordHash(string password, byte[] salt, int iterations) {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);

            // Return 20 bytes because after that it repeats
            return pbkdf2.GetBytes(20);
        }
    }
}
