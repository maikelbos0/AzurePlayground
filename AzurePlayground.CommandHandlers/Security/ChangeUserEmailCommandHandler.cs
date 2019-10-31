using AzurePlayground.Commands.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;

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
            throw new System.NotImplementedException();
        }
    }
}