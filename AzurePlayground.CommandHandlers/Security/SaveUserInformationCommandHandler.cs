using AzurePlayground.CommandHandlers.Decorators;
using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using System;

namespace AzurePlayground.CommandHandlers.Security {
    [InterfaceInjectable]
    [Audit]
    public sealed class SaveUserInformationCommandHandler : ICommandHandler<SaveUserInformationCommand> {
        private readonly IUserRepository _repository;

        public SaveUserInformationCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<SaveUserInformationCommand> Execute(SaveUserInformationCommand command) {
            var result = new CommandResult<SaveUserInformationCommand>();
            var user = _repository.TryGetByEmail(command.Email);

            if (user != null && user.Status == UserStatus.Active) {
                user.DisplayName = command.DisplayName;
                user.Description = command.Description;
                user.ShowEmail = command.ShowEmail;

                _repository.Update();
            }
            else {
                // Since there is no user input, the user is not responsible for errors and we should not use the command result for feedback
                throw new InvalidOperationException($"Attempted to save user information for {(user == null ? "non-existent" : "inactive")} user '{command.Email}'");
            }

            return result;
        }
    }
}