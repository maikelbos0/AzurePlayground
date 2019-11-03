using AzurePlayground.CommandHandlers.Decorators;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using System;

namespace AzurePlayground.CommandHandlers.Security {
    [InterfaceInjectable]
    [Audit]
    public sealed class ChangeUserProfileCommandHandler : ICommandHandler<ChangeUserProfileCommand> {
        private readonly IUserRepository _repository;

        public ChangeUserProfileCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<ChangeUserProfileCommand> Execute(ChangeUserProfileCommand command) {
            var result = new CommandResult<ChangeUserProfileCommand>();
            var user = _repository.GetActiveByEmail(command.Email);

            user.DisplayName = command.DisplayName;
            user.Description = command.Description;
            user.ShowEmail = command.ShowEmail;

            _repository.Update();

            return result;
        }
    }
}