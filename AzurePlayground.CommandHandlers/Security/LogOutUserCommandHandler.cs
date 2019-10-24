using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using System;

namespace AzurePlayground.CommandHandlers.Security {
    [InterfaceInjectable]
    public sealed class LogOutUserCommandHandler : ICommandHandler<LogOutUserCommand> {
        private readonly IUserRepository _repository;

        public LogOutUserCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<LogOutUserCommand> Execute(LogOutUserCommand parameter) {
            var result = new CommandResult<LogOutUserCommand>();
            var user = _repository.TryGetByEmail(parameter.Email);

            if (user != null && user.Status == UserStatus.Active) {
                user.LogOut();
                _repository.Update();
            }
            else {
                // Since there is no user input, the user is not responsible for errors and we should not use the command result for feedback
                throw new InvalidOperationException($"Attempted to log out {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
            }

            return result;
        }
    }
}