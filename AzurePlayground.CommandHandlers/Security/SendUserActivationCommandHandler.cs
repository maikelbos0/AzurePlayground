using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class SendUserActivationCommandHandler : ICommandHandler<SendUserActivationCommand> {
        private readonly IUserRepository _repository;
        private readonly IMailClient _mailClient;
        private readonly IMailTemplate<ActivationMailTemplateParameters> _template;

        public SendUserActivationCommandHandler(IUserRepository repository, IMailClient mailClient, IMailTemplate<ActivationMailTemplateParameters> template) {
            _repository = repository;
            _mailClient = mailClient;
            _template = template;
        }

        public CommandResult<SendUserActivationCommand> Execute(SendUserActivationCommand parameter) {
            var result = new CommandResult<SendUserActivationCommand>();
            var user = _repository.TryGetByEmail(parameter.Email);

            // Don't return errors to prevent leaking information
            if (user != null && user.Status == UserStatus.New) {
                user.GenerateActivationCode();

                _repository.Update();

                _mailClient.Send(_template.GetMessage(new ActivationMailTemplateParameters(user.Email, user.ActivationCode.Value), user.Email));
            }

            return result;
        }
    }
}