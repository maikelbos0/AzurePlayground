using AzurePlayground.Commands.Security;
using AzurePlayground.Repositories.Security;

namespace AzurePlayground.CommandHandlers.Security {
    public sealed class ConfirmUserEmailChangeCommandHandler : ICommandHandler<ConfirmUserEmailChangeCommand> {
        private readonly IUserRepository _repository;

        public ConfirmUserEmailChangeCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<ConfirmUserEmailChangeCommand> Execute(ConfirmUserEmailChangeCommand command) {
            throw new System.NotImplementedException();
        }
    }
}