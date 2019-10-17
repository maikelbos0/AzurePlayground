using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class RegisterUserCommandHandler : BaseUserCommandHandler, ICommandHandler<RegisterUserCommand> {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        private readonly IMailClient _mailClient;
        private readonly IMailTemplate<ActivationMailTemplateParameters> _template;

        public RegisterUserCommandHandler(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IMailTemplate<ActivationMailTemplateParameters> template) {
            _playgroundContextFactory = playgroundContextFactory;
            _mailClient = mailClient;
            _template = template;
        }

        public CommandResult<RegisterUserCommand> Execute(RegisterUserCommand parameter) {
            var result = new CommandResult<RegisterUserCommand>();
            var user = new User() {
                Email = parameter.Email,
                Password = new Password(parameter.Password),
                Status = UserStatus.New,
                ActivationCode = GetNewActivationCode()
            };

            user.AddEvent(UserEventType.Registered);

            using (var context = _playgroundContextFactory.GetContext()) {
                if (context.Users.Any(u => u.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))) {
                    result.AddError(p => p.Email, "Email address already exists");
                }

                if (result.Success) {
                    context.Users.Add(user);
                    context.SaveChanges();

                    _mailClient.Send(_template.GetMessage(new ActivationMailTemplateParameters(user.Email, user.ActivationCode.Value), user.Email));
                }
            }

            return result;
        }
    }
}