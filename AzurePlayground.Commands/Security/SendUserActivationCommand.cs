using System;
using System.Linq;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class SendUserActivationCommand : BaseUserCommand, ISendUserActivationCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public SendUserActivationCommand(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserSendActivation> Execute(UserSendActivation parameter) {
            var result = new CommandResult<UserSendActivation>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                // Don't return errors to prevent leaking information
                if (user != null && !user.IsActive) {
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