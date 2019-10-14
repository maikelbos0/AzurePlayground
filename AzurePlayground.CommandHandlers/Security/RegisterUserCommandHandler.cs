using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class RegisterUserCommandHandler : BaseUserCommandHandler, IRegisterUserCommandHandler {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public RegisterUserCommandHandler(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserRegistration> Execute(UserRegistration parameter) {
            var result = new CommandResult<UserRegistration>();
            var user = new User() {
                Email = parameter.Email,
                Password = new Password(parameter.Password),
                Status = UserStatus.New,
                ActivationCode = GetNewActivationCode()
            };

            user.UserEvents.Add(new UserEvent() {
                Date = DateTime.UtcNow,
                Type = UserEventType.Registered
            });

            if (!parameter.Password.Equals(parameter.ConfirmPassword, StringComparison.Ordinal)) {
                result.AddError(p => p.ConfirmPassword, "Password and confirm password must match");
            }

            using (var context = _playgroundContextFactory.GetContext()) {
                if (context.Users.Any(u => u.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))) {
                    result.AddError(p => p.Email, "Email address already exists");
                }
                
                if (result.Success) {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }

            if (result.Success) {
                SendActivationEmail(user);
            }

            return result;
        }
    }
}
