using System;
using System.Linq;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Commands.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class SendUserActivationCommandHandler : BaseUserCommandHandler, ISendUserActivationCommandHandler {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public SendUserActivationCommandHandler(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<SendUserActivationCommand> Execute(SendUserActivationCommand parameter) {
            var result = new CommandResult<SendUserActivationCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                // Don't return errors to prevent leaking information
                if (user != null && user.Status == UserStatus.New) {
                    user.ActivationCode = GetNewActivationCode();
                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        Type = UserEventType.ActivationCodeSent
                    });

                    context.SaveChanges();

                    SendActivationEmail(user);
                }
            }

            return result;
        }
    }
}