using AzurePlayground.Commands.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Repositories.Security;
using AzurePlayground.Utilities.Container;
using System;

namespace AzurePlayground.CommandHandlers.Security {
    [InterfaceInjectable]
    public sealed class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand> {
        private readonly IUserRepository _repository;

        public DeactivateUserCommandHandler(IUserRepository repository) {
            _repository = repository;
        }

        public CommandResult<DeactivateUserCommand> Execute(DeactivateUserCommand parameter) {
            var result = new CommandResult<DeactivateUserCommand>();
            var user = _repository.GetByEmail(parameter.Email, UserStatus.Active);

            if (user.Password.Verify(parameter.Password)) {
                user.Deactivate();
            }
            else {
                result.AddError(p => p.Password, "Invalid password");
                user.DeactivationFailed();
            }

            _repository.Update();

            return result;
        }
    }
}