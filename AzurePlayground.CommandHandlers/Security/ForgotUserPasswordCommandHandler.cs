using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class ForgotUserPasswordCommandHandler : BaseUserCommandHandler, ICommandHandler<ForgotUserPasswordCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        private readonly IMailClient _mailClient;
        private readonly IMailTemplate<PasswordResetMailTemplateParameters> _template;

        public ForgotUserPasswordCommandHandler(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IMailTemplate<PasswordResetMailTemplateParameters> template) {
            _playgroundContextFactory = playgroundContextFactory;
            _mailClient = mailClient;
            _template = template;
        }

        public CommandResult<ForgotUserPasswordCommand> Execute(ForgotUserPasswordCommand parameter) {
            var result = new CommandResult<ForgotUserPasswordCommand>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                // There is no error reporting to prevent information leaking
                if (user != null && user.Status == UserStatus.Active) {
                    var token = GetNewPasswordResetToken();

                    user.PasswordResetToken = new TemporaryPassword(token);
                    user.AddEvent(UserEventType.PasswordResetRequested);

                    _mailClient.Send(_template.GetMessage(new PasswordResetMailTemplateParameters(user.Email, token), user.Email));
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}
