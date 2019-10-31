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
        private readonly IMailTemplate<ActivationMailTemplateParameters> _template;

        public ChangeUserEmailCommandHandler(IUserRepository repository, IMailClient mailClient, IMailTemplate<ActivationMailTemplateParameters> template) {
            _repository = repository;
            _mailClient = mailClient;
            _template = template;
        }

        public CommandResult<ChangeUserEmailCommand> Execute(ChangeUserEmailCommand command) {
            var result = new CommandResult<ChangeUserEmailCommand>();
            var user = _repository.TryGetByEmail(command.CurrentEmail);
            var existingUser = _repository.TryGetByEmail(command.NewEmail);

            if (user == null || user.Status != UserStatus.Active) {
                // Since there is no user input for email, the user is not responsible for these errors and we should not use the command result for feedback
                throw new InvalidOperationException($"Attempted to change email for {(user == null ? "non-existent" : "inactive")} user '{command.CurrentEmail}'");
            }

            if (!user.Password.Verify(command.Password)) {
                result.AddError(c => c.Password, "Invalid password");
            }

            if (existingUser != null) {
                result.AddError(c => c.NewEmail, "Email address already exists");
            }

            if (result.Success) {
                user.ChangeEmail(command.NewEmail);
                _mailClient.Send(_template.GetMessage(new ActivationMailTemplateParameters(user.Email, user.ActivationCode.Value), user.Email));
            }
            else {
                user.ChangeEmailFailed();
            }

            _repository.Update();

            return result;
        }
    }
}