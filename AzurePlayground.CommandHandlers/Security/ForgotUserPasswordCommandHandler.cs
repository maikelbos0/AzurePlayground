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
    public class ForgotUserPasswordCommandHandler : BaseUserCommandHandler, IForgotUserPasswordCommandHandler {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        
        public ForgotUserPasswordCommandHandler(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }
        public CommandResult<UserForgotPassword> Execute(UserForgotPassword parameter) {
            var result = new CommandResult<UserForgotPassword>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                // There is no error reporting to prevent information leaking
                if (user != null && user.Status == UserStatus.Active) {
                    var token = GetNewPasswordResetToken();

                    user.PasswordResetToken = new TemporaryPassword(token);

                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        Type = UserEventType.PasswordResetRequested
                    });

                    SendPasswordResetEmail(user, token);
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}
