using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class RequestUserPasswordResetCommand : BaseUserCommand, IRequestUserPasswordResetCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        private readonly int _expirationInSeconds = 3600;

        public RequestUserPasswordResetCommand(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }
        public CommandResult<UserPasswordResetRequest> Execute(UserPasswordResetRequest parameter) {
            var result = new CommandResult<UserPasswordResetRequest>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                // There is no error reporting to prevent information leaking
                if (user != null && user.IsActive) {
                    var token = GetNewPasswordResetToken();

                    user.PasswordResetTokenSalt = GetNewPasswordSalt();
                    user.PasswordResetTokenHashIterations = _passwordHashIterations;
                    user.PasswordResetTokenHash = GetPasswordHash(token, user.PasswordResetTokenSalt, user.PasswordResetTokenHashIterations.Value);
                    user.PasswordResetTokenExpiryDate = DateTime.UtcNow.AddSeconds(_expirationInSeconds);

                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        UserEventType = UserEventType.PasswordResetRequested
                    });

                    SendPasswordResetEmail(user, token);
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}
