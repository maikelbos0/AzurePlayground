using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;

namespace AzurePlayground.CommandHandlers.Security {
    [InterfaceInjectable]
    public sealed class ChangeUserEmailCommandHandler : ICommandHandler<ChangeUserEmailCommand> {
        private readonly IUserRepository _repository;
        private readonly IMailClient _mailClient;
        private readonly IMailTemplate<ConfirmEmailMailTemplateParameters> _template;

        public ChangeUserEmailCommandHandler(IUserRepository repository, IMailClient mailClient, IMailTemplate<ConfirmEmailMailTemplateParameters> template) {
            _repository = repository;
            _mailClient = mailClient;
            _template = template;
        }

        public CommandResult<ChangeUserEmailCommand> Execute(ChangeUserEmailCommand command) {
            var result = new CommandResult<ChangeUserEmailCommand>();
            var user = _repository.GetByEmail(command.CurrentEmail, UserStatus.Active);
            var existingUser = _repository.TryGetByEmail(command.NewEmail);

            if (!user.Password.Verify(command.Password)) {
                result.AddError(c => c.Password, "Invalid password");
            }

            if (existingUser != null) {
                result.AddError(c => c.NewEmail, "Email address already exists");
            }

            if (result.Success) {
                user.RequestEmailChange(command.NewEmail);
                _mailClient.Send(_template.GetMessage(new ConfirmEmailMailTemplateParameters(user.Email, user.NewEmailConfirmationCode.Value), user.NewEmail));
            }
            else {
                user.RequestEmailChangeFailed();
            }

            _repository.Update();

            return result;
        }
    }
}