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
    public class RegisterUserCommand : BaseUserCommand, IRegisterUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public RegisterUserCommand(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserRegistration> Execute(UserRegistration parameter) {
            var result = new CommandResult<UserRegistration>();
            var user = new User() {
                Email = parameter.Email,
                PasswordSalt = GetNewPasswordSalt(),
                PasswordHashIterations = _passwordHashIterations,
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
                SendActivationEmail(user);
            }

            return result;
        }
    }
}
