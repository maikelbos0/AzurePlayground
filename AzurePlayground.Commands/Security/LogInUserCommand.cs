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
    public class LogInUserCommand : BaseUserCommand, ILogInUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public LogInUserCommand(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserLogIn> Execute(UserLogIn parameter) {
            var result = new CommandResult<UserLogIn>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user != null 
                    && user.IsActive
                    && user.PasswordHash.SequenceEqual(GetPasswordHash(parameter.Password, user.PasswordSalt, user.PasswordHashIterations))) {

                    // If we log in, the password reset is not needed anymore and leaving it is a security risk
                    user.PasswordResetTokenSalt = null;
                    user.PasswordResetTokenHash = null;
                    user.PasswordResetTokenHashIterations = null;
                    user.PasswordResetTokenExpiryDate = null;

                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        UserEventType_Id = UserEventType.LoggedIn.Id
                    });
                }
                else {
                    result.AddError(p => p.Email, "Invalid email or password");

                    if (user != null) {
                        user.UserEvents.Add(new UserEvent() {
                            Date = DateTime.UtcNow,
                            UserEventType_Id = UserEventType.FailedLogIn.Id
                        });
                    }
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}