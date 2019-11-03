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
            var user = _repository.GetActiveByEmail(parameter.Email);

            user.LogOut();
            _repository.Update();

            return result;
        }
    }
}