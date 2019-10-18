using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class SendUserActivationCommandHandler : ICommandHandler<SendUserActivationCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        private readonly IMailClient _mailClient;
        private readonly IMailTemplate<ActivationMailTemplateParameters> _template;

        public SendUserActivationCommandHandler(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IMailTemplate<ActivationMailTemplateParameters> template) {
            _playgroundContextFactory = playgroundContextFactory;
            _mailClient = mailClient;
            _template = template;
        }

        public CommandResult<SendUserActivationCommand> Execute(SendUserActivationCommand parameter) {
            var result = new CommandResult<SendUserActivationCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                // Don't return errors to prevent leaking information
                if (user != null && user.Status == UserStatus.New) {
                    user.GenerateActivationCode();

                    context.SaveChanges();

                    _mailClient.Send(_template.GetMessage(new ActivationMailTemplateParameters(user.Email, user.ActivationCode.Value), user.Email));
                }
            }

            return result;
        }
    }
}